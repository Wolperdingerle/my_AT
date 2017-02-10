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

        private int _trend = 2;                 // Trendgröße
        private int _abstand = 2;               // Abstand in Ticks vom Low
        private bool _automatisch = true;       // Stopp-Order wird automatisch aktiviert
        private bool _profitOnly = true;        // Stopp-Order erst über Prifit im Promille aktvieren
        private int _teilverkauf = 2;           // Anzahl Teilverkäufe für die Position; Mindeszgröße 4.000 EUR
        private IOrder oBStop = null;           // Bewegungs-Stopp-Order
        private IOrder oTStop = null;           // Trend-Stopp-Order
        private double Stopp = 0.0;
        private int Stueck = 0;                 // Menge zu verkaufen
        private double _profit = 5;             // Mindestprofit in Promille
        private bool _sendMail = false;         // Email nach Ausführung zusenden
        private string BStopp = "Bewegungs-Soft-Stopp";
        private string TStopp = "Trend-Soft-Stopp";
        private bool _stopLimit = true;         // Stopp-Order als Stopp-Limit-Order
        private double Limit = 0.0;             // LimitPreis ist die Hälfte aus Stopp und Einstiegspreis berechnet
        private bool _softstopp = true;         // Stopp als Softstopp Bar by Bar
        private int _toleranz = 2;              // Toleranz in Tick bei InsideBars
        private bool TStopAktiv = false;        // Steuervariable Bewegungs / Trendstopp
    //    private Test2Plot _Test2Plot = null;

        #endregion

        protected override void OnInit()
		{
			CalculateOnClosedBar = true;
            RequiredBarsCount = 500;
            IsAutomated = _automatisch;
            Abstand = _abstand;
            Teilverkauf = _teilverkauf;
            SoftStopp = _softstopp;

        }
        protected override void OnStart()
        {
            base.OnStart();
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
                if(!TStopAktiv)
                { 
                #region Stopp-Order filled
                if ((TradeInfo != null && Trade.Quantity == 0 && oBStop != null ) || (oBStop != null && oBStop.OrderState == OrderState.Filled))
                {
                    //oBStop = null;
                   // Stopp = 0;
                }
                #endregion Stopp-Order-filled

                #region vorhandene Stop- oder StopLimit-Order holen falls eine vorhanden ist und noch nicht von der Strategie verwaltet wird
                if (oBStop == null && Orders.Count > 0)
                {
                    int i = 0;
                    do
                    {
                        if (Orders[i].Action == OrderAction.Sell && (Orders[i].OrderType == OrderType.Stop || Orders[i].OrderType == OrderType.StopLimit))
                        {
                            oBStop = Orders[i];
                            Stopp = oBStop.StopPrice;
                            Limit = oBStop.LimitPrice;
                            if (oBStop.OrderType == OrderType.Stop) _stopLimit = false;
                            else _stopLimit = true;
                        }
                        ++i;
                    } while (i < Orders.Count);
                }
                #endregion

                #region Stueck
                if (Trade == null || Trade != null && Trade.PositionType ==  PositionType.Flat)
                {
                    AddChartTextFixed("MyText", " kein Trade offen ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                    return;
                }
                if (Trade.Quantity * Trade.AvgPrice < 8000)     // verkaufe alles wenn unter 8.000 €
                {
                    Stueck = Trade.Quantity;
                }
                else if (Trade.Quantity * Trade.AvgPrice < 12000)   // verkaufe alles oder die Hälfte wenn 8.000 bis 12.000
                {
                    if (_teilverkauf > 0 )
                        Stueck = (int)(Trade.Quantity / 2);
                    else
                        Stueck = Trade.Quantity;
                }
                else if (_teilverkauf > 0 &&_teilverkauf * 4000 < (int)(Trade.Quantity * Trade.AvgPrice))        // verkaufe einen Teilbetrag >= 4000 €
                {
                    Stueck = (int)(Trade.Quantity/_teilverkauf);    
                }
                else
                {
                    //Stueck = 1 + (int)(Trade.Quantity * Trade.AvgPrice) / 4000; // verkaufe einen Teilbetrag mit ca. 4.000 €
                    Stueck = Trade.Quantity;                // verkaufe alles 
                }
                if (oBStop != null && oBStop.OrderState == OrderState.Filled)
                {
                    AddChartTextFixed("MyText", " Bewegunsstopp ausgeführt ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                    return;
                }

                if (Chart != null && _profitOnly) AddChartTextFixed("MyText", "Bewegunsstopp für " + Stueck.ToString("F0") + " Stück ProfitOnly " +
                    (_profit*Trade.AvgPrice*Stueck/1000).ToString("F2") + " €, Soft-Stopp: " + Stopp.ToString("F2"), TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                if (Chart != null && !_profitOnly) AddChartTextFixed("MyText", "Bewegunsstopp für "+ Stueck.ToString("F0") + " Stück  Soft-Stopp: " + Stopp.ToString("F2"), TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);

                if (Trade == null || (Trade != null && (Trade.PositionType == PositionType.Flat)))
                {
                    return;
                }   
                #endregion Stueck

                if (Core.PreferenceManager.IsAtrEntryDistance) _abstand = (int)Math.Max(_abstand, ATR(14)[1] * Core.PreferenceManager.AtrEntryDistanceFactor);    // Tick-Abstand

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
                            Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice)/2 + Trade.AvgPrice);
                            ReplaceOrder(oBStop, Stueck, Limit, Stopp);
                        }
                        else
                        {
                            CancelOrder(oBStop);
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
                
                if ((oBStop == null || (oBStop != null && oBStop.OrderState == OrderState.Cancelled)) && 
                    (!_profitOnly || (_profitOnly && (Stopp > Instrument.Round2TickSize(((1 + _profit/1000)* Trade.AvgPrice + _abstand* TickSize))))))  
                {
                    if (_softstopp)
                    {
                        Stopp = Instrument.Round2TickSize((1 + _profit / 1000) * Trade.AvgPrice);
                        Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                    }
                    if (_stopLimit)
                        oBStop = SubmitOrder(0, OrderAction.Sell, OrderType.StopLimit, Stueck, Limit, Stopp, "Stopp B", BStopp);
                    else
                        oBStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, Stueck, 0, Stopp, "Stopp B", BStopp);
                    if (_automatisch) oBStop.ConfirmOrder();
                }


                // Hardstopp auf Low[0] nachsetzen und damit aktivieren
                if (!_softstopp 
                    || ((oBStop != null) && _softstopp && Close[0] < Stopp))
                { 
                    if ( oBStop.OrderState != OrderState.Filled && oBStop.OrderState != OrderState.PendingSubmit &&
                         oBStop.OrderState != OrderState.PendingReplace && oBStop.OrderState != OrderState.PartFilled)
                    {
                        Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                        ReplaceOrder(oBStop, Stueck, Limit, 
                                     Instrument.Round2TickSize(Math.Max(oBStop.StopPrice, (Low[0] - _abstand * TickSize))));
                    }
                }
                    #endregion Hardstopp_Berechnung

                    if (oBStop != null) // || (oStop != null && oStop.OrderState != OrderState.Cancelled))
                    {
                        //  _Test2Plot.Zeichne(Stopp, oStop.StopPrice);

                        if (Chart != null)
                            AddChartTextFixed("MyText", "Bewegunsstopp für " + Stueck.ToString("F0") + " Stück  Soft-Stopp: " + Stopp.ToString("F2") +
                            " = " + ((Stopp - Trade.AvgPrice) * Stueck).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    }
               }
            
            else        // TStopAktiv, Trend-Stopp
            {
                #region Trendstopp
                Stueck = Trade.Quantity;
                // P3-Berechnung: im aufwärtstrend:
                if (P123Adv(_trend).ValidP3Price[1] > P123Adv(_trend).P1Price[1] && P123Adv(_trend).ValidP3DateTime[1] > P123Adv(_trend).P1DateTime[1])
                    Stopp = Instrument.Round2TickSize(Math.Max(Stopp, P123Adv(_trend).ValidP3Price[1])- _abstand * TickSize);
                else    // im Abwärtstrend
                    Stopp = Instrument.Round2TickSize(Math.Max(Stopp, P123Adv(_trend).P2Price[1]) - _abstand * TickSize);


                // 1 Trend-Hardstopp setzen wenn Position Stopp über Profit liegt

                if ((oTStop == null || (oTStop != null && oTStop.OrderState == OrderState.Cancelled)) &&
                    (!_profitOnly || (_profitOnly && (Stopp > Instrument.Round2TickSize(((1 + _profit / 1000) * Trade.AvgPrice + _abstand * TickSize))))))
                {
                    if (_softstopp)
                    {
                       // Stopp = Instrument.Round2TickSize((1 + _profit / 1000) * Trade.AvgPrice);
                       // Limit = Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                    }
                    _stopLimit = false; // Trend-Stopp arbeitet nur mit Stopp-Order
                    if (_stopLimit)
                        oTStop = SubmitOrder(0, OrderAction.Sell, OrderType.StopLimit, Stueck, Limit, Stopp, "Stopp B", TStopp);
                    else
                        oTStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, Stueck, 0, Stopp, "Stopp B", TStopp);
                    if (_automatisch) oTStop.ConfirmOrder();
                }


                // Hardstopp auf P3[1] nachsetzen und damit aktivieren
                if (oTStop != null && oTStop.StopPrice < Stopp)
                {
                    if (oTStop.OrderState != OrderState.Filled && oTStop.OrderState != OrderState.PendingSubmit &&
                         oTStop.OrderState != OrderState.PendingReplace && oTStop.OrderState != OrderState.PartFilled)
                    {
                        Limit = 0;     // nur Stopp-Order Instrument.Round2TickSize((Stopp - Trade.AvgPrice) / 2 + Trade.AvgPrice);
                        ReplaceOrder(oTStop, Stueck, Limit,
                                     Instrument.Round2TickSize(Math.Max(oTStop.StopPrice, Stopp)));
                    }
                }
                if (oTStop != null && Chart != null) // || (oStop != null && oStop.OrderState != OrderState.Cancelled))
                {
                    //  _Test2Plot.Zeichne(Stopp, oStop.StopPrice);

                    if (oTStop.OrderState != OrderState.Filled)
                        AddChartTextFixed("MyText", "Trendstopp für " + Stueck.ToString("F0") + " Stück  Hard-Stopp: " + Stopp.ToString("F2") +
                        " = " + ((Stopp - Trade.AvgPrice) * Stueck).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    else
                            AddChartTextFixed("MyText", "Trendstopp ausgeführt "+
                        " Gewinn: " + (Trade.ClosedProfitLoss - Trade.Commission).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    }
                #endregion Trendstopp 
            }
        }
        }
        #region Properties

        [Description("Anzahln Teilverkäufe")]
        [Category("Parameters")]
        public int Teilverkauf
        {
            get { return _teilverkauf; }
            set { _teilverkauf = value; }
        }

        [Description("Soft-Stopp")]
        [Category("Parameters")]
        public bool SoftStopp
        {
            get { return _softstopp; }
            set { _softstopp = value; }
        }

        [Description("nur Gewinn-Stopp")]
        [Category("Parameters")]
        public bool ProfitOnly
        {
            get { return _profitOnly; }
            set { _profitOnly = value; }
        }

        [Description("Mindest-Profit in Promille")]
        [Category("Parameters")]
        public double Profit
        {
            get { return _profit; }
            set { _profit = Math.Max(0, value); }
        }


        [Description("Trendgrösse (0 -3)")]
        [Category("Parameters")]
        public int Trend
        {
            get { return _trend; }
            set { _trend = Math.Min(3,Math.Max(value, 0)); }
        }


        [Description("Tick-Abstand")]
        [Category("Parameters")]
        public int Abstand
        {
            get { return _abstand; }
            set { _abstand = value; }
        }


        [Description("Toleranz-Abstand für InsideBars in Tick")]
        [Category("Parameters")]
        public int Toleranz
        {
            get { return _toleranz; }
            set { _toleranz = value; }
        }


        [Description("Stopp-Limit-Order")]
        [Category("Parameters")]
        public bool StopLimit
        {
            get { return _stopLimit; }
            set { _stopLimit = value; }
        }

        [Description("Automatisch")]
        [Category("Parameters")]
        public bool Automatisch
        {
            get { return _automatisch; }
            set { _automatisch = value; }
        }


        [Description("SendMail")]
        [Category("Parameters")]

        public bool SendMail
        {
            get { return _sendMail; }
            set { _sendMail = value; }
        }
        #endregion
	}
}
