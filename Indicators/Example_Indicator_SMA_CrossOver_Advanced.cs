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
/// This indicator provides entry and exit signals for a SMA crossover.
/// Long  Signal when fast SMA crosses slow SMA above. OutputDescriptor is set to  1.
/// Short Signal wenn fast SMA crosses slow SMA below. OutputDescriptor is set to -1.
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    /// <summary>
    /// Class which holds all important data like the OrderDirection. 
    /// We use this object as a global default return object for the calculate method in indicators.
    /// </summary>
    public class ResultValue_Example_Indicator_SMA_CrossOver_Advanced
    {
        public bool ErrorOccured = false;
        public OrderDirection? Entry = null;
        public OrderDirection? Exit = null;
        public double Price = 0.0;
        public double Slow = 0.0;
        public double Fast = 0.0;
    }

    [Description("Basic indicator example for SMA crossover")]
    public class Example_Indicator_SMA_CrossOver_Advanced : UserIndicator
    {
        #region Variable
        //input
        private Color _plot0color = Color.Blue;
        private int _plot0width = 2;
        private DashStyle _plot0dashstyle = DashStyle.Solid;
        private Color _plot1color = Color.Red;
        private int _plot1width = 2;
        private DashStyle _plot1dashstyle = DashStyle.Solid;
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;
        private int _fastsma = 20;
        private int _slowsma = 50;
        private int _trend = 1;
        private int _abstand = 2;


        //output

        //internal
        #endregion Variable

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void OnInit()
        {
            //Define the plots and its color which is displayed underneath the chart
            Add(new OutputDescriptor(this.Plot0Color, "Hard-Stopp"));
            Add(new OutputDescriptor(this.Plot1Color, "Soft-Stopp"));

            //Define if the OnBarUpdate method should be triggered only on BarClose (=end of period)
            //or with each price update
            CalculateOnClosedBar = true;

            //Indicator should be drawn on the price panel
            this.IsOverlay = true;

            //Because of backtesting reasons if we use the advanced mode we need at least two bars!
            //In this case we are using SMA50, so we need at least 50 bars.
            this.RequiredBarsCount = 50;
        }


        /// <summary>
        /// Called on each update of the bar.
        /// </summary>
        protected override void OnCalculate()
        {

            //Check if peridocity is valid for this script
            if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": Periodicity of your data feed is suboptimal for this indicator!", InfoLogLevel.AlertLog);
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = this.calculate(this.InSeries, this.FastSma, this.SlowSma, this.IsLongEnabled, this.IsShortEnabled);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                Log(this.DisplayName + ": A problem has occured during the calculation method!", InfoLogLevel.AlertLog);
                return;
            }

            //Set the curve data for the chart drawing
            this.Hard_Stopp.Set(returnvalue.Fast);
            this.Soft_Stopp.Set(returnvalue.Slow);

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderDirection.Buy:
                        AddChartDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Orange);
                        break;
                    case OrderDirection.Sell:
                        AddChartDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Orange);
                        break;
                }
            }


            ////Exit
            //if (returnvalue.Exit.HasValue)
            //{
            //    switch (returnvalue.Exit)
            //    {
            //        case OrderDirection.Buy:
            //            DrawDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Orange);
            //            break;
            //        case OrderDirection.Sell:
            //            DrawDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Orange);
            //            break;
            //    }
            //}


            //Set the drawing style, if the user has changed it.
            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;
            PlotColors[1][0] = this.Plot1Color;
            OutputDescriptors[1].PenStyle = this.Dash0Style;
            OutputDescriptors[1].Pen.Width = this.Plot0Width;
        }


        /// <summary>
        /// In this method we do all the work and return the object with all data like the OrderActions.
        /// This method can be called from any other script like strategies, indicators or conditions.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultValue_Example_Indicator_SMA_CrossOver_Advanced calculate(IDataSeries data, int fastsma, int slowsma, bool islongenabled, bool isshortenabled)
        {
            //Create a return object
            ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = new ResultValue_Example_Indicator_SMA_CrossOver_Advanced();

            //try catch block with all calculations
            try
            {
                //Calculate SMA and set the data into the result object
                returnvalue.Fast = SMA(data, fastsma)[0];
                returnvalue.Slow = SMA(data, slowsma)[0];
                returnvalue.Price = data.Last();

                /*
                 * CrossAbove: We create buy (entry) signal for the long position and BuyToCover (exit) for the short position.
                 * CrossBelow: We create sell (exit) signal for the long positon and SellShort (entry) for the short position.
                 */
                if (CrossAbove(SMA(data, fastsma), SMA(data, slowsma), 0) == true)
                {
                    if (islongenabled)
                    {
                        returnvalue.Entry = OrderDirection.Buy;
                    }
                    if (isshortenabled)
                    {
                        returnvalue.Exit = OrderDirection.Buy;
                    }
                   
                }
                else if (CrossBelow(SMA(data, fastsma), SMA(data, slowsma), 0) == true)
                {
                    if (islongenabled)
                    {
                        returnvalue.Exit = OrderDirection.Sell;
                    }
                    if (isshortenabled)
                    {
                        returnvalue.Entry = OrderDirection.Sell;
                    }
                }

            }
            catch (Exception)
            {
                //If this method is called via a strategy or a condition we need to log the error.
                returnvalue.ErrorOccured = true;
            }

            //return the result object
            return returnvalue;
        }




        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Advanced (I)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Advanced (I)";
            }
        }


        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Day ||  tf.Periodicity == DatafeedHistoryPeriodicity.Hour || tf.Periodicity == DatafeedHistoryPeriodicity.Minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region Properties



        #region Input


        /// <summary>
        /// </summary>
        [Description("The period of the fast SMA indicator.")]
        [Category("Parameters")]
        [DisplayName("Period fast")]
        public int FastSma
        {
            get { return _fastsma; }
            set { _fastsma = value; }
        }


        /// <summary>
        /// </summary>
        [Description("The period of the slow SMA indicator.")]
        [Category("Parameters")]
        [DisplayName("Period slow")]
        public int SlowSma
        {
            get { return _slowsma; }
            set { _slowsma = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create long positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create short positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Select Color for the long indicator.")]
            [Category("Plots")]
            [DisplayName("Color long")]
            public Color Plot0Color
            {
                get { return _plot0color; }
                set { _plot0color = value; }
            }
            // Serialize Color object
            [Browsable(false)]
            public string Plot0ColorSerialize
            {
                get { return SerializableColor.ToString(_plot0color); }
                set { _plot0color = SerializableColor.FromString(value); }
            }

            /// <summary>
            /// </summary>
            [Description("Line width for long indicator.")]
            [Category("Plots")]
            [DisplayName("Line width long")]
            public int Plot0Width
            {
                get { return _plot0width; }
                set { _plot0width = Math.Max(1, value); }
            }

            /// <summary>
            /// </summary>
            [Description("DashStyle for long indicator.")]
            [Category("Plots")]
            [DisplayName("DashStyle long")]
            public DashStyle Dash0Style
            {
                get { return _plot0dashstyle; }
                set { _plot0dashstyle = value; }
            }


        /// <summary>
        /// </summary>
        [Description("Select Color for the short indicator.")]
        [Category("Plots")]
        [DisplayName("Color short")]
        public Color Plot1Color
        {
            get { return _plot1color; }
            set { _plot1color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot1ColorSerialize
        {
            get { return SerializableColor.ToString(_plot1color); }
            set { _plot1color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Line width for short indicator.")]
        [Category("Plots")]
        [DisplayName("Line width short")]
        public int Plot1Width
        {
            get { return _plot1width; }
            set { _plot1width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for short indicator.")]
        [Category("Plots")]
        [DisplayName("DashStyle short")]
        public DashStyle Dash1Style
        {
            get { return _plot1dashstyle; }
            set { _plot1dashstyle = value; }
        }

        #endregion


        #region Output

        [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Hard_Stopp
            {
                get { return Outputs[0]; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Soft_Stopp
            {
                get { return Outputs[1]; }
            }

            #endregion




        #endregion

    }
}