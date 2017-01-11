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

        private int _period = 14;
  
        #endregion      
          
		protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "Umsatz"));
            CalculateOnClosedBar = true;
            RequiredBarsCount = _period+1;
            IsOverlay = false;
        }

		protected override void OnCalculate()
		{
            if(Volume[0] > 0)
            Umsatz.Set(Volume[0] / SMA(Volume, _period)[0]);
            //Umsatz.Set(SMA(Volume, _period)[0] * SMA(Close, _period)[0]/1000);

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
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz()
        {
			return MarktUmsatz(InSeries);
		}

		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<MarktUmsatz>(input);

			if (indicator != null)
				return indicator;

			indicator = new MarktUmsatz
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
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz()
		{
			return LeadIndicator.MarktUmsatz(InSeries);
		}

		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.MarktUmsatz(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz()
		{
			return LeadIndicator.MarktUmsatz(InSeries);
		}

		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz(IDataSeries input)
		{
			return LeadIndicator.MarktUmsatz(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz()
		{
			return LeadIndicator.MarktUmsatz(InSeries);
		}

		/// <summary>
		/// Durchschnittlicher, täglicher Marktumsatz
		/// </summary>
		public MarktUmsatz MarktUmsatz(IDataSeries input)
		{
			return LeadIndicator.MarktUmsatz(input);
		}
	}

	#endregion

}

#endregion
