using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using AgenaTrader.API;
using AgenaTrader.Plugins;
using AgenaTrader.Custom;

namespace AgenaTrader.UserCode
{
	public abstract partial class UserScan : ScanBase
	{
		private EmptyIndicator _indicator;

		protected EmptyIndicator LeadIndicator
		{
			get { return _indicator ?? (_indicator = new EmptyIndicator { InSeries = InSeries }); }
		}

		private IApiBase _indicator4MarktTechnikProfessionalPackage;

		protected _MarktTechnikProfessionalPackage.AgenaTrader.UserCode.EmptyIndicator LeadIndicator4MarktTechnikProfessionalPackage
		{
			get { return (_MarktTechnikProfessionalPackage.AgenaTrader.UserCode.EmptyIndicator) (_indicator4MarktTechnikProfessionalPackage ?? (_indicator4MarktTechnikProfessionalPackage = new _MarktTechnikProfessionalPackage.AgenaTrader.UserCode.EmptyIndicator { InSeries = InSeries })); }
		}

		public override void Dispose()
		{
			if (_indicator != null) { try { _indicator.Dispose(); } catch { } _indicator = null; }
			if (_indicator4MarktTechnikProfessionalPackage != null) { try { _indicator4MarktTechnikProfessionalPackage.Dispose(); } catch { } _indicator4MarktTechnikProfessionalPackage = null; }
			base.Dispose();
		}
	}
}
