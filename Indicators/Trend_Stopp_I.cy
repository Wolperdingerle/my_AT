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

    /// <summary>
    /// Class which holds all important data like the OrderDirection. 
    /// We use this object as a global default return object for the calculate method in indicators.
    /// </summary>
    public class ResultValue_Indicator_Trend_Stopp
    {
        public bool ErrorOccured = false;
       // public OrderDirection? Entry = null;
       // public OrderDirection? Exit = null;
        public bool _StoppLimit = true;
        public bool _ProfitOnly = true;
        public int _Trend = 2;
        public int _Abstand = 2;
        public double _HStopp = 0.0;
     //   public double _SStopp = 0.0;
        public double _Limit = 0.0;
    }

    public class Trend_Stopp_I : UserIndicator
    {
        #region Variable
        //input
        private Color _plot0color = Color.Red;
        private int _plot0width = 2;
        private DashStyle _plot0dashstyle = DashStyle.Solid;
        private Color _plot1color = Color.OrangeRed;
        private int _plot1width = 2;
        private DashStyle _plot1dashstyle = DashStyle.Dash;
        private Color _plot2color = Color.Blue;
        private int _plot2width = 2;
        private DashStyle _plot2dashstyle = DashStyle.DashDot;
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;
     
        private double _Limit = 0.0;
        

        //output

        private double Stopp = 0.0;
        private int _trend = 2;
        private int _abstand = 2;
        private double _limit = 0.0;
        private IDataSeries Hard_Stopp;
        private IDataSeries Soft_Stopp;
        private IDataSeries Limit;
        private int Teilverkauf = 0;
        #endregion Variable

        protected override void OnInit()
        {
            //Define the plots and its color which is displayed underneath the chart
            Add(new OutputDescriptor(this.Plot0Color, "Hard_Stopp"));
            Add(new OutputDescriptor(this.Plot1Color, "Soft_Stopp"));
            Add(new OutputDescriptor(this.Plot2Color, "Limit"));

            // AddOutput(new OutputDescriptor(Color.FromKnownColor(KnownColor.DarkRed), OutputSerieDrawStyle.TriangleDown, "Soft_Stopp"));
            IsOverlay = true;
            //Hard_Stopp = new DataSeries(this);
            //Soft_Stopp = new DataSeries(this);
            //Limit = new DataSeries(this);
        }


        protected override void OnCalculate()
        {

            //Check if peridocity is valid for this script
            if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": Periodicity of your data feed is suboptimal for this indicator!", InfoLogLevel.AlertLog);
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue_Indicator_Trend_Stopp returnvalue = this.calculate(this.InSeries, this._trend, this._abstand, this._Hard_Stopp[0] );

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                Log(this.DisplayName + ": A problem has occured during the calculation method!", InfoLogLevel.AlertLog);
                return;
            }


            if (TradeInfo == null)
                return;
            else
            {
                //if (Teilverkauf == 0) Teilverkauf = TradeInfo.Quantity;
                // #region Trend-Stopp-Berechnung: 

                if (P123Adv(_trend).ValidP3Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).ValidP3DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    Stopp = Math.Max(Stopp, (P123Adv(_trend).ValidP3Price[0] - _abstand * TickSize));       //im Aufwärtstrend: P3 gültig
                else if (P123Adv(_trend).ValidP3Price[0] < 1 && P123Adv(_trend).P2Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).P2DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    Stopp = Math.Max(Stopp, (P123Adv(_trend).P1Price[0]) - _abstand * TickSize);            // im Aufwärtsttrend, noch kein P3 vorhanden,  Stopp am P1
                else if (P123Adv(_trend).P2Price[0] < P123Adv(_trend).P1Price[0] && P123Adv(_trend).TempP3DateTime[0] > P123Adv(_trend).P2DateTime[0]
                       && P123Adv(_trend).P2DateTime[0] < P123Adv(_trend).TempP3DateTime[0])
                    Stopp = Math.Max(Stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize);           //im Abwärtstrend: TempP3 gältig, Stopp am letztenP2

                //if (Teilverkauf > 0 && Teilverkauf != TradeInfo.Quantity)
                //{
                  //  Stopp = Instrument.Round2TickSize((Close[0] - TradeInfo.AvgPrice) / 2 + TradeInfo.AvgPrice);
                  //  Teilverkauf = TradeInfo.Quantity;
                //}
                else Stopp = Instrument.Round2TickSize(Stopp);
                if (Stopp > 0)
                {
                    Outputs[0][0] = Stopp;  // Hard-Stopp
                    //if(_Limit > 0)
                        Outputs[1][0] = Stopp -0.5;   // Soft_Stopp
                    //if(_Limit > 0)
                        Outputs[2][0] = Stopp - 1 ; // Limit
                }
                //   #endregion Trend-Stopp-Berechnung
            }

            #region Ploteigenschaften
            //Set the drawing style, if the user has changed it.
            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;
            PlotColors[0][1] = this.Plot1Color;
            OutputDescriptors[1].PenStyle = this.Dash1Style;
            OutputDescriptors[1].Pen.Width = this.Plot1Width;
            PlotColors[0][2] = this.Plot1Color;
            OutputDescriptors[2].PenStyle = Dash2Style;
            OutputDescriptors[2].Pen.Width = this.Plot2Width;
            #endregion Ploteigenschaften
        }

        /// <summary>
        /// In this method we do all the work and return the object with all data like the OrderActions.
        /// This method can be called from any other script like strategies, indicators or conditions.
        /// Aufruf mit meinen Parametern
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultValue_Indicator_Trend_Stopp calculate(IDataSeries data, int _Trend, int _Abstand, double _HStopp, double _SStopp, double _Limit)
        {
            //Create a return object
            ResultValue_Indicator_Trend_Stopp returnvalue = new ResultValue_Indicator_Trend_Stopp();

            //try catch block with all calculations
            try
            {
                //Calculate SMA and set the data into the result object
                returnvalue._HStopp = Outputs[0][0];
                returnvalue._SStopp = Outputs[1][0];
               // returnvalue._Limit = Outputs[2][0];

                /*
                 * CrossAbove: We create buy (entry) signal for the long position and BuyToCover (exit) for the short position.
                 * CrossBelow: We create sell (exit) signal for the long positon and SellShort (entry) for the short position.
                 */
                 /*
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
                */
            }
            catch (Exception)
            {
                //If this method is called via a strategy or a condition we need to log the error.
                returnvalue.ErrorOccured = true;
            }

            //return the result object
            return returnvalue;
        }


        #region Properties
        #region Input

        [Description("Trendgrösse (0 -3)")]
        [Category("Parameters")]
        public int Trend
        {
            get { return _trend; }
            set { _trend = Math.Min(3, Math.Max(value, 0)); }
        }

