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
    
  
    [Description("Basic indicator Trend-Stopp")]
    public class Trend_Stopp : UserIndicator
	{
        #region Variable für die Plots
        //input
        //input Eigenschafte der Plots
        // Softstopp
        private Color _plot0color = Color.Blue;
        private int _plot0width = 2;
        private DashStyle _plot0dashstyle = DashStyle.Dash;

        // intern
        private int _trend = 1;
        private int _abstand = 2;
        private int stueck;
        private int _BarsRequired = 500;
        private DateTime startZeit;
        private double _stopp = 0;


        #endregion Variable
        

        protected override void OnInit()
		{
            //Define the plots and its color which is displayed in the chart
            Add(new OutputDescriptor(Plot0Color, "Soft_Stopp"));
            RequiredBarsCount = RequiredBarsCount;
            IsOverlay = true;
            if (TradeInfo != null && TradeInfo.Quantity > 0 && stueck == 0)
            {
                stueck = TradeInfo.Quantity;
                startZeit = TradeInfo.CreatedDateTime;
                _stopp = 0;
            }
            
        }

		protected override void OnCalculate()
		{
            if (TradeInfo == null || (TradeInfo != null && TradeInfo.Quantity == 0 )) return;     // kein Trade offen, nur letzten Bar berechnen
            #region Timeframe
            //Check if peridocity is valid for this script
            if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": Zeitbasierter Chart erforderlich!", InfoLogLevel.AlertLog);
                return;
            }
            #endregion Timeframe
            
            if(TradeInfo.Quantity != stueck)   // Kauf oder Teilverkauf hat stattgefunden
            {
                stueck = TradeInfo.Quantity;
                startZeit = Bars[0].Time;
                _stopp = 0;
            }

            #region Trend-Stopp-Berechnung: Starte ab dem 2. Bar nach Tradeeröffung
            if (Bars[1] != null && Bars[1].Time > startZeit)
            {
                if (P123Adv(_trend).ValidP3Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).ValidP3DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).ValidP3Price[0] - _abstand * TickSize)));       //im Aufwärtstrend: P3 gültig
                else if (P123Adv(_trend).ValidP3Price[0] < 1 && P123Adv(_trend).P2Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).P2DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).P1Price[0]) - _abstand * TickSize));            // im Aufwärtsttrend, noch kein P3 vorhanden,  Stopp am P1
                else if (P123Adv(_trend).P2Price[0] < P123Adv(_trend).P1Price[0] && P123Adv(_trend).TempP3DateTime[0] > P123Adv(_trend).P2DateTime[0]
                    && P123Adv(_trend).P2DateTime[0] < P123Adv(_trend).TempP3DateTime[0])
                    _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize));           //im Abwärstrend: TempP3 gültig, Stopp am letzten P2
               // else if (P123Adv(_trend).P2DateTime[0] < startZeit)
               // _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize));           //im Abwärtstrend: P2 vor Kauf, Stopp am gültigen P2
             
                if (_stopp > 0)
                    Soft_Stopp.Set(_stopp);     // zeigt Verlauf Softstopp
                else
                    Soft_Stopp.Reset();
            }
            #endregion

            #region Ploteinstellungen ändern
            //Set the drawing style, if the user has changed it.
            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;
            
            #endregion
        }
        
        

        #region Properties
        // Beginn Qutput
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Soft_Stopp
		{
			get { return Outputs[0]; }
        }
        [Description("Trend (0 bis 3)")]
        [InputParameter]
		public int Trend
		{
			get { return _trend; }
			set { _trend = Math.Max(0, value); }
		}
		
		[InputParameter]
		public int Abstand
		{
			get { return _abstand; }
			set { _abstand = Math.Max(0, value); }
		}


        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        
        public override string ToString()
        {
            return "Trend_Stopp (I) Trend "+ _trend;
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Trend_Stopp (I) Trend" + _trend;
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

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        [Description("Select Color für Soft-Stopp.")]
        [Category("Plots")]
        [DisplayName("Color Soft-Stopp")]
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
        [Description("Line width für Soft-Stopp.")]
        [Category("Plots")]
        [DisplayName("Line Soft-Stopp")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle für Soft-Stopp.")]
        [Category("Plots")]
        [DisplayName("DashStyle Soft-Stopp")]
        public DashStyle Dash0Style
        {
            get { return _plot0dashstyle; }
            set { _plot0dashstyle = value; }
        }
        
        #endregion
    }
}