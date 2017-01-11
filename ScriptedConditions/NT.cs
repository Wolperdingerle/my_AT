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

	[Description("Neuer Trend Einstieg")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    [TimeFrameRequirements("1 hour", "1 day")]
    public class NT : UserScriptedCondition
	{
		#region Variables

		private int _trend = 2;

        #endregion
        public NT()
        {

        }

        protected override void OnBarsRequirements()
        {
            Add(new TimeFrame("1 day"));
        }



        protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 137, 183, 54), "Entry"));
			IsOverlay = true;
            CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{
            CalculateEntry();
        }
        private void CalculateEntry()
        {
            // Berechnung long oder Short
            /* Long-Einstieg wenn: Umsatz am Vortag größer als dem 3-fachen Durchschnittsumsatz der letzten 14 Tage ist.
             * Es liegt ein 

            /*
                    switch (TradeDirection)
                    {
                        case PositionType.Long:
                            Occurred.Set(1);
                            Entry.Set();

                            break;

                        case PositionType.Short:
                            Occurred.Set(-1);
                            Entry.Set();
                        case PositionType.Flat:
                            Occurred.Reset(0);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    */
        }
        #region Properties

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
			return new[]{Entry};
		}

		[Description("Trendstärke")]
		[Category("Parameters")]
		public int Trend
		{
			get { return _trend; }
			set { _trend = Math.Min(Math.Max(0, value),3); }
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
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(System.Int32 trend)
        {
			return NT(InSeries, trend);
		}

		/// <summary>
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(IDataSeries input, System.Int32 trend)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<NT>(input, i => i.Trend == trend);

			if (indicator != null)
				return indicator;

			indicator = new NT
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Trend = trend
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
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(System.Int32 trend)
		{
			return LeadIndicator.NT(InSeries, trend);
		}

		/// <summary>
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(IDataSeries input, System.Int32 trend)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.NT(input, trend);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(System.Int32 trend)
		{
			return LeadIndicator.NT(InSeries, trend);
		}

		/// <summary>
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(IDataSeries input, System.Int32 trend)
		{
			return LeadIndicator.NT(input, trend);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(System.Int32 trend)
		{
			return LeadIndicator.NT(InSeries, trend);
		}

		/// <summary>
		/// Neuer Trend Einstieg
		/// </summary>
		public NT NT(IDataSeries input, System.Int32 trend)
		{
			return LeadIndicator.NT(input, trend);
		}
	}

	#endregion

}

#endregion
