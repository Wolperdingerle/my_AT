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
	public abstract partial class UserStrategy : Strategy
	{
		private EmptyIndicator _indicatorPrivate;

		protected EmptyIndicator LeadIndicator
		{
			get { return _indicatorPrivate ?? (_indicatorPrivate = new EmptyIndicator { InSeries = InSeries }); }
		}

		/// <summary>
		/// Used for backward compatibility with previously generated code.
		/// </summary>
		protected EmptyIndicator _indicator
		{
			get { return LeadIndicator; }
		}

		private IApiBase _indicator4MarktTechnikProfessionalPackage;

		protected _MarktTechnikProfessionalPackage.AgenaTrader.UserCode.EmptyIndicator LeadIndicator4MarktTechnikProfessionalPackage
		{
			get { return (_MarktTechnikProfessionalPackage.AgenaTrader.UserCode.EmptyIndicator) (_indicator4MarktTechnikProfessionalPackage ?? (_indicator4MarktTechnikProfessionalPackage = new _MarktTechnikProfessionalPackage.AgenaTrader.UserCode.EmptyIndicator { InSeries = InSeries })); }
		}

		protected UserStrategy()
		{
			IsAutoConfirmOrder = true;
		}

		public override void Dispose()
		{
			if (_indicatorPrivate != null) { try { _indicatorPrivate.Dispose(); } catch { } _indicatorPrivate = null; }
			if (_indicator4MarktTechnikProfessionalPackage != null) { try { _indicator4MarktTechnikProfessionalPackage.Dispose(); } catch { } _indicator4MarktTechnikProfessionalPackage = null; }
			base.Dispose();
		}
	}
}
