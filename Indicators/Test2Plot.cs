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
	[Description("Test mit 2 Plot")]
	public class Test2Plot : UserIndicator
    {
        #region Variables

        public double SoftStopp = 0.0;
        public double HardStopp = 0.0;

        private int _BarsRequired = 2;

        #endregion Variables
        protected override void OnInit()
		{
            RequiredBarsCount = _BarsRequired;
            Add(new Plot(Color.FromArgb(255, 102, 255, 255), PlotStyle.Cross, "Soft_Stopp"));
            Plots[0].Pen.Width = 2;
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Hash, "Hard_Stopp"));
            Plots[1].Pen.Width = 3;
            IsOverlay = true;
			CalculateOnClosedBar = true;
            

        }
        
		protected override void OnCalculate()
		{
           // SoftStopp = High[0];
           // HardStopp = Low[0];
            Soft_Stopp.Set(SoftStopp);
            // was geändert, test
			Hard_Stopp.Set(HardStopp);
		}
       
        public void Zeichne(double SoftStopp, double HardStopp)
        {
            if (SoftStopp > 1) Soft_Stopp.Set(SoftStopp);
            if (HardStopp > 1) Hard_Stopp.Set(HardStopp);
        }
        
		#region Properties
        
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Soft_Stopp
		{
			get { return Outputs[0]; }
            //set { Outputs[0] = value; }
        }

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Hard_Stopp
		{
			get { return Outputs[1]; }
            //set { Outputs[1] = value; }
            //set { _profitOnly = value; }
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
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot()
        {
			return Test2Plot(InSeries);
		}

		/// <summary>
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Test2Plot>(input);

			if (indicator != null)
				return indicator;

			indicator = new Test2Plot
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
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot()
		{
			return LeadIndicator.Test2Plot(InSeries);
		}

		/// <summary>
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Test2Plot(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot()
		{
			return LeadIndicator.Test2Plot(InSeries);
		}

		/// <summary>
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot(IDataSeries input)
		{
			return LeadIndicator.Test2Plot(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot()
		{
			return LeadIndicator.Test2Plot(InSeries);
		}

		/// <summary>
		/// Test mit 2 Plot
		/// </summary>
		public Test2Plot Test2Plot(IDataSeries input)
		{
			return LeadIndicator.Test2Plot(input);
		}
	}

	#endregion

}

#endregion
