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
	[Description("Durchschnittlicher, täglicher Marktumsatz")]
	public class MarktUmsatz : UserIndicator
	{
        #region Variables

        private int _period = 50;
       

        #endregion      
           public MarktUmsatz()
        {
            _period = 100;
            RequiredBarsCount = _period;
            IsOverlay = true;
        }
		protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Umsatz"));
			CalculateOnClosedBar = true;
            
		}

		protected override void OnCalculate()
		{
           if(Volume[0] > 0)
			Umsatz.Set(Volume[0]/ SMA(Volume, _period)[0]*100);
			

		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Umsatz
		{
			get { return Outputs[0]; }
		}

		#endregion
	}
}