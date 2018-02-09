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

/// <summary>
/// Bewegungs-Softstopp nach Markttechnik für die halbe Position
/// für den Rest Trendstop am P3, Trendstärke einstellbar.
/// 1. Hardstopp immer für die gesamte Position, wenn Profit überschritten wird.
/// Wird der 1. Hardstopp nachgezogen, dann nur für fie halbe Position
/// einstellbare Besonderheiten: 
/// Hardstopp (Stop-Order beim Broker) wird erst gesetzt wenn ein einstellbarer Gewinn in Promille zum Stopp aufgelaufen ist (ProfitOnly, Profit)
/// Abstand vom Low/High des letzten Bars 
/// Toleranz bei InsideBars in Ticks
/// Teilverkauf: von der gesamte Position kann auch nur ein Teil mit dem Stopp verkauft werden. Die Mindestordergröße ist aber immer 4.000€. (Mindestprovision)
/// Die Trendstopp-Order liegt immer unter P3 als Stopp-Order
/// Parameter Vollkasko: beim 1. Trendstopp wird die gesamte Position geschützt.
/// ToDo: 
/// Graphische Darstellung im Chart fehlt noch.
/// 
/// </summary>

namespace AgenaTrader.UserCode
{
    [Description("Bewegungs und Trendstopp als Softstopp mit einstellbarem Volumen")]

    public class BT_Soft_Stop : UserStrategy
    {
        #region Variables
        private bool _trendStoppOnly = true;    // Schalter Bewegung +  Trend oder nur Trendstopp
        private int _trend = 1;                 // Trendgröße
        private int _abstand = 2;               // Abstand in Ticks vom Low
        private bool _automatisch = true;       // Stopp-Order wird automatisch aktiviert
        private bool _profitOnly = true;        // Stopp-Order erst über Prifit im Promille aktvieren
        private int _teilverkauf = 3;           // Anzahl Teilverkäufe für die Position; Mindeszgröße 4.000 EUR
        private IOrder oBStop = null;           // Bewegungs-Stopp-Order
        private IOrder oTStop = null;           // Trend-Stopp-Order
        private double Stopp = 0.0;
        private int Stueck = 0;                 // Menge zu verkaufen
        private double _profit = 5;             // Mindestprofit in Promille
        private bool _sendMail = true;         // Email nach Ausführung zusenden
        private string BStopp = "Bewegungs-Soft-Stopp";
        private string TStopp = "Trend-Soft-Stopp";
        private bool TotalSchutz = false;        // frühestmöglicher Schutz für Gesamtposition
        private bool _stopLimit = false;         // Stopp-Order als Stopp-Limit-Order
        private double Limit = 0.0;             // LimitPreis ist die Hälfte aus Stopp und Einstiegspreis berechnet
        private bool _softstopp = false;         // Stopp als Softstopp Bar by Bar
        private int _toleranz = 2;              // Toleranz in Tick bei InsideBars
        private bool TStopAktiv = true;        // Steuervariable Bewegungs / Trendstopp
        private bool Stueck_Merker = true;      
        //    private Test2Plot _Test2Plot = null;

        #endregion

        protected override void OnInit()
        {
            CalculateOnClosedBar = true;
            RequiredBarsCount = 500;
            IsAutoConfirmOrder = _automatisch;
            Abstand = _abstand;
            Teilverkauf = _teilverkauf;
            SoftStopp = _softstopp;

        }
        protected override void OnStart()
        {
            base.OnStart();
            if (_trendStoppOnly) TStopAktiv = true;


            //     _Test2Plot = new Test2Plot();
            //Print("b");
        }

