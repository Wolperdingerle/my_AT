using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using AgenaTrader.API;
using AgenaTrader.Custom;
using AgenaTrader.Plugins;
using AgenaTrader.Helper;
using AgenaTrader.UserCode;

/// <summary>
/// 
/// Stopp und Limit auf volle 10 Ct runden wegen TWS-Error 110
/// Trendstop am P3, Trendstärke einstellbar.
/// 1. Hardstopp immer für die gesamte Position, wenn Profit überschritten wird.
/// Wird der 1. Hardstopp nachgezogen, dann nur für die Teilposition (einstellbar, min 4.000 €) Position
/// einstellbare Besonderheiten: 
/// Hardstopp (Stop-Order beim Broker) wird erst gesetzt wenn ein einstellbarer Gewinn in Promille zum Stopp aufgelaufen ist (ProfitOnly, Profit)
/// Abstand vom Low des P3-Bars
/// Toleranz bei InsideBars in Ticks
/// Teilverkauf: von der gesamte Position kann auch nur ein Teil mit dem Stopp verkauft werden. Die Mindestordergröße ist aber immer 4.000€. (Mindestprovision)
/// Die Trendstopp-Order liegt immer unter P3 als Stopp-Order bzw bestätigtem P2 im Abwärtstrend (bei Long-Position)
/// Parameter Vollkasko: beim 1. Trendstopp wird die gesamte Position geschützt.
/// bei Softstopp wird Hardstopp an den letzten Stopp nachgezogen wenn das Close unter dem Stopp schließt.
/// Neuer Stop für den REst/2.Tranche kommt im die Mitte zwischen Close[0] und Einstiegspreis (halber Gewinn wird gesichert, von da aus normale Stoppsetzung (im Abwätrstrend auf P2 mit vorhandenem P3?.
/// ToDo: 
/// Graphische Darstellung im Chart fehlt noch.
/// 
/// </summary>

namespace AgenaTrader.UserCode
{
    [Description("Trendstopp als Softstopp mit einstellbarem Volumen")]
    public class Trend_Stopp_S : UserStrategy
    {
        #region Variables
        
        private int _trend = 1;                 // Trendgröße
        private int _abstand = 2;               // Abstand in Ticks vom Low
        private double _LimitFaktor = 0.5;       // Faktor Abstand Stopp-Limit als fester %-Satz
        private bool _automatisch = true;       // Stopp-Order wird automatisch aktiviert
        private bool _profitOnly = true;        // Stopp-Order erst über Pr0fit im Promille aktvieren
        private int _teilverkauf = 3;           // Anzahl Teilverkäufe für die Position; Mindeszgröße 4.000 EUR
        private bool _softstopp = true;         // Softstopp statt Hardstopp nach dem 1. Stopp
        private IOrder oTStop = null;           // Trend-Stopp-Order
        public double Stopp = 0.0;
        private int Stueck = 0;                 // Menge zu verkaufen
        private double _profit = 5;             // Mindestprofit in Promille
        private bool _sendMail = true;         // Email nach Ausführung zusenden
        private string TStopp = "Trend ";
        private bool TotalSchutz = false;        // frühestmöglicher Schutz für Gesamtposition nur am 1. Stopp (Vollkasko)
        private bool _softstoppOnly = true;    // keine Hardstopp, wenn Softstopp aktiviert
        private bool _stopLimit = true;         // Stopp-Order als Stopp-Limit-Order
        private double Limit = 0.0;             // LimitPreis ist die Hälfte aus Stopp und Einstiegspreis bzw 1,5 x ATR(14) berechnet
        private double altStopp = 0.0;          // speichert alten Stopp-Preis für die Ausgabe des neuen Stopps im Ausgabefenster
        

        #endregion Variables

        protected override void OnInit()
        {
            CalculateOnClosedBar = true;
            RequiredBarsCount = Math.Max(10, RequiredBarsCount);
            IsAutoConfirmOrder = _automatisch;
            Abstand = _abstand;
            Teilverkauf = _teilverkauf;
            TraceOrders = true;

        }
        protected override void OnStart()
        {
            base.OnStart();
            TStopp = TStopp + _trend;
            if (TotalSchutz) TStopp = TStopp + " VK";
            if (_stopLimit) TStopp = TStopp + " SL";
            if (_softstopp) TStopp = TStopp + " SS";
            if (_softstoppOnly) TStopp = TStopp + " nur SS";

        }

