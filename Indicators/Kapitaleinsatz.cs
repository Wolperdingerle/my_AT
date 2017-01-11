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
       // private int Nachkauf;

		protected override void OnInit()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
            CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{
			//MyPlot1.Set(Input[0]);
            if (TradeInfo != null)
            {
                if (TradeInfo.PositionType != PositionType.Flat)
                { 
                    Kapital = (TradeInfo.Quantity * TradeInfo.AvgPrice);
                    //Nachkauf = (int)(Math.Max(Kapital * 0.5, 4000) / Close[0]);
                   // DrawTextFixed("MyText", "Kapital: " + Kapital.ToString("F2") + " Nachkauf " + Nachkauf + " Stück", TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    AddChartTextFixed("MyText", "Kapital: " + Kapital.ToString("F2"), TextPosition.BottomLeft, Color.Red, new Font("Areal", 12), Color.Blue, Color.Empty, 10);
                    /*
                    if (.Orders.Count > 0)
                    {
                        int i = 0;
                        do
                        {
                            if (Orders[i].Action == OrderAction.Buy && Orders[i].OrderState != OrderState.Filled && Orders[i].OrderType == OrderType.Limit)
                            {
                                EnterOrder = Orders[i];
                                Wert = EnterOrder.Quantity * EnterOrder.LimitPrice;
                            }
                            ++i;
                        } while (i < Orders.Count);
                    }
                    */
                }
            }
        }

		#region Properties
/*
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}
        */
		#endregion
	}
}

#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz()
        {
			return Kapitaleinsatz(InSeries);
		}

		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Kapitaleinsatz>(input);

			if (indicator != null)
				return indicator;

			indicator = new Kapitaleinsatz
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz()
		{
			return LeadIndicator.Kapitaleinsatz(InSeries);
		}

		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Kapitaleinsatz(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz()
		{
			return LeadIndicator.Kapitaleinsatz(InSeries);
		}

		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz(IDataSeries input)
		{
			return LeadIndicator.Kapitaleinsatz(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz()
		{
			return LeadIndicator.Kapitaleinsatz(InSeries);
		}

		/// <summary>
		/// Kapitaleinsatz des laufenden Trades
		/// </summary>
		public Kapitaleinsatz Kapitaleinsatz(IDataSeries input)
		{
			return LeadIndicator.Kapitaleinsatz(input);
		}
	}

	#endregion

}

#endregion
