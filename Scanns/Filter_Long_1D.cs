using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using global::AgenaTrader.API;
using global::AgenaTrader.Custom;
using global::AgenaTrader.Plugins;

namespace AgenaTrader.UserCode
{
	[TimeFrameRequirements("1 Week", "1 Day")]
	[ScanTypeAttribute(ScanType.Long)]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[Category("")]
	[Description("Trendfilter long 1 Tag MPh 3")]
	public class Filter_Long_1D : UserScan
	{
		protected override void OnInit()
		{
			ScanType = ScanType.Long;

			IsEntry = true;
			IsStop = false;
			IsTarget = false;

			ArrowColor = Color.FromArgb(255, 0, 51, 0);
			BarHighlightColor = null;
		}

		private static readonly ITimeFrame GenTF_1 = CoreInstanse.Current.TimeFrameFactory.Create(global::AgenaTrader.Plugins.DatafeedHistoryPeriodicity.Week, new global::AgenaTrader.Plugins.TimeFrames.RegularTimeFrameParameters(1));
		private static readonly ITimeFrame GenTF_2 = CoreInstanse.Current.TimeFrameFactory.Create(global::AgenaTrader.Plugins.DatafeedHistoryPeriodicity.Day, new global::AgenaTrader.Plugins.TimeFrames.RegularTimeFrameParameters(1));

		protected override void OnCalculate()
		{
			LastOutput = LeadIndicator.MarketPhasesPro(MultiBars.GetBarsItem(GenTF_1).Close, 2, 0.2, 0.5, global::AgenaTrader.Custom.P123ProValidationMethod.Correction, 0, 0, 0, false)[0] > 0
				&& Math.Abs(LeadIndicator.MarketPhasesPro(MultiBars.GetBarsItem(GenTF_2).Close, 2, 0.2, 0.5, global::AgenaTrader.Custom.P123ProValidationMethod.Correction, 0, 0, 0, false)[0] - 3) <= Double.Epsilon;
		}
	}

	#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

	public partial class UserScan
	{
		public Filter_Long_1D Filter_Long_1D()
		{
			return GetCondition<Filter_Long_1D>();
		}

		public Filter_Long_1D Filter_Long_1D(IDataSeries input)
		{
			return GetCondition<Filter_Long_1D>(input);
		}
	}

