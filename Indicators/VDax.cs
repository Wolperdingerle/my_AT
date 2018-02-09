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
	public class VDax : UserIndicator
	{
        private Double r1;
        #region Variable für die OutputDescriptors
        //input Eigenschafte der OutputDescriptors
        private Color _plot0color = Color.Blue;
        private int _plot0width = 2;
        private DashStyle _plot0dashstyle = DashStyle.Dash;
        #endregion OutputDescriptor
        
        public VDax()
		{
            _intervall = 60;

		}

		protected override void OnInit()
		{
            CalculateOnClosedBar = true;
			AddOutput(new OutputDescriptor(_plot0color, "VDax new"));
			IsOverlay = true;
           
        }

		protected override void OnCalculate()
		{
            if (IsProcessingBarIndexLast)
            {
                r1 = (double)GlobalUtilities.GetCurrentVdaxNew(_intervall);
                
                // VDax_new.Set(r1);
                if (Chart != null)
                {
                    if (r1 > 0)
                        AddChartTextFixed("MyText", "VDax new " + r1, TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                }
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries VDax_new
		{
			get { return Outputs[0]; }
		}

		[InputParameter]
		public int Intervall
		{
			get { return _intervall; }
			set { _intervall = Math.Max(1, value); }
		}
		private int _intervall;
/*
   
        [Description("Select Color für VDax new")]
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
        [Description("Line width für VDax new.")]
        [Category("Plots")]
        [DisplayName("Line Soft-Stopp")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle f�r VDax new.")]
        [Category("Plots")]
        [DisplayName("DashStyle Soft-Stopp")]
        public DashStyle Dash0Style
        {
            get { return _plot0dashstyle; }
            set { _plot0dashstyle = value; }
        }
*/

        #endregion
    }
}