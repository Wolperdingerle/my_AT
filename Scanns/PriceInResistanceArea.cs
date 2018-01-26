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
	[UsedCalculationUnit(typeof(global::AgenaTrader.Custom.Swing), typeof(global::AgenaTrader.Custom.ATR))]
	[ScanTypeAttribute(ScanType.Short)]
	[IsEntryAttribute(false)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[Category("SupportResistance")]
	[Description("Detects charts where the price is near a resistance area")]
	public class PriceInResistanceArea : UserScan
	{
		protected override void OnInit()
		{
			ScanType = ScanType.Short;

			IsEntry = false;
			IsStop = false;
			IsTarget = false;

			ArrowColor = Color.FromArgb(255, 204, 0, 0);
			ArrowSize = 0.5;
			TextColor = null;
			ScanColor = null;
			BarHighlightColor = null;
		}

		protected override void OnCalculate()
		{
			LastOutput = MultiBars.GetBarsItem(TimeFrame).Close[0] <= (((LeadIndicator.Swing(MultiBars.GetBarsItem(TimeFrame).Close, 5).SwingHigh[1]+0)+(LeadIndicator.ATR(MultiBars.GetBarsItem(TimeFrame).Close, 14)[0]*0.35)))
				&& MultiBars.GetBarsItem(TimeFrame).Close[0] >= (((LeadIndicator.Swing(MultiBars.GetBarsItem(TimeFrame).Close, 5).SwingHigh[1]+0)-(LeadIndicator.ATR(MultiBars.GetBarsItem(TimeFrame).Close, 14)[0]*0.35)));
		}
	}
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manually
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="true" ScanType="Short" IsEntry="false" IsStop="false" IsTarget="false" Category="SupportResistance" Description="Detects charts where the price is near a resistance area" ArrowColor="204, 0, 0" TextColor="" ScanColor="" BarHighlightColor="">
  <Lexems xsi:type="ScanLexemSerie" Type="Close">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo>0</BarsAgo>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="LessEqual" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemIndicator" Serie="SwingHigh" IsFloatPoint="true" RoundToTickSize="false">
      <BarsAgo>1</BarsAgo>
      <XmlIndicator Name="Swing" Type="Indicator" FullTypeName="AgenaTrader.Custom.Swing">
        <Params>
          <TAParam Name="Strength" IsProperty="true">
            <Value xsi:type="xsd:int">5</Value>
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
            <Value xsi:type="xsd:boolean">false</Value>
          </TAParam>
          <TAParam Name="IsShowPriceMarkers" IsProperty="true">
            <Value xsi:type="xsd:boolean">false</Value>
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
              <Plot Name="Swing high" PlotColor="0, 128, 0" Width="1" PlotStyle="Dot" PenStyle="Solid" />
              <Plot Name="Swing low" PlotColor="255, 69, 0" Width="1" PlotStyle="Dot" PenStyle="Solid" />
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
            <Value xsi:type="xsd:string">Swing(Close, 5)</Value>
          </TAParam>
          <TAParam Name="Tags" IsProperty="true" />
        </Params>
      </XmlIndicator>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Plus" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Plus" />
    <Lexems xsi:type="ScanLexemEol" />
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
              <Plot Name="ATR" PlotColor="0, 0, 255" Width="1" PlotStyle="Line" PenStyle="Solid" />
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
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Multiple" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="0.35" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <Lexems xsi:type="ScanLexemEol" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>true</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemCondition" Condition="And" />
  <Lexems xsi:type="ScanLexemEol" />
  <Lexems xsi:type="ScanLexemSerie" Type="Close">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo>0</BarsAgo>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="GreatEqual" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemIndicator" Serie="SwingHigh" IsFloatPoint="true" RoundToTickSize="false">
      <BarsAgo>1</BarsAgo>
      <XmlIndicator Name="Swing" Type="Indicator" FullTypeName="AgenaTrader.Custom.Swing">
        <Params>
          <TAParam Name="Strength" IsProperty="true">
            <Value xsi:type="xsd:int">5</Value>
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
            <Value xsi:type="xsd:boolean">false</Value>
          </TAParam>
          <TAParam Name="IsShowPriceMarkers" IsProperty="true">
            <Value xsi:type="xsd:boolean">false</Value>
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
              <Plot Name="Swing high" PlotColor="0, 128, 0" Width="1" PlotStyle="Dot" PenStyle="Solid" />
              <Plot Name="Swing low" PlotColor="255, 69, 0" Width="1" PlotStyle="Dot" PenStyle="Solid" />
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
            <Value xsi:type="xsd:string">Swing(Close, 5)</Value>
          </TAParam>
          <TAParam Name="Tags" IsProperty="true" />
        </Params>
      </XmlIndicator>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Plus" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemEol" />
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
              <Plot Name="ATR" PlotColor="0, 0, 255" Width="1" PlotStyle="Line" PenStyle="Solid" />
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
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Multiple" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="0.35" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <Lexems xsi:type="ScanLexemEol" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>true</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemEol" />
  <CalculateOnBarClose xsi:nil="true" />
  <ArrowSize>0.5</ArrowSize>
</ScanWizartState>
*/
#endregion