	#endregion
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manualy
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="false" ScanType="Long" IsEntry="true" IsStop="false" IsTarget="false" Category="" Description="Trendfilter long 1 Tag MPh 3" ArrowColor="0, 51, 0" BarHighlightColor="">
  <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true">
    <BarsAgo>0</BarsAgo>
    <XmlIndicator Name="MarketPhasesPro" Type="Indicator" FullTypeName="AgenaTrader.Custom.MarketPhasesPro">
      <Params>
        <TAParam Name="TrendSize" IsProperty="true">
          <Value xsi:type="xsd:int">2</Value>
        </TAParam>
        <TAParam Name="Phase4Level" IsProperty="true">
          <Value xsi:type="xsd:double">0.2</Value>
        </TAParam>
        <TAParam Name="Phase5Level" IsProperty="true">
          <Value xsi:type="xsd:double">0.5</Value>
        </TAParam>
        <TAParam Name="P2ValidationMethod" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="P2Distance" IsProperty="true">
          <Value xsi:type="xsd:double">0</Value>
        </TAParam>
        <TAParam Name="P3Distance" IsProperty="true">
          <Value xsi:type="xsd:double">0</Value>
        </TAParam>
        <TAParam Name="P3BreakageDistance" IsProperty="true">
          <Value xsi:type="xsd:double">0</Value>
        </TAParam>
        <TAParam Name="BreakTheTrendByCS" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="Location" IsProperty="true">
          <Value xsi:type="xsd:int">1</Value>
        </TAParam>
        <TAParam Name="XmlInstrument" IsProperty="true" />
        <TAParam Name="TimeFrame" IsProperty="true">
          <Value xsi:type="xsd:string">1 Week</Value>
        </TAParam>
        <TAParam Name="VisibleIndicatorValues" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="Overlay" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="DisplayInDataBox" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="PaintPriceMarkers" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="VerticalGridLines" IsProperty="true" />
        <TAParam Name="DrawOnPricePanel" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="Displacement" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="Panel" IsProperty="true">
          <Value xsi:type="xsd:string">Same as input series</Value>
        </TAParam>
        <TAParam Name="IsPanelCustom" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="XmlPlots" IsProperty="true">
          <Value xsi:type="ArrayOfPlot">
            <Plot Name="MarketPhase" PlotColor="255, 140, 0" Width="4" PlotStyle="Bar" PenStyle="Solid" />
          </Value>
        </TAParam>
        <TAParam Name="XmlLines" IsProperty="true">
          <Value xsi:type="ArrayOfLine" />
        </TAParam>
        <TAParam Name="FixedInputParameters" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">0</Value>
        </TAParam>
        <TAParam Name="ValuesDisplayStyle" IsProperty="true" />
        <TAParam Name="AutoScale" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="InputPriceType" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">3</Value>
        </TAParam>
        <TAParam Name="CustomInput" IsProperty="true" />
        <TAParam Name="CalculateOnBarClose" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="BarsRequired" IsProperty="true">
          <Value xsi:type="xsd:int">200</Value>
        </TAParam>
        <TAParam Name="Caption" IsProperty="true">
          <Value xsi:type="xsd:string">Market Phases Pro</Value>
        </TAParam>
        <TAParam Name="Tags" IsProperty="true" />
      </Params>
    </XmlIndicator>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Great" />
  <Lexems xsi:type="ScanLexemValue" LexemType="Double" CharValue="0" />
  <Lexems xsi:type="ScanLexemCondition" Condition="And" />
  <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true">
    <BarsAgo>0</BarsAgo>
    <XmlIndicator Name="MarketPhasesPro" Type="Indicator" FullTypeName="AgenaTrader.Custom.MarketPhasesPro">
      <Params>
        <TAParam Name="TrendSize" IsProperty="true">
          <Value xsi:type="xsd:int">2</Value>
        </TAParam>
        <TAParam Name="Phase4Level" IsProperty="true">
          <Value xsi:type="xsd:double">0.2</Value>
        </TAParam>
        <TAParam Name="Phase5Level" IsProperty="true">
          <Value xsi:type="xsd:double">0.5</Value>
        </TAParam>
        <TAParam Name="P2ValidationMethod" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="P2Distance" IsProperty="true">
          <Value xsi:type="xsd:double">0</Value>
        </TAParam>
        <TAParam Name="P3Distance" IsProperty="true">
          <Value xsi:type="xsd:double">0</Value>
        </TAParam>
        <TAParam Name="P3BreakageDistance" IsProperty="true">
          <Value xsi:type="xsd:double">0</Value>
        </TAParam>
        <TAParam Name="BreakTheTrendByCS" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="Location" IsProperty="true">
          <Value xsi:type="xsd:int">1</Value>
        </TAParam>
        <TAParam Name="XmlInstrument" IsProperty="true" />
        <TAParam Name="TimeFrame" IsProperty="true">
          <Value xsi:type="xsd:string">1 Day</Value>
        </TAParam>
        <TAParam Name="VisibleIndicatorValues" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="Overlay" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="DisplayInDataBox" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="PaintPriceMarkers" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="VerticalGridLines" IsProperty="true" />
        <TAParam Name="DrawOnPricePanel" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="Displacement" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="Panel" IsProperty="true">
          <Value xsi:type="xsd:string">Same as input series</Value>
        </TAParam>
        <TAParam Name="IsPanelCustom" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="XmlPlots" IsProperty="true">
          <Value xsi:type="ArrayOfPlot">
            <Plot Name="MarketPhase" PlotColor="255, 140, 0" Width="4" PlotStyle="Bar" PenStyle="Solid" />
          </Value>
        </TAParam>
        <TAParam Name="XmlLines" IsProperty="true">
          <Value xsi:type="ArrayOfLine" />
        </TAParam>
        <TAParam Name="FixedInputParameters" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">0</Value>
        </TAParam>
        <TAParam Name="ValuesDisplayStyle" IsProperty="true" />
        <TAParam Name="AutoScale" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="InputPriceType" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">3</Value>
        </TAParam>
        <TAParam Name="CustomInput" IsProperty="true" />
        <TAParam Name="CalculateOnBarClose" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="BarsRequired" IsProperty="true">
          <Value xsi:type="xsd:int">200</Value>
        </TAParam>
        <TAParam Name="Caption" IsProperty="true">
          <Value xsi:type="xsd:string">Market Phases Pro</Value>
        </TAParam>
        <TAParam Name="Tags" IsProperty="true" />
      </Params>
    </XmlIndicator>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Equal" />
  <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="3" CharValue="0" />
  <CalculateOnClosedBar xsi:nil="true" />
</ScanWizartState>
*/
#endregion
