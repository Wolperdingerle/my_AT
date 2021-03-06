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
	[UsedCalculationUnit(typeof(global::AgenaTrader.Custom.LowestLowPrice))]
	[ScanTypeAttribute(ScanType.Short)]
	[IsEntryAttribute(false)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[Category("52Periods")]
	[Description("Detect a new 52 period low")]
	public class New52PeriodLow : UserScan
	{
		protected override void OnInit()
		{
			ScanType = ScanType.Short;

			IsEntry = false;
			IsStop = false;
			IsTarget = false;

			ArrowColor = null;
			ArrowSize = 0.5;
			TextColor = null;
			ScanColor = null;
			BarHighlightColor = Color.Black;
		}

		protected override void OnCalculate()
		{
			LastOutput = LeadIndicator.LowestLowPrice(MultiBars.GetBarsItem(TimeFrame).Close, 52)[0] < LeadIndicator.LowestLowPrice(MultiBars.GetBarsItem(TimeFrame).Close, 52)[1];
		}
	}
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manually
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="false" ScanType="Short" IsEntry="false" IsStop="false" IsTarget="false" Category="52Periods" Description="Detect a new 52 period low" ArrowColor="" TextColor="" ScanColor="" BarHighlightColor="Black">
  <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
    <BarsAgo>0</BarsAgo>
    <XmlIndicator Name="LowestLowPrice" Type="Indicator" FullTypeName="AgenaTrader.Custom.LowestLowPrice">
      <Params>
        <TAParam Name="BarsBack" IsProperty="true">
          <Value xsi:type="xsd:int">52</Value>
        </TAParam>
        <TAParam Name="XmlInstrument" IsProperty="true" />
        <TAParam Name="TimeFrame" IsProperty="true" />
        <TAParam Name="ChartVisibleValuesCount" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="IsOverlay" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="IsShowInDataBox" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="IsShowPriceMarkers" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="IsShowChartVerticalGrid" IsProperty="true" />
        <TAParam Name="IsAddDrawingsToPricePanel" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="Displacement" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="Panel" IsProperty="true">
          <Value xsi:type="xsd:string">New Panel</Value>
        </TAParam>
        <TAParam Name="IsPanelCustom" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="XmlPlots" IsProperty="true">
          <Value xsi:type="ArrayOfPlot">
            <Plot Name="LowestLow" PlotColor="Blue" Width="1" PlotStyle="Line" PenStyle="Solid" />
          </Value>
        </TAParam>
        <TAParam Name="XmlLines" IsProperty="true">
          <Value xsi:type="ArrayOfLevelLine" />
        </TAParam>
        <TAParam Name="FixedInputParameters" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">0</Value>
        </TAParam>
        <TAParam Name="OutputsDisplayStyle" IsProperty="true" />
        <TAParam Name="IsAutoAdjustableScale" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="InputPriceType" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">3</Value>
        </TAParam>
        <TAParam Name="CustomInput" IsProperty="true" />
        <TAParam Name="CalculateOnClosedBar" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="CalculationUpdatesTimeFrame" IsProperty="true" />
        <TAParam Name="RequiredBarsCount" IsProperty="true">
          <Value xsi:type="xsd:int">20</Value>
        </TAParam>
        <TAParam Name="Caption" IsProperty="true">
          <Value xsi:type="xsd:string">LowestLowPrice(Close, 5)</Value>
        </TAParam>
        <TAParam Name="Tags" IsProperty="true" />
      </Params>
    </XmlIndicator>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Less" />
  <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
    <BarsAgo>1</BarsAgo>
    <XmlIndicator Name="LowestLowPrice" Type="Indicator" FullTypeName="AgenaTrader.Custom.LowestLowPrice">
      <Params>
        <TAParam Name="BarsBack" IsProperty="true">
          <Value xsi:type="xsd:int">52</Value>
        </TAParam>
        <TAParam Name="XmlInstrument" IsProperty="true" />
        <TAParam Name="TimeFrame" IsProperty="true" />
        <TAParam Name="ChartVisibleValuesCount" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="IsOverlay" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="IsShowInDataBox" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="IsShowPriceMarkers" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="IsShowChartVerticalGrid" IsProperty="true" />
        <TAParam Name="IsAddDrawingsToPricePanel" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="Displacement" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="Panel" IsProperty="true">
          <Value xsi:type="xsd:string">New Panel</Value>
        </TAParam>
        <TAParam Name="IsPanelCustom" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="XmlPlots" IsProperty="true">
          <Value xsi:type="ArrayOfPlot">
            <Plot Name="LowestLow" PlotColor="Blue" Width="1" PlotStyle="Line" PenStyle="Solid" />
          </Value>
        </TAParam>
        <TAParam Name="XmlLines" IsProperty="true">
          <Value xsi:type="ArrayOfLevelLine" />
        </TAParam>
        <TAParam Name="FixedInputParameters" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">0</Value>
        </TAParam>
        <TAParam Name="OutputsDisplayStyle" IsProperty="true" />
        <TAParam Name="IsAutoAdjustableScale" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="InputPriceType" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">3</Value>
        </TAParam>
        <TAParam Name="CustomInput" IsProperty="true" />
        <TAParam Name="CalculateOnClosedBar" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
        </TAParam>
        <TAParam Name="CalculationUpdatesTimeFrame" IsProperty="true" />
        <TAParam Name="RequiredBarsCount" IsProperty="true">
          <Value xsi:type="xsd:int">20</Value>
        </TAParam>
        <TAParam Name="Caption" IsProperty="true">
          <Value xsi:type="xsd:string">LowestLowPrice(Close, 5)</Value>
        </TAParam>
        <TAParam Name="Tags" IsProperty="true" />
      </Params>
    </XmlIndicator>
  </Lexems>
  <CalculateOnBarClose xsi:nil="true" />
  <ArrowSize>0.5</ArrowSize>
</ScanWizartState>
*/
#endregion
