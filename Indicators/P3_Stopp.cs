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
	public class P3_Stopp : UserIndicator
	{
        #region Variable
        private double _stopp;
        private int _abstand = 0;
        private DateTime StartDatum;


        #endregion

        public P3_Stopp()
		{
			Trend = 14;
		}

		protected override void OnInit()
		{
			// Executed once at the beginning
			// Write your own initialization code
			// More Information: https://agenatrader.github.io/AgenaScript-documentation/keywords/#oninit

			AddOutput(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Stopp"));
			IsOverlay = true;
		}

		protected override void OnCalculate()
		{
            // Executed once for every single bar, starting from the first in the char
            // Write your own bar handling here
            // More Information: https://agenatrader.github.io/AgenaScript-documentation/events/#oncalculate

            //Stopp.Set(InSeries[0]);
            P3(_stopp, Trend, StartDatum);

		}

        public void P3(double _stopp, int Trend, DateTime StartDatum)
        {
            #region Trend-Stopp-Berechnung: Starte ab dem 2. Bar nach Tradeer�ffung
            if (Bars[1] != null && Bars[1].Time > StartDatum)
            {
                if (Core.PreferenceManager.IsAtrEntryDistance) _abstand = (int)Math.Max(_abstand, ATR(14)[1] * Core.PreferenceManager.AtrEntryDistanceFactor);    // Tick-Abstand
                if (P123Adv(_trend).ValidP3Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).ValidP3DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).ValidP3Price[0] - _abstand * TickSize)));       //im Aufw�rtstrend: P3 g�ltig
                else if (P123Adv(_trend).ValidP3Price[0] < 1 && P123Adv(_trend).P2Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).P2DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).P1Price[0]) - _abstand * TickSize));            // im Aufw�rtsttrend, noch kein P3 vorhanden,  Stopp am P1
                else if (P123Adv(_trend).P2Price[0] < P123Adv(_trend).P1Price[0] && P123Adv(_trend).TempP3DateTime[0] > P123Adv(_trend).P2DateTime[0]
                    && P123Adv(_trend).P2DateTime[0] < P123Adv(_trend).TempP3DateTime[0])
                    _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize));           //im Abw�rstrend: TempP3 g�ltig, Stopp am letzten P2
                                                                                                                                        // else if (P123Adv(_trend).P2DateTime[0] < startZeit)
                                                                                                                                        // _stopp = Instrument.Round2TickSize(Math.Max(_stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize));           //im Abw�rtstrend: P2 vor Kauf, Stopp am g�ltigen P2

                if (_stopp > 0)
                    Stopp.Set(_stopp);     // zeigt Verlauf Softstopp
                else
                    Stopp.Reset();
            }
            #endregion

        }
        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Stopp
		{
			get { return Outputs[0]; }
		}

		[InputParameter]
		public int Trend    // Werte von 0 bis 3
		{
			get { return _trend; }
			set { _trend = Math.Min( Math.Max(0, value),3); }
		}
		private int _trend;

		#endregion
	}
}