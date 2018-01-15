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
    public class Trend_Stopp_I : UserIndicator
    {
        #region Variable
        private double Stopp;
        private int _trend;
        private int _abstand;
        private IDataSeries dStopp;
        #endregion Variable

        protected override void OnInit()
        {
            AddOutput(new OutputDescriptor(Color.FromKnownColor(KnownColor.DarkOrange), OutputSerieDrawStyle.Hash, "Soft_Stopp"));
            IsOverlay = true;
            dStopp = new DataSeries(this);
        }

        protected override void OnCalculate()
        {
            if (TradeInfo == null)
                return;
            else
            {
                // #region Trend-Stopp-Berechnung: 

                if (P123Adv(_trend).ValidP3Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).ValidP3DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    Stopp = Math.Max(Stopp, (P123Adv(_trend).ValidP3Price[0] - _abstand * TickSize));       //im Aufw�rtstrend: P3 g�ltig
                else if (P123Adv(_trend).ValidP3Price[0] < 1 && P123Adv(_trend).P2Price[0] > P123Adv(_trend).P1Price[0] && P123Adv(_trend).P2DateTime[0] > P123Adv(_trend).P1DateTime[0])
                    Stopp = Math.Max(Stopp, (P123Adv(_trend).P1Price[0]) - _abstand * TickSize);            // im Aufw�rtsttrend, noch kein P3 vorhanden,  Stopp am P1
                else if (P123Adv(_trend).P2Price[0] < P123Adv(_trend).P1Price[0] && P123Adv(_trend).TempP3DateTime[0] > P123Adv(_trend).P2DateTime[0]
                       && P123Adv(_trend).P2DateTime[0] < P123Adv(_trend).TempP3DateTime[0])
                    Stopp = Math.Max(Stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize);           //im Abw�rtstrend: TempP3 g�ltig, Stopp am letztenP2
                                                                                                           //  else if (P123Adv(_trend).P2DateTime[0] < TradingManager.Core.TradingManager.GetTrade.Times;  //Info.CreatedDateTime)
                                                                                                           //      Stopp = Math.Max(Stopp, (P123Adv(_trend).P2Price[0]) - _abstand * TickSize);           //im Abw�rtstrend: P2 vor Kauf, Stopp am g�ltigen P2
                                                                                                           //else Stopp = (Close[0] - Trade.AvgPrice)/2 + Trade.AvgPrice);                             // Sonderfall: nach ausl�sen des Stopp Sperre bis neuem, g�ltigen Stopp
                Soft_Stopp[0] = Stopp;

                //   #endregion Trend-Stopp-Berechnung
            }


            Soft_Stopp.Set(InSeries[0]);
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Soft_Stopp
        {
            get { return Outputs[0]; }
        }

        #endregion
    }
}