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
	[UsedCalculationUnit(typeof(global::AgenaTrader.Custom.VOL), typeof(global::AgenaTrader.Custom.VOLMA))]
	[ScanTypeAttribute(ScanType.Short)]
	[IsEntryAttribute(false)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[Category("Volume")]
	[Description("Detects unusual high volume")]
	public class UnusualLowVolume : UserScan
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
			BarHighlightColor = Color.FromArgb(255, 51, 102, 255);
		}

		protected override void OnCalculate()
		{
			LastOutput = LeadIndicator.VOL(MultiBars.GetBarsItem(TimeFrame).Close)[0] <= ((LeadIndicator.VOLMA(MultiBars.GetBarsItem(TimeFrame).Close, 20)[0]*0.6));
		}
	}
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manually
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="false" ScanType="Short" IsEntry="false" IsStop="false" IsTarget="false" Category="Volume" Description="Detects unusual high volume" ArrowColor="" TextColor="" ScanColor="" BarHighlightColor="51, 102, 255">
  <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
    <BarsAgo>0</BarsAgo>
    <XmlIndicator Name="VOL" Type="Indicator" FullTypeName="AgenaTrader.Custom.VOL">
      <Params>
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
            <Plot Name="Volume" PlotColor="Green" Width="2" PlotStyle="Bar" PenStyle="Solid" />
          </Value>
        </TAParam>
        <TAParam Name="XmlLines" IsProperty="true">
          <Value xsi:type="ArrayOfLevelLine">
            <LevelLine Name="Mid line" XmlColor="Blue" Value="0" Width="1" DashStyle="Solid" />
          </Value>
        </TAParam>
        <TAParam Name="FixedInputParameters" IsProperty="true">
          <Value xsi:type="xsd:unsignedByte">0</Value>
        </TAParam>
        <TAParam Name="OutputsDisplayStyle" IsProperty="true">
          <Value xsi:type="SuffixedPriceDisplayStyle" />
        </TAParam>
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
          <Value xsi:type="xsd:string">VOL(Close)</Value>
        </TAParam>
        <TAParam Name="Tags" IsProperty="true" />
      </Params>
    </XmlIndicator>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="LessEqual" />
  <Lexems xsi:type="ScanLexemCalculation" RoundToTickSize="false">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemIndicator" Serie="" IsFloatPoint="true" RoundToTickSize="false">
      <BarsAgo>0</BarsAgo>
      <XmlIndicator Name="VOLMA" Type="Indicator" FullTypeName="AgenaTrader.Custom.VOLMA">
        <Params>
          <TAParam Name="Period" IsProperty="true">
            <Value xsi:type="xsd:int">20</Value>
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
              <Plot Name="VOLMA" PlotColor="Blue" Width="1" PlotStyle="Line" PenStyle="Solid" />
            </Value>
          </TAParam>
          <TAParam Name="XmlLines" IsProperty="true">
            <Value xsi:type="ArrayOfLevelLine" />
          </TAParam>
          <TAParam Name="FixedInputParameters" IsProperty="true">
            <Value xsi:type="xsd:unsignedByte">0</Value>
          </TAParam>
          <TAParam Name="OutputsDisplayStyle" IsProperty="true">
            <Value xsi:type="SuffixedPriceDisplayStyle" />
          </TAParam>
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
            <Value xsi:type="xsd:string">VOLMA(Close, 14)</Value>
          </TAParam>
          <TAParam Name="Tags" IsProperty="true" />
        </Params>
      </XmlIndicator>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Multiple" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="0.6" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <CalculateOnBarClose xsi:nil="true" />
  <ArrowSize>0.5</ArrowSize>
</ScanWizartState>
*/
#endregion
