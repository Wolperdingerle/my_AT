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
	[Description("Kapitaleinsatz des laufenden Trades")]
	public class Kapitaleinsatz : UserIndicator
	{
        private double Kapital;
        //private int Nachkauf;

		protected override void OnInit()
		{
			//Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
			CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{
			
            if (TradeInfo != null)
            {
                if (TradeInfo.PositionType != PositionType.Flat)
                { 
                    Kapital = (TradeInfo.Quantity * TradeInfo.AvgPrice);
                    //Nachkauf = (int)(Math.Max(Kapital * 0.5, 4000) / Close[0]);
                    AddChartTextFixed("MyText", "Kapital: " + Kapital.ToString("F2") + " € ", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
		    //AddChartTextFixed("MyText", "Kapital: " + Kapital.ToString("F2") + " Nachkauf " + Nachkauf + " St�ck", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                }
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

		#endregion
	}
}