        protected override void OnOrderExecution(IExecution execution)
        {

            if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
            {
                if (oTStop != null && execution.Name == oTStop.Name)
                {
                    if (_sendMail && Core.PreferenceManager.DefaultEmailAddress != "") this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.",
                         execution.Instrument.Symbol + "Trend-Stop-Order " + execution.Name + " ausgeführt. Profit:" + (Trade.ClosedProfitLoss - execution.Commission).ToString("F2"));

                    Stopp = 0;
                    oTStop = null; // Objekt löschen, damit startet Trendstopp erneut.
                }
            }
        }
        protected override void OnCalculate()
        {
            if (IsProcessingBarIndexLast)
            {
                #region Zeitbasierter Chart
                if (Trade == null || Trade != null && Trade.PositionType == PositionType.Flat)
                {
                    AddChartTextFixed("MyText", " Trendstopp aktiv, kein Trade offen ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                    if (oTStop != null) oTStop.CancelOrder(); 
                    return;
                }
                #endregion

                #region vorhandene Stop- oder StopLimit-Order holen falls eine vorhanden ist und noch nicht von der Strategie verwaltet wird
                if (oTStop == null && Orders.Count > 0)
                {
                    int i = 0;
                    do
                    {
                        if (Orders[i].Direction == OrderDirection.Sell && Orders[i].OrderState != OrderState.Filled && (Orders[i].OrderType == OrderType.Stop || Orders[i].OrderType == OrderType.StopLimit))
                        {
                            oTStop = Orders[i];

                            Stopp = Orders[i].StopPrice;
                            Limit = Orders[i].LimitPrice;
                            if (Orders[i].OrderType == OrderType.Stop) _stopLimit = false;
                            else _stopLimit = true;
                        }
                        ++i;
                    } while (i < Orders.Count);
                }
                #endregion
                if (Core.PreferenceManager.IsAtrEntryDistance) _abstand = (int)Math.Max(_abstand, ATR(14)[1] * Core.PreferenceManager.AtrEntryDistanceFactor);    // Tick-Abstand

                #region Stueck

                if ((int)(Trade.Quantity * Trade.AvgPrice / 4000) < _teilverkauf)
                    _teilverkauf = (int)(Trade.Quantity * Trade.AvgPrice / 4000);
                if (_teilverkauf <= 1) Stueck = Trade.Quantity;
                else Stueck = (int)(Trade.Quantity / _teilverkauf);

                if (Trade.Quantity * Trade.AvgPrice < 8000) Stueck = Trade.Quantity;


                #endregion Stueck


                #region Vollkasko
                if (oTStop == null && TotalSchutz) Stueck = Trade.Quantity;
                #endregion Vollkasko

                #region Hardstopp: 1. Stopp setzen
                if(!_softstoppOnly)
                { 
                if (Stopp > 0 &&(oTStop == null || (oTStop != null && oTStop.OrderState == OrderState.Cancelled)) &&
                  (!_profitOnly || (_profitOnly && (Stopp > Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize))))))
                  {
                      // setze 1. Stopp in die Mitte des bisher aufgelaufenen Profit
                      if(Stopp > ((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize))   // Gewinnstopp
                      { 
                           Stopp = Instrument.Round2TickSize((Stopp - Trade.AvgPrice)/2 + Trade.AvgPrice);
                           Limit = Instrument.Round2TickSize(Stopp * (1- _LimitFaktor/100));
                       }
                       if (_stopLimit)
                           oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.StopLimit, Stueck, Limit, Stopp, "Stopp T", TStopp);
                       else
                           oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, Stueck, 0, Stopp, "Stopp T", TStopp);
                       if (_automatisch) oTStop.ConfirmOrder();
                   }
                }
                #endregion


