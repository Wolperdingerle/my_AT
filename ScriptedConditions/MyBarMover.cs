/*
Funktioniert mit Long-Trades,
Short hakt noch


 * This program is just an example and should not be used in an
 * productive envirtonment! Do NOT trade with this program!
 *
 * Copyright (C) 2015  Stefan Filipiak <opensource@meinindikator.de>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

#region Using declarations
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
#endregion

namespace AgenaTrader.UserCode
{
    [Description("BarByBar Gewinn-Stopp")]
    [IsEntryAttribute(false)]
    [IsStopAttribute(true)]
    [IsTargetAttribute(false)]
    [OverrulePreviousStopPrice(true)]

    public class MyBarMover : UserScriptedCondition
    {
        #region Variables
        private int _abstand = 2;
        private int _toleranz = 1;
        private double _profit = 2;
        private double Stopp;
        
        #endregion

        public MyBarMover()
        {
           
        }

        protected override void OnInit()

        {
            IsOverlay = true;
            CalculateOnClosedBar = true;
            RequiredBarsCount = 2;
            IsEntry = false;
            IsStop = true;
            IsTarget = false;
            Add(new Plot(Color.Transparent, "Occurred"));
            Add(new Plot(Color.Magenta, "Stop"));   // Kurs des Stops
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnCalculate()
        {
            CalculateStop();
        }

        /// <summary>

        /// Recalculate wird z. B. von AT++ aufgerufen, wenn man �ber die
        /// ActionBar f�r die B/S-Buttons kauft bzw. verkauft (oder �ber
        /// das Kontextmen�).
        /// 
        /// �ber Recalculate wird die initiale TradeDirection abgerufen,
        /// liegt das Setup im Markt, dann geht die Reise �ber OnBarUpdate
        /// weiter.
        /// </summary>
        public override void Recalculate()

        {
            CalculateStop();
        }

        private void CalculateStop()
        {
            // Occurred muss beim Stop nicht gesetzt werden, weil der Stop
            // nur von der Traderichtung abh�ngig ist.
            switch (TradeDirection)
            {
                case PositionType.Long:
                    if (InsideBarsMT(InSeries, InsideBarsMTToleranceUnit.Ticks, _toleranz).IsInsideBar[0] == 1.0)
                    {
                        Stopp = Instrument.Round2TickSize(InsideBarsMT(InSeries, InsideBarsMTToleranceUnit.Ticks, _toleranz).LowBeforeOutsideBar[0] - _abstand * TickSize);
                    }
                    else
                    {
                        Stopp = Instrument.Round2TickSize(Math.Max(Stopp, (Low[0] - _abstand * TickSize)));
                    }
                    if ((_profit == 0) || (TradeInfo != null && _profit > 0 && Stopp > TradeInfo.AvgPrice * (1 + _profit / 1000)))
                    {
                        Stop.Set(Stopp);
                    }
                    break;

                case PositionType.Short:
                    if (Stopp < (1 + _abstand) * TickSize)
                    {
                        Stopp = Math.Max(High[0], High[1]) + _abstand * TickSize;
                        Stop.Set(Stopp);
                        Stop[1] = Stopp;
                        Print(" 0 " + Stopp);
                    }
                    if (InsideBarsMT(InSeries, InsideBarsMTToleranceUnit.Ticks, _toleranz).IsInsideBar[0] == 1.0)
                    {
                        Stopp = Instrument.Round2TickSize(InsideBarsMT().HighBeforeOutsideBar[0] + _abstand * TickSize);
                        Print(" 1 " + Stopp);
                        Stopp = Instrument.Round2TickSize(InsideBarsMT(InSeries, InsideBarsMTToleranceUnit.Ticks, _toleranz).HighBeforeOutsideBar[0] + _abstand * TickSize);
                        Print(" 2 " + Stopp);
                    }
                    else
                    {
                        Stopp = Instrument.Round2TickSize(Math.Min(Stopp, (High[0] + _abstand * TickSize)));
                        Print(" 3 " + Stopp);
                    }
                    if ((_profit == 0) || (TradeInfo != null && _profit > 0 && Stopp < TradeInfo.AvgPrice * (1 - _profit / 1000)))
                    {

                        Stop.Set(Stopp);
                    }
                    else
                    {
                        if (Stop[1] > 0)
                        Stop.Set(Stop[1]);
                    }
                    break;

                case PositionType.Flat:
                    Stop.Reset();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        public DataSeries Stop
        {
            get { return Outputs[1]; }
        }

        public override IList<DataSeries> GetStops()
        {
            return new[] { Stop };
        }

        [Description("")]
        [Category("Parameters")]
        public int Tick_Abstand_vom_Bar
        {
            get { return _abstand; }
            set { _abstand = Math.Max(0, value); }
        }

     //   [Description("Toleranz in Ticks f�r Insidebars.")]
        [Category("Parameters")]
        public int Toleranz_InsideBars
        {
            get { return _toleranz; }
            set { _toleranz = Math.Max(0, value); }
        }

       // [Description("Mindestgewinn in Promille.")]
        [Category("Parameters")]
        public double Profit_Promille
        {
            get { return _profit; }
            set { _profit = Math.Max(0, value); }
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
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
        {
			return MyBarMover(InSeries, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}

		/// <summary>
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(IDataSeries input, System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<MyBarMover>(input, i => i.Tick_Abstand_vom_Bar == tick_Abstand_vom_Bar && i.Toleranz_InsideBars == toleranz_InsideBars && Math.Abs(i.Profit_Promille - profit_Promille) <= Double.Epsilon);

			if (indicator != null)
				return indicator;

			indicator = new MyBarMover
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Tick_Abstand_vom_Bar = tick_Abstand_vom_Bar,
							Toleranz_InsideBars = toleranz_InsideBars,
							Profit_Promille = profit_Promille
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
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			return LeadIndicator.MyBarMover(InSeries, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}

		/// <summary>
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(IDataSeries input, System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.MyBarMover(input, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			return LeadIndicator.MyBarMover(InSeries, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}

		/// <summary>
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(IDataSeries input, System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			return LeadIndicator.MyBarMover(input, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			return LeadIndicator.MyBarMover(InSeries, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}

		/// <summary>
		/// BarByBar Gewinn-Stopp
		/// </summary>
		public MyBarMover MyBarMover(IDataSeries input, System.Int32 tick_Abstand_vom_Bar, System.Int32 toleranz_InsideBars, System.Double profit_Promille)
		{
			return LeadIndicator.MyBarMover(input, tick_Abstand_vom_Bar, toleranz_InsideBars, profit_Promille);
		}
	}

	#endregion

}

#endregion
