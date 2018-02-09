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
/// Änderung: Order wird gelöscht, ween bei aktiver Order das Kapital nicht mehr ausreicht
/// </summary>

namespace AgenaTrader.UserCode
{
	[Description("Enter mit Trailing Stopp Limit Order Bar by Bar Mover mit einstellbarem Volumen.")]
	public class Enter_Trail_Stop_Vol : UserStrategy
	{

        /* Startprogrammierung: nach Wahl des Kapitals wird eine Stop-Limit-Order (Stop-Preis = Limit-Preis) über das letzte High gelegt
         * 
         */
    #region Variables

	    private int _kapital = 5000;// vorschschlagenes Kapital
        private int _abstand = 2;
        private int _delta = 5;
        private bool _automatisch = true;
        private bool _sendMail = true;
        private IOrder EnterOrder = null;
        private IOrder LimitOrder = null;
        private string orderName = "Vol_Peek_Stp";
        private double Wert = 0;
        private int quantity = 0;
        private double StopPreis = 0;
        private int _barBack = 1;        // BarBack = 1 = letzter Bar, Bar[0] = laufender bar, nicht abgeschlossen
        private bool _stopLimit = false;


        #endregion

        protected override void OnInit()
		{
			CalculateOnClosedBar = true;
            Kapital = _kapital;
            IsAutoConfirmOrder = _automatisch;
            BarBack = _barBack;
            StopLimit = _stopLimit;
            RequiredBarsCount = _barBack +1;
        }

        protected override void OnStart()
        {

            // Limit-Order vorhanden??
            if (Chart != null) AddChartTextFixed("MyText", "Stop-Limit-Kauf für ca. " + _kapital.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            if (Core.PreferenceManager.IsAtrEntryDistance) _abstand = (int)Math.Max(_abstand, ATR(14)[1] * Core.PreferenceManager.AtrEntryDistanceFactor);
            Wert = _kapital;
         }

        protected override void OnOrderExecution(IExecution execution)
        {

            if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
            {
                if (EnterOrder != null && execution.Order.OrderType == EnterOrder.OrderType)
                {
                    if (_sendMail && Core.PreferenceManager.DefaultEmailAddress != "") this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                         execution.Instrument.Symbol + " Kauf-Order " + execution.Name + " ausgeführt.",
                         execution.Instrument.Symbol + " Kauf-Order " + execution.Name +" mit " + execution.Quantity + " Stück ausgeführt. Ausführungspreis: " + (execution.Price).ToString("F2"));
                    if (LimitOrder != null) LimitOrder.CancelOrder();
                }
                if(LimitOrder != null && execution.Order.OrderType == LimitOrder.OrderType) if (EnterOrder != null) EnterOrder.CancelOrder();
            }
        }

