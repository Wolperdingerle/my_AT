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
    [Description("Verhältnis des Tagesvolumen zum Durchschnittsvolumen der letzten X Tage")]
	public class Vol_Prozent : UserIndicator



	{
		#region Variables
		private int _tage = 14;
        #endregion


        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromArgb(255, 0, 102, 204), "Proz_Vol"));
			CalculateOnClosedBar = true;
            RequiredBarsCount = _tage + 2;
		}

		protected override void OnCalculate()
		{
            Proz_Vol.Set(Volume[0] / SMA(Volume, _tage)[1]);
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Proz_Vol
		{
			get { return Outputs[0]; }
		}

		[Description("Tage für Durchschnittsvolumen")]
		[Category("Parameters")]
		public int Tage
		{
			get { return _tage; }
			set { _tage = Math.Max(1, value); }
		}

		#endregion
	}
}