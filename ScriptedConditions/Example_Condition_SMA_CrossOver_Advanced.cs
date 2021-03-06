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
/// This scripted condition provides entry and exit signals for a SMA crossover.
/// Long  Signal when fast SMA crosses slow SMA above. OutputDescriptor is set to  1.
/// Short Signal wenn fast SMA crosses slow SMA below. OutputDescriptor is set to -1.
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Basic indicator example for SMA crossover")]
    [IsEntryAttribute(true)]
    [IsStopAttribute(false)]
    [IsTargetAttribute(false)]
    [OverrulePreviousStopPrice(false)]
    public class Example_Condition_SMA_CrossOver_Advanced : UserScriptedCondition
    {

        //input
        private Color _plot0color = Color.Orange;
        private int _plot0width = 1;
        private DashStyle _plot0dashstyle = DashStyle.Solid;
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;
        private int _fastsma = 20;
        private int _slowsma = 50;

        //output

        //internal
        private Example_Indicator_SMA_CrossOver_Advanced _Example_Indicator_SMA_CrossOver_Advanced = null;

        private int _myvalue = 1;

        protected override void OnInit()
        {
            //Print("Initialize");
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new OutputDescriptor(this.Plot0Color, "Occurred"));
            Add(new OutputDescriptor(this.Plot0Color, "Entry"));

            //Define if the OnBarUpdate method should be triggered only on BarClose (=end of period)
            //or with each price update
            CalculateOnClosedBar = true;

            //Condition should be drawn on a seperate panel
            this.IsOverlay = false;

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
                        //Long Signal
                        Occurred.Set(1);
                        Entry.Set(1);
                        break;
                    case OrderDirection.Sell:
                        //Short Signal
                        Occurred.Set(-1);
                        Entry.Set(-1);
                        break;
                }
            }
            else
            {
                //No Signal
                Occurred.Set(0);
                Entry.Set(0);
            }

            //Set the drawing style, if the user has changed it.
            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Advanced (C)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Advanced (C)";
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


        #endregion

        #region Output


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Occurred
        {
            get { return Outputs[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Entry
        {
            get { return Outputs[1]; }
        }

        public override IList<DataSeries> GetEntries()
        {
            return new[] { Entry };
        }

        #endregion

        #endregion
    }
}