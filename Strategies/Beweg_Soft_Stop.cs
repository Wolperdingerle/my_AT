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
/// Bewegungs-Softstopp nach Markttechnik
/// einstellbare Besonderheiten: 
/// Hardstopp (Stop-Order beim Broker) wird erst gesetzt wenn ein einstellbarer Gewinn in Promille zum Stopp aufgelaufen ist (ProfitOnly, Profit)
/// Abstand vom Low/High des letzten Bars 
/// Toleranz bei InsideBars in Ticks
/// Teilverkauf: von der gesamte Position kann auch nur ein Teil mit dem Stopp verkauft werden. Die Mindestordergröße ist aber immer 4.000€. (Mindestprovision)
/// 
/// ToDo: 
/// Bei Stopp-Limit-Order muß ggf die Differenz zwischen Stop- und Limit-Preis manuell im Tradebar eingegeben werden. Das ist zur Zeit nicht programmtechnisch möglich.
/// Grapgische Darstellung im Chart fehlt noch.
/// 
/// </summary>

namespace AgenaTrader.UserCode
{
	[Description("Bewegungsstopp als Softstopp mit einstellbarem Volumen")]
   
    public class Beweg_Soft_Stop : UserStrategy
	{
        #region Variables
       
        private int _abstand = 2;               // Abstand in Ticks vom Low
        private bool _automatisch = true;       // Stopp-Order wird automatisch aktiviert
        private bool _profitOnly = true;        // Stopp-Order erst über Prifit im Promille aktvieren
        private int _teilverkauf = 0;           // Anzahl Teilverkäufe für die Position; Mindeszgröße 4.000 EUR
        private IOrder oStop;
        private double Stopp;
        private int Stueck;                     // Menge zu verkaufen
        private double _profit = 4;             // Mindestprofit in Promille
        private bool _sendMail = true;          // Email nach Ausführung zusenden
        private string BStopp = "Bewegungs-Soft-Stopp";
        private bool _stopLimit = false;           // Stopp-Order als Stopp-Limit-Order
        private int delta = 20;                  // Differenz zwischen Stopp- ind Limit-Preis bei Stopp-Limit-Order in Promille vom Stopp-Preis
        private bool _softstopp = true;         // Stopp als Softstopp Bar by Bar
        private int _toleranz = 2;              // Toleranz in Tick bei InsideBars
       // private Test2Plot _Test2Plot = null;

        #endregion

        protected override void OnInit()
		{
			CalculateOnClosedBar = true;
            RequiredBarsCount = 3;
            IsAutomated = _automatisch;
            Abstand = _abstand;
            Teilverkauf = _teilverkauf;
            SoftStopp = _softstopp;

        }

        protected override void OnStart()
        {
            base.OnStart();
          //  _Test2Plot = new Test2Plot();
            Print("b");
        }