        protected override void OnCalculate()
		{
            if (!IsProcessingBarIndexLast 
                || (EnterOrder != null && (EnterOrder.OrderState == OrderState.Cancelled || EnterOrder.OrderState == OrderState.Filled))) return;
            
            if (Orders.Count > 0)   // suche nach vorhandener Stop-Limit-Order
            {
                int i = 0;
                do
                {
                    if (Orders[i].Direction == OrderDirection.Buy && Orders[i].OrderState != OrderState.Filled)
                    {
                        if (_stopLimit && Orders[i].OrderType == OrderType.StopLimit)
                            EnterOrder = Orders[i];
                        else if (!_stopLimit && Orders[i].OrderType == OrderType.Stop)
                            EnterOrder = Orders[i];
                        else
                            LimitOrder = Orders[i]; // da liegt zusätzlich eine Limit-Buy-Order
                        if (EnterOrder != null)
                        { 
                            StopPreis = EnterOrder.StopPrice;
                            Wert = EnterOrder.Quantity * EnterOrder.StopPrice;
                        }
                    }
                    ++i;
                } while (i < Orders.Count);
            }

            if (Wert > 2 * Account.CashValue)    // = freises Kapital;
            {
                Log(this.Instrument.Name + ": kein ausreichendes, freies Kapital vorhanden!", InfoLogLevel.AlertLog);
                if (EnterOrder != null && EnterOrder.OrderState != OrderState.PartFilled && EnterOrder.OrderState != OrderState.PendingCancel && EnterOrder.OrderState != OrderState.PendingReplace)
                    EnterOrder.CancelOrder();
                return;
            }
            if (EnterOrder == null)
            {
                StopPreis =  Instrument.Round2TickSize(HighestHighPrice(_barBack)[0] + _abstand * TickSize); // Limitpreis über das letzte Hoch
                quantity = (int)((_kapital / StopPreis) + 1);
                if(_stopLimit)
                    EnterOrder = SubmitOrder(0, OrderDirection.Buy, OrderType.StopLimit, quantity, StopPreis + _delta * TickSize, StopPreis, "Enter_TS", orderName);
                else
                    EnterOrder = SubmitOrder(0, OrderDirection.Buy, OrderType.Stop, quantity, 0, StopPreis, "Enter_TS", orderName);
                Wert = EnterOrder.Quantity * EnterOrder.StopPrice;

                //if (LimitOrder != null) CreateOCOGroup(new List<IOrder> {EnterOrder, LimitOrder});
                if (_automatisch) EnterOrder.ConfirmOrder();
            }
            else
            {
                if (EnterOrder != null && EnterOrder.OrderState != OrderState.Filled && EnterOrder.OrderState != OrderState.PendingReplace && EnterOrder.OrderState != OrderState.PendingSubmit)
                {
                    // Neuberechnung Quantity und Limitpreise
                    StopPreis = Math.Min(StopPreis, Instrument.Round2TickSize(HighestHighPrice(_barBack)[0] + _abstand * TickSize)); // Limitpreis über das letzte Hoch
                    quantity = (int)((_kapital / StopPreis) + 1);
                    if(EnterOrder.OrderType == OrderType.StopLimit)
                        ReplaceOrder(EnterOrder, quantity, Instrument.Round2TickSize(StopPreis + _delta * TickSize), Instrument.Round2TickSize(StopPreis));
                    else
                        ReplaceOrder(EnterOrder, quantity, 0, Instrument.Round2TickSize(StopPreis));
                    Wert = EnterOrder.Quantity * EnterOrder.StopPrice;
                    if (Chart != null) AddChartTextFixed("MyText", "Stop-Limit-Kauf für ca. " + Wert.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                }
            }
            if (EnterOrder.OrderState == OrderState.Cancelled)
            {
                //EnterOrder = null;
                if (Chart != null) AddChartTextFixed("MyText", "Order gelöscht", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            }
        }


		#region Properties

		[Description("Kapital für Einstieg, min. 4.000 €")]
		[InputParameter]
		public int Kapital
		{
			get { return _kapital; }
			set { _kapital = Math.Max(4000, value); }
		}


        [Description("Tickabstand")]
        [InputParameter]
        public int Abstand
        {
            get { return _abstand; }
            set { _abstand = Math.Max(0, value); }
        }

        [Description("Abstand Stopp - LimitTickabstand in Ticks")]
        [InputParameter]
        public int Delta
        {
            get { return _delta; }
            set { _delta = value; }
        }

        [Description("Bars back für Trailing Stop")]
		[InputParameter]
		public int BarBack
        {
			get { return _barBack; }
			set { _barBack = Math.Max(1, value); }
		}

        [Description("StopLimit-Order")]
        [InputParameter]
        public bool StopLimit
        {
            get { return _stopLimit; }
            set { _stopLimit = value; }
        }

        [Description("SendMail")]
        [InputParameter]

        public bool SendMail
        {
            get { return _sendMail; }
            set { _sendMail = value; }
        }


        [Description("Automatisch")]
        [InputParameter]
        public bool Automatisch
        {
            get { return _automatisch; }
            set { _automatisch = value; }
        }

        #endregion


    }
}
