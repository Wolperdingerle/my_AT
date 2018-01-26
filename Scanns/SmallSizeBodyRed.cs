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
	[UsedCalculationUnit(typeof(global::AgenaTrader.Custom.ATR))]
	[ScanTypeAttribute(ScanType.Short)]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[Category("BodySize")]
	[Description("Detects Downbars with small body size")]
	public class SmallSizeBodyRed : UserScan
	{
		protected override void OnInit()
		{
			ScanType = ScanType.Short;

			IsEntry = true;
			IsStop = false;
			IsTarget = false;

			ArrowColor = Color.Red;
			ArrowSize = 0.5;
			TextColor = null;
			ScanColor = null;
			BarHighlightColor = Color.Red;
		}

		protected override void OnCalculate()
		{
			LastOutput = MultiBars.GetBarsItem(TimeFrame).Open[0] > MultiBars.GetBarsItem(TimeFrame).Close[0]
				&& ((MultiBars.GetBarsItem(TimeFrame).Open[0]-MultiBars.GetBarsItem(TimeFrame).Close[0])) < ((LeadIndicator.ATR(MultiBars.GetBarsItem(TimeFrame).Close, 14)[0]/2))
				&& ((MultiBars.GetBarsItem(TimeFrame).Open[0]-MultiBars.GetBarsItem(TimeFrame).Close[0])) > ((LeadIndicator.ATR(MultiBars.GetBarsItem(TimeFrame).Close, 14)[0]/3));
		}
	}
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manually
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="false" ScanType="Short" IsEntry="true" IsStop="false" IsTarget="false" Category="BodySize" Description="Detects Downbars with small body size" ArrowColor="Red" TextColor="" ScanColor="" BarHighlightColor="Red">
  <Lexems xsi:type="ScanLexemSerie" Type="Open">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo>0</BarsAgo>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Great" />
  <Lexems xsi:type="ScanLexemSerie" Type="Close">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo>0</BarsAgo>
  </Lexems>
  <Lexems xsi:type="ScanLexemCondition" Condition="And" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemSerie" Type="Open">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemSerie" Type="Close">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Less" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
      <BarsAgo>0</BarsAgo>
      <XmlIndicator Name="ATR" Type="Indicator" FullTypeName="AgenaTrader.Custom.ATR">
        <Params>
          <TAParam Name="Period" IsProperty="true">
            <Value xsi:type="xsd:int">14</Value>
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
              <Plot Name="ATR" PlotColor="Blue" Width="1" PlotStyle="Line" PenStyle="Solid" />
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
            <Value xsi:type="xsd:boolean">true</Value>
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
            <Value xsi:type="xsd:string">ATR(Close, 14)</Value>
          </TAParam>
          <TAParam Name="Tags" IsProperty="true" />
        </Params>
      </XmlIndicator>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Div" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="2" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemCondition" Condition="And" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemSerie" Type="Open">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemSerie" Type="Close">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Great" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
      <BarsAgo>0</BarsAgo>
      <XmlIndicator Name="ATR" Type="Indicator" FullTypeName="AgenaTrader.Custom.ATR">
        <Params>
          <TAParam Name="Period" IsProperty="true">
            <Value xsi:type="xsd:int">14</Value>
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
              <Plot Name="ATR" PlotColor="Blue" Width="1" PlotStyle="Line" PenStyle="Solid" />
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
            <Value xsi:type="xsd:boolean">true</Value>
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
            <Value xsi:type="xsd:string">ATR(Close, 14)</Value>
          </TAParam>
          <TAParam Name="Tags" IsProperty="true" />
        </Params>
      </XmlIndicator>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Div" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="3" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <CalculateOnBarClose xsi:nil="true" />
  <ArrowSize>0.5</ArrowSize>
</ScanWizartState>
*/
#endregion
