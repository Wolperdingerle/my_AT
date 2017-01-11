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

namespace AgenaTrader.UserCode
{
    [Description("Limit-Vorschlags-Enter-Order mit festgelegtem Kapital und Einstiegspreis, die Stückzahl wird automatisch ermittelt und aufgerundet.")]
    public class Enter_Limit_Value : UserStrategy
    {
        #region Variables

        private int _kapital = 5000;
        private string MyText = null;
        private IOrder EnterOrder = null;
        private string orderName = "man Limit";
        private bool _sendMail = true;
        private bool _automatisch = false;
        private double Wert = 0;
        private int quantity = 0;
        private double LimitPreis = 0;

        #endregion

        protected override void OnInit()
        {
            CalculateOnClosedBar = false;
            RequiredBarsCount = 20;
            IsAutomated = _automatisch;

        }
        protected override void OnStart()
        {

            // Limit-Order vorhanden??
            if (Chart != null) AddChartTextFixed("MyText", "Limit-Kauf für ca. " + _kapital.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            //if (Core.PreferenceManager.IsAtrEntryDistance) _abstand = (int)Math.Max(_abstand, ATR(14)[1] * Core.PreferenceManager.AtrEntryDistanceFactor);
            Wert = _kapital;
            if (Orders.Count > 0)
            {
                int i = 0;
                do
                {
                    if (Orders[i].Action == OrderAction.Buy && Orders[i].OrderState != OrderState.Filled && Orders[i].OrderType == OrderType.Limit)
                    {
                        EnterOrder = Orders[i];
                        Wert = EnterOrder.Quantity * EnterOrder.LimitPrice;
                    }
                    ++i;
                } while (i < Orders.Count);
            }
        }
    
        protected override void OnOrderExecution(IExecution execution)
        {

            if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
            {
                if (EnterOrder != null && execution.Name == EnterOrder.Name)
                {
                    if (_sendMail && Core.PreferenceManager.DefaultEmailAddress != "") this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.",
                         execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt. Ausführungspreis:" + (execution.Price).ToString("F2"));
                }
            }
        }

        protected override void OnOrderChanged( IOrder Order)
        {
            if (Order.Action == OrderAction.Buy)
            {
                
                ReplaceOrder(Order, (int)(quantity/ Order.LimitPrice), Order.LimitPrice, Order.StopPrice);
            }

                
        }


        protected override void OnCalculate()
        {
          /*  if (Chart != null)
            {
                if (EnterOrder == null)
                    AddChartTextFixed("MyText", "Limit-Kauf für ca. " + _kapital.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                else if (EnterOrder != null && EnterOrder.OrderState == OrderState.Cancelled)
                    AddChartTextFixed("MyText", "Order abgebrochen ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                else if (EnterOrder != null && EnterOrder.OrderState == OrderState.Filled)
                    AddChartTextFixed("MyText", "Kauf ausgeführt ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                else
                    AddChartTextFixed("MyText", "Limit-Kauf für ca. " + Wert.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            }
            */
            if ((Trade != null && Trade.Quantity * Trade.AvgPrice >= _kapital * 0.95) || (EnterOrder != null && (EnterOrder.OrderState == OrderState.Cancelled || EnterOrder.OrderState == OrderState.Filled)))
                return;
            if (!IsProcessingBarIndexLast) return;

            if (Wert > 2 * Account.CashValue)    // = freises Kapital;
            {
                Log(this.Instrument.Name + ": kein ausreichendes, freies Kabital vorhanden!", InfoLogLevel.AlertLog);
                return;
            }

            if (EnterOrder == null)
            {
                LimitPreis = Instrument.Round2TickSize(Close[0] *0.985); // Limitpries 1,5% unter letztem Kurs
                quantity = (int)((_kapital / LimitPreis) + 1);
                EnterOrder = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, quantity, LimitPreis, 0, "", orderName);
                Wert = EnterOrder.Quantity * EnterOrder.LimitPrice;
                
            }
            else
            {

                if (EnterOrder.OrderState == OrderState.Filled)
                {
                    return;
                }
                if(EnterOrder != null && EnterOrder.OrderState != OrderState.Filled && EnterOrder.OrderState != OrderState.PendingReplace && EnterOrder.OrderState != OrderState.PendingSubmit) 
                {
                    // Neuberechnung Quantity
                    if(EnterOrder.Quantity != (int)(((_kapital / EnterOrder.LimitPrice) + 1)))
                    {
                    // Neuberechnung der Stückzahl mit aktuellem Preis
                    quantity = (int)((_kapital / EnterOrder.LimitPrice) + 1);
                    ReplaceOrder(EnterOrder, quantity, EnterOrder.LimitPrice, 0);
                    Wert = EnterOrder.Quantity * EnterOrder.LimitPrice;
                    if (Chart != null) AddChartTextFixed("MyText", "Limit-Kauf für ca. " + Wert.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
                    }
                }
            }
            if (EnterOrder.OrderState == OrderState.Cancelled)
            {
                //EnterOrder = null;
                if (Chart != null) AddChartTextFixed("MyText", "Order gelöscht", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            }
        }

            



        #region Properties

        [Description("Kapital für diese Order")]
		[Category("Parameters")]
		public int Kapital
		{
			get { return _kapital; }
			set { _kapital = Math.Max(4000, value); }
		}

        [Description("Automatisch")]
        [Category("Strategy")]
        public bool Automatisch
        {
            get { return _automatisch; }
            set { _automatisch = value; }
        }

        [Description("SendMail")]
        [Category("Strategy")]

        public bool SendMail
        {
            get { return _sendMail; }
            set { _sendMail = value; }
        }
        #endregion
    }
}
