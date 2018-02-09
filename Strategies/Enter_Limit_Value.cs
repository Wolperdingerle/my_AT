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
    [Description("Limit-Vorschlags-Enter-Order mit festgelegtem Kapital und Einstiegspreis, die Stückzahl wird automatisch ermittelt und aufgerundet. Der Einstiegspreis kann im Chart auch nach der Aktievierung verschoben werden. Die Stückzahl wird auch dann noch angepasst.")]
    public class Enter_Limit_Value : UserStrategy
    {
        #region Variables

        private double _kapital = 5000;
        private string MyText = null;
        private IOrder EnterOrder = null;
        private string orderName = "man Limit";
        private bool _sendMail = true;
        private bool _automatisch = false;
        private double LimitPreis = 0;

        #endregion

        protected override void OnInit()
        {
            CalculateOnClosedBar = false;
            RequiredBarsCount = 2;
            IsAutoConfirmOrder = _automatisch;
        }
        protected override void OnStart()
        {

            // Limit-Order vorhanden??
            if (Chart != null) AddChartTextFixed("MyText", "Limit-Kauf für ca. " + _kapital.ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            if (Orders.Count > 0 && EnterOrder == null)
            {
                int i = 0;
                do
                {
                    if (Orders[i].Direction == OrderDirection.Buy && Orders[i].OrderState != OrderState.Filled && Orders[i].OrderType == OrderType.Limit)
                    {
                        EnterOrder = Orders[i];
                       // _kapital = EnterOrder.Quantity * EnterOrder.LimitPrice;
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
            if (Order.Direction == OrderDirection.Buy && Order.OrderState != OrderState.PendingReplace && Order.OrderState != OrderState.PartFilled && Order.OrderState != OrderState.PendingSubmit
                && Order.OrderState != OrderState.PendingCancel && Order.OrderState != OrderState.Filled)
            {
                if(Order.LimitPrice > 0 && Order.Quantity != (int)(_kapital / Order.LimitPrice) + 1)
                {
                    ReplaceOrder(Order, (int)(_kapital / Order.LimitPrice) + 1, Instrument.Round2TickSize(Order.LimitPrice) , 0);
                }
                if (Chart != null) AddChartTextFixed("MyText", "Limit-Kauf für ca. " + (EnterOrder.Quantity * EnterOrder.LimitPrice).ToString("F0") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);

            }


        }


        protected override void OnCalculate()
        {

            if (!IsProcessingBarIndexLast || (EnterOrder != null && (EnterOrder.OrderState == OrderState.Cancelled || EnterOrder.OrderState == OrderState.Filled)))
                return;
            
            if (_kapital > 2 * Account.CashValue)    // = freises Kapital;
            {
                Log(this.Instrument.Name + ": kein ausreichendes, freies Kabital vorhanden!", InfoLogLevel.AlertLog);
                if (EnterOrder != null && EnterOrder.OrderState != OrderState.PartFilled && EnterOrder.OrderState != OrderState.PendingCancel && EnterOrder.OrderState != OrderState.PendingReplace)
                    EnterOrder.CancelOrder();
                return;
            }

            if (EnterOrder == null)
            {
                LimitPreis = Instrument.Round2TickSize(Close[0] *0.985); // Limitpries 1,5% unter letztem Kurs10; // Limitpries 1,5% unter letztem Kurs
                EnterOrder = SubmitOrder(0, OrderDirection.Buy, OrderType.Limit, (int)((_kapital / LimitPreis) + 1), LimitPreis, 0, orderName, orderName);
            }
            else
                if (EnterOrder.OrderState == OrderState.Filled) return;
            
            if (EnterOrder != null && EnterOrder.OrderState == OrderState.Cancelled)
            {
                EnterOrder = null;
                if (Chart != null) AddChartTextFixed("MyText", "Order gelöscht", TextPosition.BottomLeft, Color.Red, new Font("Areal", 14), Color.Blue, Color.Empty, 10);
            }
        }
        

        #region Properties

        [Description("Kapital für diese Order")]
		[InputParameter]
		public double Kapital
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
