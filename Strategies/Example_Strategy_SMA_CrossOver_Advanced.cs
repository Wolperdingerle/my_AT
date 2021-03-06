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
/// Version: 1.2.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This strategy provides provides entry and exit signals for a SMA crossover.
/// Long  Signal when fast SMA crosses slow SMA above. OutputDescriptor is set to  1.
/// Short Signal wenn fast SMA crosses slow SMA below. OutputDescriptor is set to -1.
/// StopLoss is set to 1% and Target is set to 3%
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Basic indicator example for SMA crossover")]
    public class Example_Strategy_SMA_CrossOver_Advanced : UserStrategy
    {

        //input
        private bool _autopilot = true;
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;
        private int _fastsma = 20;
        private int _slowsma = 50;

        //output

        //internal
        private Example_Indicator_SMA_CrossOver_Advanced _Example_Indicator_SMA_CrossOver_Advanced = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

		protected override void OnInit()
		{
            //Define if the OnBarUpdate method should be triggered only on BarClose (=end of period)
            //or with each price update
            CalculateOnClosedBar = true;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set, this will lead to a better usability
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
            }

            //Because of backtesting reasons if we use the advanced mode we need at least two bars!
			//In this case we are using SMA50, so we need at least 50 bars.
            this.RequiredBarsCount = 50;
        }


        protected override void OnStart()
        {
            base.OnStart();

            //Init our indicator to get code access to the calculate method
            this._Example_Indicator_SMA_CrossOver_Advanced = new Example_Indicator_SMA_CrossOver_Advanced();

        }

		protected override void OnCalculate()
		{
            //Set Autopilot
            this.IsAutoConfirmOrder = this.Autopilot;

            //Check if peridocity is valid for this script
            if (!this._Example_Indicator_SMA_CrossOver_Advanced.DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": Periodicity of your data feed is suboptimal for this indicator!", InfoLogLevel.AlertLog);
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = this._Example_Indicator_SMA_CrossOver_Advanced.calculate(this.InSeries, this.FastSma, this.SlowSma, this.IsLongEnabled, this.IsShortEnabled);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                Log(this.DisplayName + ": A problem has occured during the calculation method!", InfoLogLevel.AlertLog);
                return;
            }

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderDirection.Buy:
                        this.DoEnterLong();
                        break;
                    case OrderDirection.Sell:
                        this.DoEnterShort();
                        break;
                }
            }

            //Exit
            if (returnvalue.Exit.HasValue)
            {
                switch (returnvalue.Exit)
                {
                    case OrderDirection.Buy:
                        this.DoExitShort();
                        break;
                    case OrderDirection.Sell:
                        this.DoExitLong();
                        break;
                }
            }
		}


        /// <summary>
        /// Create LONG order.
        /// </summary>
        private void DoEnterLong()
        {
            if (_orderenterlong == null)
            {
                //_orderenterlong = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = this.DefaultOrderQuantity, SignalName =  this.DisplayName + "_" + OrderDirection.Buy + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
                _orderenterlong = SubmitOrder(new StrategyOrderParameters { Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = this.DefaultOrderQuantity, SignalName = this.DisplayName + "_" + OrderDirection.Buy + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.ToString(), Instrument = this.Instrument, TimeFrame = this.TimeFrame });
                //set a stop loss for our order. we set it 1% below the current price
                SetUpStopLoss(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 0.99, false);

                //set a target for our order. we set it 3% above the current price
                SetUpProfitTarget(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 1.05);
            }
        }

        /// <summary>
        /// Create SHORT order.
        /// </summary>
        private void DoEnterShort()
        {
            if (_orderentershort == null)
            {
                //_orderentershort = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = this.DefaultOrderQuantity, SignalName =  this.DisplayName + "_" + OrderDirection.Sell + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
                _orderentershort = SubmitOrder(new StrategyOrderParameters { Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = this.DefaultOrderQuantity, SignalName = this.DisplayName + "_" + OrderDirection.Sell + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), Instrument = this.Instrument, TimeFrame = this.TimeFrame });
                //set a stop loss for our order. we set it 1% above the current price
                SetUpStopLoss(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 1.01, false);

                //set a target for our order. we set it 3% below the current price
                SetUpProfitTarget(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 0.95);
            }
        }

        /// <summary>
        /// Exit the LONG position.
        /// </summary>
        private void DoExitLong()
        {
            if (_orderenterlong != null)
            {
                //SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = this._orderenterlong.Name});
                SubmitOrder(new StrategyOrderParameters { Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = this._orderenterlong.Quantity });
                this._orderenterlong = null;
            }
        }

        /// <summary>
        /// Fill the SHORT position.
        /// </summary>
        private void DoExitShort()
        {
            if (_orderentershort != null)
            {
                //SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = this._orderentershort.Name});
                SubmitOrder(new StrategyOrderParameters { Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = this._orderentershort.Quantity });
                this._orderentershort = null;

            }
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Advanced (S)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Advanced (S)";
            }
        }


        #region Properties


        #region Input

        /// <summary>
        /// </summary>
        [Description("The period of the fast SMA indicator.")]
        [InputParameter]
        [DisplayName("Period fast")]
        public int FastSma
        {
            get { return _fastsma; }
            set { _fastsma = value; }
        }


        /// <summary>
        /// </summary>
        [Description("The period of the slow SMA indicator.")]
        [InputParameter]
        [DisplayName("Period slow")]
        public int SlowSma
        {
            get { return _slowsma; }
            set { _slowsma = value; }
        }

        [Description("If true the strategy will handle everything. It will create buy orders, sell orders, stop loss orders, targets fully automatically")]
        [Category("Safety first!")]
        [DisplayName("Autopilot")]
        public bool Autopilot
        {
            get { return _autopilot; }
            set { _autopilot = value; }
        }

        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create long positions.")]
        [InputParameter]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create short positions.")]
        [InputParameter]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }




        #endregion

        #endregion

    }
}