        protected override void OnOrderExecution(IExecution execution)
        {

            if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
            {
                if (oBStop != null && execution.Name == oBStop.Name)
                {
                    TStopAktiv = true;  // Trend-Stopp aktivieren
                    Stopp = 0;          // Wechsel auf Trendstopp, deshalb zurück auf 0
                    if (_sendMail && Core.PreferenceManager.DefaultEmailAddress != "") this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.",
                         execution.Instrument.Symbol + "Bewegungs-Stop-Order " + execution.Name + " ausgeführt. Profit:" + (Trade.ClosedProfitLoss - execution.Commission).ToString("F2"));

                }
                if (oTStop != null && execution.Name == oTStop.Name)
                {
                    if (_sendMail && Core.PreferenceManager.DefaultEmailAddress != "") this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.",
                         execution.Instrument.Symbol + "Trend-Stop-Order " + execution.Name + " ausgeführt. Profit:" + (Trade.ClosedProfitLoss - execution.Commission).ToString("F2"));

                }
            }
        }
        protected override void OnCalculate()
        {
            if (IsProcessingBarIndexLast)
            {
                if (Trade == null || Trade != null && Trade.PositionType == PositionType.Flat)
                {
                    AddChartTextFixed("MyText", " kein Trade offen ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                    return;
                }
                #region vorhandene Stop- oder StopLimit-Order holen falls eine vorhanden ist und noch nicht von der Strategie verwaltet wird
                if ((oBStop == null && !TStopAktiv || oTStop == null && TStopAktiv) && Orders.Count > 0)
                {
                    int i = 0;
                    do
                    {
                        if (Orders[i].Direction == OrderDirection.Sell && Orders[i].OrderState != OrderState.Filled && (Orders[i].OrderType == OrderType.Stop || Orders[i].OrderType == OrderType.StopLimit))
                        {
                            if (TStopAktiv) oTStop = Orders[i];
                            else oBStop = Orders[i];
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

                if (Stueck_Merker)
                {
                    Stueck_Merker = false;
                    if ((int)(Trade.Quantity * Trade.AvgPrice / 4000) < _teilverkauf)
                        _teilverkauf = (int)(Trade.Quantity * Trade.AvgPrice / 4000);
                    if (_teilverkauf <= 1) Stueck = Trade.Quantity;
                    else Stueck = (int)(Trade.Quantity / _teilverkauf);
                }

                if (Trade.Quantity * Trade.AvgPrice < 8000) Stueck = Trade.Quantity;


                #endregion Stueck
              
                if (!TStopAktiv)
                {
                    #region Bewegungsstopp



                    #region Stopp_Berechnung
                    // Stopp-Berechnung: bei InsideBar zurück auf Aussenstab, sonst BarByBar

                    if (InsideBarsMT(Close, InsideBarsMTToleranceUnit.Ticks, _toleranz).IsInsideBar[0] > 0)
                    {
                        Stopp = Instrument.Round2TickSize(InsideBarsMT(Close, InsideBarsMTToleranceUnit.Ticks, _toleranz).LowBeforeOutsideBar[0] - _abstand * TickSize);
                        // ggf oStop zurücksetzen
                        if (oBStop != null && oBStop.StopPrice > Stopp)
                        {
                            if ((!_profitOnly || (_profitOnly && (Stopp > ((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize)))))    // neuer Stopp auch im Gewinn
                            {
                                Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                                ReplaceOrder(oBStop, Stueck, Limit, Stopp);
                            }
                            else
                            {
                               oBStop.CancelOrder();
                                oBStop = null;
                                Limit = 0;
                            }
                        }
                    }
                    else
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, (Low[1] - _abstand * TickSize)));

                    #endregion Stopp_Berechnung


                    #region Hardstopp_Berechnung

                    // 1 Hardstopp setzen wenn Position Stopp über Profit liegt

                    if (Position.CreatedDateTime < Time[1] && (oBStop == null || (oBStop != null && oBStop.OrderState == OrderState.Cancelled)) &&
                        (!_profitOnly || (_profitOnly && (Stopp > Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize))))))
                    {
                        if (_softstopp)
                        {
                            Stopp = Instrument.Round2TickSize((1 + _profit / 1000) * Trade.AvgPrice);
                            Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                        }
                        if (TotalSchutz)   //(TotalSchutz) //
                        {
                            if (_stopLimit)
                                oBStop = SubmitOrder(0, OrderDirection.Sell, OrderType.StopLimit, Trade.Quantity, Limit, Stopp, "Stopp B", BStopp);
                            else
                                oBStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, Trade.Quantity, 0, Stopp, "Stopp B", BStopp);
                        }
                        else
                        {
                            if (_stopLimit)
                                oBStop = SubmitOrder(0, OrderDirection.Sell, OrderType.StopLimit, Stueck, Limit, Stopp, "Stopp B", BStopp);
                            else
                                oBStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, Stueck, 0, Stopp, "Stopp B", BStopp);
                        }
                        if (_automatisch) oBStop.ConfirmOrder();
                    }
                    
                    // Hardstopp auf Low[0] nachsetzen und damit aktivieren
                    if (oBStop != null && (!_softstopp || _softstopp && Close[0] < Stopp))
                    {
                        if (oBStop.OrderState != OrderState.Filled && oBStop.OrderState != OrderState.PendingSubmit &&
                             oBStop.OrderState != OrderState.PendingReplace && oBStop.OrderState != OrderState.PartFilled)
                        {
                            Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                            ReplaceOrder(oBStop, Stueck, Limit,
                                         Instrument.Round2TickSize(Math.Max(oBStop.StopPrice, (Low[0] - _abstand * TickSize))));
                        }
                    }
                    #endregion Hardstopp_Berechnung

                    #endregion Bewegungsstopp
                }

                else        // TStopAktiv, Trend-Stopp
                {
                    #region Trendstopp

                    _softstopp = false; // Trendstopp nur ohne Stoftstop
                    _stopLimit = false; // Trend-Stopp arbeitet nur mit Stopp-Order

                    #region Stopp-Berechnung: 

                    if (P123Adv(_trend).ValidP3Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).ValidP3DateTime[0] > P123Adv(_trend).P1DateTime[0])
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, P123Adv(_trend).ValidP3Price[0]) - _abstand * TickSize);       //im Aufwärtstrend: P3 gültig
                    else if (P123Adv(_trend).ValidP3Price[0] < 1 && P123Adv(_trend).P2Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).P2DateTime[0] > P123Adv(_trend).P1DateTime[0])  // im Abwärtstrend
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, P123Adv(_trend).P1Price[0]) - _abstand * TickSize);               // im Aufwärtsttrend, noch kein P3 vorhanden
                    else
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, P123Adv(_trend).P2Price[0]) - _abstand * TickSize);               // im Abwärtstrend

                    Limit = 0;     // nur Stopp-Order Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                    #endregion Stopp-Berechnung:

                    // 1 Trend-Hardstopp setzen wenn Position Stopp über Profit liegt
                    if (TotalSchutz)
                    {
                        if ((oTStop == null || (oTStop != null && oTStop.OrderState == OrderState.Cancelled)) &&
                            (!_profitOnly || (_profitOnly && (Low[1] > Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize))))))
                        {
                            Stopp = Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize));
                            if (_stopLimit)
                                oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.StopLimit, Trade.Quantity, Limit, Stopp, "Stopp B", TStopp);
                            else
                                oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, Trade.Quantity, 0, Stopp, "Stopp B", TStopp);
                        }
                        if (_automatisch) oTStop.ConfirmOrder();
                    }
                    else
                    { 
                        if ((oTStop == null || (oTStop != null && oTStop.OrderState == OrderState.Cancelled)) &&
                            (!_profitOnly || (_profitOnly && (Stopp > Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize))))))
                        {
                            if (_softstopp)
                            {
                            // Stopp = Instrument.Round2TickSize((1 + _profit / 1000) * Trade.AvgPrice);
                            // Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                            }
                        
                            if (_stopLimit)
                                oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.StopLimit, Stueck, Limit, Stopp, "Stopp B", TStopp);
                            else
                                oTStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, Stueck, 0, Stopp, "Stopp B", TStopp);
                        
                            if (_automatisch) oTStop.ConfirmOrder();

                       }
                    }
                    // Hardstopp auf P3[1] nachsetzen und damit aktivieren
                    if (oTStop != null && oTStop.StopPrice < Stopp)
                    {
                        if (oTStop.OrderState != OrderState.Filled && oTStop.OrderState != OrderState.PendingSubmit &&
                            oTStop.OrderState != OrderState.PendingReplace && oTStop.OrderState != OrderState.PartFilled)
                        {
                            ReplaceOrder(oTStop, Stueck, Limit,
                                         Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                        }
                    }
                }

                #endregion Trendstopp

                #region Anzeige
                if (Chart != null)
                {
                    if (!TStopAktiv) // Bewegungsstopp-Anzeigen)
                    {
                        //  _Test2Plot.Zeichne(Stopp, oStop.StopPrice);
                        if (oBStop == null)
                            AddChartTextFixed("MyText", "Bewegungs-Stopp aktiv, Teilverkäufe: " + _teilverkauf, TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                        else
                        {
                            if (oBStop.OrderState != OrderState.Filled)
                                AddChartTextFixed("MyText", "Bewegungs-Stopp für " + Stueck.ToString("F0") + " Stück  Hard-Stopp: " + Stopp.ToString("F2") +
                                " = " + ((Stopp - Trade.AvgPrice) * Stueck).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                            else
                                AddChartTextFixed("MyText", "Bewegungs-Stopp ausgeführt " +
                                " Gewinn: " + (Trade.ClosedProfitLoss - Trade.Commission).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                        }
                    }
                    else
                    {
                        if (oTStop == null)
                            AddChartTextFixed("MyText", "Trendstopp aktiv, Trend " + _trend + " Teilverkäufe: " + _teilverkauf, TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                        else
                        {
                            if (oTStop.OrderState == OrderState.Filled)
                                AddChartTextFixed("MyText", "Trendstopp ausgeführt", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);

                            else
                                AddChartTextFixed("MyText", "Trendstopp für " + Stueck.ToString("F0") + " Stück  Hard-Stopp: " + Stopp.ToString("F2") +
                                  " = " + ((Stopp - Trade.AvgPrice) * Stueck).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                        }
                    }
                }
                #endregion Anzeige
            }
        }
     
    
        
        #region Properties

        [Description("nur Trendstopp")]
        [InputParameter]
        public bool TrenStoppOnly
        {
            get { return _trendStoppOnly; }
            set { _trendStoppOnly = value; }
        }

        [Description("Anzahln Teilverkäufe")]
        [InputParameter]
        public int Teilverkauf
        {
            get { return _teilverkauf; }
            set { _teilverkauf = value; }
        }

        [Description("Soft-Stopp")]
        [InputParameter]
        public bool SoftStopp
        {
            get { return _softstopp; }
            set { _softstopp = value; }
        }

        [Description("nur Gewinn-Stopp")]
        [InputParameter]
        public bool ProfitOnly
        {
            get { return _profitOnly; }
            set { _profitOnly = value; }
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


        [Description("Toleranz-Abstand für InsideBars in Tick")]
        [InputParameter]
        public int Toleranz
        {
            get { return _toleranz; }
            set { _toleranz = value; }
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
        #endregion
	}
}