                #region Softstopp
                if(_softstoppOnly)
                {
                    if (Stopp > 0)
                    {
                        if (_softstopp && Close[0] < Stopp)
                        {
                            Stopp = Instrument.Round2TickSize(Close[0] - _abstand * TickSize);
                            Limit = Instrument.Round2TickSize(Stopp * (1 - _LimitFaktor / 100));
                            if (oTStop == null)
                            { 
                                if (_stopLimit)
                                    oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.StopLimit, Stueck, Limit, Stopp, "Stopp T", TStopp);
                                else
                                    oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, Stueck, 0, Stopp, "Stopp T", TStopp);
                            }
                            if (_automatisch) oTStop.ConfirmOrder();
                            else
                            { 
                                if (oTStop.OrderState != OrderState.Filled && oTStop.OrderState != OrderState.PendingSubmit &&
                                oTStop.OrderState != OrderState.PendingReplace && oTStop.OrderState != OrderState.PartFilled)
                                {
                                    if (_stopLimit) ReplaceOrder(oTStop, Stueck, Instrument.Round2TickSize(Limit), Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                                    else ReplaceOrder(oTStop, Stueck, 0, Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                                }
                            }
                        }
                    }
                }
                else
                { 
                    if (Stopp > 0 && oTStop != null)
                    { 
                       if (_softstopp && Close[0] < Stopp)
                        {
                            Stopp = Instrument.Round2TickSize(Close[0] - _abstand * TickSize);
                            Limit = Instrument.Round2TickSize(Stopp * (1 - _LimitFaktor / 100));

                            if (oTStop.OrderState != OrderState.Filled && oTStop.OrderState != OrderState.PendingSubmit &&
                                oTStop.OrderState != OrderState.PendingReplace && oTStop.OrderState != OrderState.PartFilled)
                            {
                                if (_stopLimit) ReplaceOrder(oTStop, Stueck, Instrument.Round2TickSize(Limit), Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                                else ReplaceOrder(oTStop, Stueck, 0, Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                            }   
                        }
                    }
                }
                #endregion Softstopp

                #region Hardstopp nachziehen
                if (Stopp > 0 && oTStop != null)
                {
                    if ((oTStop.StopPrice < Stopp && !_softstopp) || (Stopp > oTStop.StopPrice && oTStop.StopPrice < Trade.AvgPrice))
                    {
                        if (oTStop.OrderState != OrderState.Filled && oTStop.OrderState != OrderState.PendingSubmit &&
                            oTStop.OrderState != OrderState.PendingReplace && oTStop.OrderState != OrderState.PartFilled)
                        {
                            if(Stopp > Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize)) && oTStop.StopPrice < Trade.AvgPrice)    // 1. Stopp einmalig in den Gewinn ziehen.
                            {
                                Stopp = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                                Limit = Instrument.Round2TickSize(Stopp * (1 - _LimitFaktor / 100));
                                if (_stopLimit) ReplaceOrder(oTStop, Stueck, Instrument.Round2TickSize(Limit), Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                                else ReplaceOrder(oTStop, Stueck, 0, Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                            }
                            else
                            { 
                                if (_stopLimit) ReplaceOrder(oTStop, Stueck, Instrument.Round2TickSize(Limit), Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                                else ReplaceOrder(oTStop, Stueck, 0, Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                            }
                        }
                    }
                }
                #endregion Hardstopp

                #region Trend-Stopp-Berechnung: 
                if(Bars[2].Time > Trade.CreatedDateTime )
                {
                    if (P123Adv(_trend).ValidP3Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).ValidP3DateTime[0] > P123Adv(_trend).P1DateTime[0])
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, (P123Adv(_trend).ValidP3Price[0] - _abstand * TickSize)));       //im Aufwärtstrend: P3 gültig
                    else if (P123Adv(_trend).ValidP3Price[0] < 1 && P123Adv(_trend).P2Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).P2DateTime[0] > P123Adv(_trend).P1DateTime[0])
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, (P123Adv(_trend).P1Price[0]) - _abstand * TickSize));            // im Aufwärtsttrend, noch kein P3 vorhanden,  Stopp am P1
                    else if (P123Adv(_trend).P2Price[0] < P123Adv(_trend).P1Price[0] && P123Adv(_trend).TempP3DateTime[0] > P123Adv(_trend).P2DateTime[0]
                           && P123Adv(_trend).P2DateTime[0] < P123Adv(_trend).TempP3DateTime[0])
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize));           //im Abwärtstrend: TempP3 gültig, Stopp am letztenP2
                    
                        Limit = Instrument.Round2TickSize(Stopp * (1 - _LimitFaktor / 100));
                }
                #endregion Trend-Stopp-Berechnung
                
            }
            #region Anzeige
            if (altStopp != Stopp)
            {
                if (_stopLimit) Print(Bars[0].Time + " " + Instrument.Symbol + " Stopp: " + Stopp.ToString("F2") + " Limit: " + Limit.ToString("F2"));
                else Print(Bars[0].Time + " " + Instrument.Symbol + " Stopp: " + Stopp.ToString("F2"));
                altStopp = Stopp;
            }
            if (Chart != null)
            {
                //  _Test2Plot.Zeichne(Stopp, oStop.StopPrice);
                if (oTStop == null)
                    if (altStopp != Stopp)
                        AddChartTextFixed("MyText", "Trendstopp aktiv, Teilverkäufe: " + _teilverkauf + " " + TStopp + " Stopp: " + Stopp.ToString("F2"), TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    else
                        AddChartTextFixed("MyText", "Trendstopp aktiv, Teilverkäufe: " + _teilverkauf + " " + TStopp + " Stopp: " + Stopp.ToString("F2"), TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                else
                {
                    if (oTStop.OrderState == OrderState.Filled)
                        AddChartTextFixed("MyText", "Trendstopp ausgeführt", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                    else
                      if (_softstopp)
                        AddChartTextFixed("MyText", "Softstopp aktiv, Trend " + _trend + " Softstopp: " + Stopp.ToString("F2") +
                          " G/V " + ((oTStop.StopPrice - Trade.AvgPrice) * Stueck).ToString("F2") + " € " + TStopp, TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    else
                    {
                        if (_stopLimit)
                            AddChartTextFixed("MyText", "Trendstopp aktiv, Trend " + _trend +
                              " G/V " + ((oTStop.LimitPrice - Trade.AvgPrice) * Stueck).ToString("F2") + " € " + TStopp, TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                        else
                            AddChartTextFixed("MyText", "Trendstopp aktiv, Trend " + _trend +
                              " G/V " + ((oTStop.StopPrice - Trade.AvgPrice) * Stueck).ToString("F2") + " € " + TStopp, TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    }
                }      
            }
            #endregion Anzeige
        }

     
        #region Properties

        [Description("Anzahl Teilverkäufe")]
        [InputParameter]
        public int Teilverkauf
        {
            get { return _teilverkauf; }
            set { _teilverkauf = value; }
        }

        [Description("nur Gewinn-Stopp")]
        [InputParameter]
        public bool ProfitOnly
        {
            get { return _profitOnly; }
            set { _profitOnly = value; }
        }


        [Description("Soft-Stopp statt Hardstopp")]
        [InputParameter]
        public bool SoftStopp
        {
            get { return _softstopp; }
            set { _softstopp = value; }
        }

        [Description("ausschließlich Soft-Stopp, kein Hardstopp")]
        [InputParameter]
        public bool Soft_Only
        {
            get { return _softstoppOnly; }
            set { _softstoppOnly = value; }
        }
        

        [Description("Mindest-Profit in Promille")]
        [InputParameter]
        public double Profit
        {
            get { return _profit; }
            set { _profit = Math.Max(0, value); }
        }


        [Description("Trendgrösse (0 -3)")]
        [InputParameter]
        public int Trend
        {
            get { return _trend; }
            set { _trend = Math.Min(3,Math.Max(value, 0)); }
        }


        [Description("Tick-Abstand")]
        [InputParameter]
        public int Abstand
        {
            get { return _abstand; }
            set { _abstand = value; }
        }


        [Description("%-Faktor Abstand Stopp-Limit-Preis")]
        [InputParameter]
        public double Limit_Faktor
        {
            get { return _LimitFaktor; }
            set { _LimitFaktor = value; }
        }


        [Description("Stopp-Limit-Order")]
        [InputParameter]
        public bool StopLimit
        {
            get { return _stopLimit; }
            set { _stopLimit = value; }
        }

        [Description("Vollkasko für die Gesamtposition")]
        [InputParameter]
        public bool Vollkasko
        {
            get { return TotalSchutz; }
            set { TotalSchutz = value; }
        }
        

        [Description("SendMail")]
        [InputParameter]

        public bool SendMail
        {
            get { return _sendMail; }
            set { _sendMail = value; }
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Trend-Stopp (S)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Trend-Stopp (S)";
            }
        }

    }
}
        #endregion Properties