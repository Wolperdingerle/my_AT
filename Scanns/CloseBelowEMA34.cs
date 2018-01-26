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
	[UsedCalculationUnit(typeof(global::AgenaTrader.Custom.EMA))]
	[ScanTypeAttribute(ScanType.Short)]
	[IsEntryAttribute(false)]
	[IsStopAttribute(true)]
	[IsTargetAttribute(false)]
	[Category("EMA34")]
	[Description("Price crosses below EMA34")]
	public class CloseBelowEMA34 : UserScan
	{
		protected override void OnInit()
		{
			ScanType = ScanType.Short;

			IsEntry = false;
			IsStop = true;
			IsTarget = false;

			ArrowColor = Color.Red;
			ArrowSize = 0.25;
			TextColor = null;
			ScanColor = null;
			BarHighlightColor = null;
		}

		protected override void OnCalculate()
		{
			LastOutput = CrossBelow(MultiBars.GetBarsItem(TimeFrame).Close, LeadIndicator.EMA(MultiBars.GetBarsItem(TimeFrame).Close, 34), 1);
		}
	}
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manually
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="false" ScanType="Short" IsEntry="false" IsStop="true" IsTarget="false" Category="EMA34" Description="Price crosses below EMA34" ArrowColor="Red" TextColor="" ScanColor="" BarHighlightColor="">
  <Lexems xsi:type="ScanLexemSerie" Type="Close">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo xsi:nil="true" />
  </Lexems>
  <Lexems xsi:type="ScanLexemCrossOperation" Operation="CrossBelow" Period="1" />
  <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
    <BarsAgo xsi:nil="true" />
    <XmlIndicator Name="EMA" Type="Indicator" FullTypeName="AgenaTrader.Custom.EMA">
      <Params>
        <TAParam Name="Period" IsProperty="true">
          <Value xsi:type="xsd:int">34</Value>
        </TAParam>
        <TAParam Name="XmlInstrument" IsProperty="true" />
        <TAParam Name="TimeFrame" IsProperty="true" />
        <TAParam Name="ChartVisibleValuesCount" IsProperty="true">
          <Value xsi:type="xsd:int">0</Value>
        </TAParam>
        <TAParam Name="IsOverlay" IsProperty="true">
          <Value xsi:type="xsd:boolean">true</Value>
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
          <Value xsi:type="xsd:string">Same as input series</Value>
        </TAParam>
        <TAParam Name="IsPanelCustom" IsProperty="true">
          <Value xsi:type="xsd:boolean">false</Value>
        </TAParam>
        <TAParam Name="XmlPlots" IsProperty="true">
          <Value xsi:type="ArrayOfPlot">
            <Plot Name="EMA" PlotColor="Blue" Width="1" PlotStyle="Line" PenStyle="Solid" />
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
          <Value xsi:type="xsd:string">EMA(Close, 14)</Value>
        </TAParam>
        <TAParam Name="Tags" IsProperty="true" />
      </Params>
    </XmlIndicator>
  </Lexems>
  <CalculateOnBarClose xsi:nil="true" />
  <ArrowSize>0.25</ArrowSize>
</ScanWizartState>
*/
#endregion