        protected override void OnOrderExecution(IExecution execution)
        {

            if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
            {
                if (oStop != null && execution.Name == oStop.Name)
                {
                    if (_sendMail && Core.PreferenceManager.DefaultEmailAddress != "") this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.",
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt. Profit:" + (Trade.ClosedProfitLoss - execution.Commission).ToString("F2"));
                    
                }
            }
        }
		protected override void OnCalculate()
		{
            if (IsProcessingBarIndexLast)
            {
                #region Stopp-Order filled
                if ((TradeInfo != null && Trade.Quantity == 0 && oStop != null ) || (oStop != null && oStop.OrderState == OrderState.Filled))
                {
                    oStop = null;
                    Stopp = 0;
                }
                #endregion Stopp-Order-filled

                #region vorhandene Order holen
                if (Orders.Count > 0)
            {
                int i = 0;
                do
                {
                    if (Orders[i].Action == OrderAction.Sell && (Orders[i].OrderType == OrderType.Stop || Orders[i].OrderType == OrderType.StopLimit))
                    {
                        oStop = Orders[i];
                        if (oStop.OrderType == OrderType.Stop) _stopLimit = false;
                        else _stopLimit = true;
                        //if (oStop.OrderState != OrderState.Filled)
                            //Stopp = Orders[i].StopPrice + _abstand * TickSize;
                    }
                    ++i;
                } while (i < Orders.Count);
            }
            #endregion

                #region Stueck
            if (Trade == null)
            {
                AddChartTextFixed("MyText", " kein Trade offen ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                return;
            }
            if (Trade.Quantity * Trade.AvgPrice < 8000)     // verkaufe alles
            {
                Stueck = Trade.Quantity;
            }
            else if (Trade.Quantity * Trade.AvgPrice < 12000)   // verkaufe alles oder die Hälfte
            {
                if (_teilverkauf > 0 )
                    Stueck = (int)(Trade.Quantity / 2);
                else
                    Stueck = Trade.Quantity;
            }
            else if (_teilverkauf * 4000 < (int)(Trade.Quantity * Trade.AvgPrice))        // verkaufe alles oder ein Teilbetrag >= 4000 €
            {
                Stueck = (int)(Trade.Quantity/_teilverkauf);
            }
                
                else
            {
                    Stueck = 1 + (int)(Trade.Quantity * Trade.AvgPrice) / 4000; // verkaufe einen Teilbetrag mit ca. 4.000 €
            }
            if (oStop != null && oStop.OrderState == OrderState.Filled)
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
                    if (oStop != null && oStop.StopPrice > Stopp)
                    {
                        if ((!_profitOnly || (_profitOnly && (Stopp > ((1 + _profit/1000)* Trade.AvgPrice + _abstand * TickSize)))))    // neuer Stopp auch im Gewinn
                        {
                            ReplaceOrder(oStop, Stueck, Stopp - (delta) * TickSize, Stopp);
                            Print("Stop-Preis: " + oStop.Price + " Limit: " + oStop.LimitPrice);
                        }
                        else
                        { 
                            CancelOrder(oStop);
                            oStop = null;
                        }
                    }
                }
                else Stopp = Instrument.Round2TickSize(Math.Max(Stopp, (Low[1] - _abstand * TickSize)));

                #endregion Stopp_Berechnung
                
                #region Hardstopp_Berechnung

                // 1 Hardstopp setzen wenn Position Stopp über Profit liegt
                if ((oStop == null || (oStop != null && oStop.OrderState == OrderState.Cancelled)) && 
                    (!_profitOnly || (_profitOnly && (Stopp > ((1 + _profit/1000)* Trade.AvgPrice + _abstand* TickSize)))))  
                {
                    if (_softstopp) Stopp = Instrument.Round2TickSize(( 1 + _profit/1000)* Trade.AvgPrice + _abstand * TickSize);
                    if (_stopLimit)
                        oStop = SubmitOrder(0, OrderAction.Sell, OrderType.StopLimit, Stueck, Stopp - (_abstand + delta) * TickSize, Stopp - _abstand * TickSize, "Stopp B", BStopp);
                    else
                        oStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, Stueck, 0, Stopp - _abstand * TickSize, "Stopp B", BStopp);
                    if (_automatisch) oStop.ConfirmOrder();
                }


                // Hardstopp auf Low[1] nachsetzen und damit aktivieren
                if (!_softstopp 
                    || ((oStop != null) && _softstopp && Close[0] < Stopp - _abstand * TickSize))
                { 
                    if ( oStop.OrderState != OrderState.Filled && oStop.OrderState != OrderState.PendingSubmit &&
                         oStop.OrderState != OrderState.PendingReplace && oStop.OrderState != OrderState.PartFilled)
                    {
                        ReplaceOrder(oStop, Stueck, Math.Max(oStop.StopPrice, (Low[1] - (_abstand + (int)(delta * Close[1] / 1000)) * TickSize)), Stopp - _abstand * TickSize);
                    }
                }
                #endregion Hardstopp_Berechnung
            }
            if (oStop != null) // || (oStop != null && oStop.OrderState != OrderState.Cancelled))
            {
             //   _Test2Plot.Zeichne(Stopp, oStop.StopPrice);
                Print(Instrument.Symbol + " Bar: " + ProcessingBarIndex + " Stueck: " + Stueck + " Stopp: " + Stopp + " o-Stop-Preis: " + oStop.Price + " Limit:" + oStop.LimitPrice);
                if (Chart != null) AddChartTextFixed("MyText", "Bewegunsstopp für " + Stueck.ToString("F0") + " Stück  Soft-Stopp: " + Stopp.ToString("F2") +
                    " = " +((Stopp - Trade.AvgPrice)*Stueck).ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
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