/*
        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Soft_Stopp
        {
            get { return Outputs[1]; }
        }
*/

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Trend_Stopp (I)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Trend_Stopp (I)";
            }
        }


        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Day || tf.Periodicity == DatafeedHistoryPeriodicity.Hour || tf.Periodicity == DatafeedHistoryPeriodicity.Minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region Properties



        #region InSeries

      
        /// <summary>
        /// </summary>
        [Description("Select Color für Hard-Stopp.")]
        [Category("Plots")]
        [DisplayName("Color Hard-Stopp")]
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
        [Description("Line für Hard-Stopp.")]
        [Category("Plots")]
        [DisplayName("Line für Hard-Stopp")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle für Hard-Stopp.")]
        [Category("Plots")]
        [DisplayName("DashStyle Hard-Stopp")]
        public DashStyle Dash0Style
        {
            get { return _plot0dashstyle; }
            set { _plot0dashstyle = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Select Color für Soft-Stopp.")]
        [Category("Plots")]
        [DisplayName("Color Soft-Stopp")]
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
        [Description("Line width für Soft-Stopp.")]
        [Category("Plots")]
        [DisplayName("Line Soft-Stopp")]
        public int Plot1Width
        {
            get { return _plot1width; }
            set { _plot1width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle für Soft-Stopp.")]
        [Category("Plots")]
        [DisplayName("DashStyle Soft-Stopp")]
        public DashStyle Dash1Style
        {
            get { return _plot1dashstyle; }
            set { _plot1dashstyle = value; }
        }


        [Description("Select Color für Limit.")]
        [Category("Plots")]
        [DisplayName("Color Limit")]
        public Color Plot2Color
        {
            get { return _plot2color; }
            set { _plot2color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot2ColorSerialize
        {
            get { return SerializableColor.ToString(_plot2color); }
            set { _plot2color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Linenbreite für Limit.")]
        [Category("Plots")]
        [DisplayName("Linenbreite Limit")]
        public int Plot2Width
        {
            get { return _plot1width; }
            set { _plot2width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle für Limit Linie.")]
        [Category("Plots")]
        [DisplayName("DashStyle Limit")]
        public DashStyle Dash2Style
        {
            get { return _plot2dashstyle; }
            set { _plot2dashstyle = value; }
        }

        #endregion


        #region Output

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries _Hard_Stopp
        {
            get { return Outputs[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries _Soft_Stopp
        {
            get { return Outputs[1]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indikator_Limit
        {
            get { return Outputs[2]; }
        }

        #endregion Output


        #endregion

        #endregion



        #endregion
    }
}