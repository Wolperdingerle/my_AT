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
	[ScanTypeAttribute(ScanType.Long)]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[Category("ReversalBars")]
	[Description("Long ReversalBar with long tail and close of the bar is greater than the open of the bar")]
	public class ReversalLong_GreenBar : UserScan
	{
		protected override void OnInit()
		{
			ScanType = ScanType.Long;

			IsEntry = true;
			IsStop = false;
			IsTarget = false;

			ArrowColor = Color.LawnGreen;
			BarHighlightColor = null;
		}

		protected override void OnCalculate()
		{
			LastOutput = MultiBars.GetBarsItem(TimeFrame).Open[0] < MultiBars.GetBarsItem(TimeFrame).Close[0]
				&& ((MultiBars.GetBarsItem(TimeFrame).Open[0]-MultiBars.GetBarsItem(TimeFrame).Low[0])) > ((MultiBars.GetBarsItem(TimeFrame).High[0]-MultiBars.GetBarsItem(TimeFrame).Open[0]))
				&& ((MultiBars.GetBarsItem(TimeFrame).High[0]-MultiBars.GetBarsItem(TimeFrame).Open[0])) < (((MultiBars.GetBarsItem(TimeFrame).High[0]-MultiBars.GetBarsItem(TimeFrame).Low[0])*(0.45*1)));
		}
	}

	#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

	public partial class UserScan
	{
		public ReversalLong_GreenBar ReversalLong_GreenBar()
		{
			return GetCondition<ReversalLong_GreenBar>();
		}

		public ReversalLong_GreenBar ReversalLong_GreenBar(IDataSeries input)
		{
			return GetCondition<ReversalLong_GreenBar>(input);
		}
	}

	#endregion
}

#region XML. Generated by ScanningEscort Tool. Do not change or remove this region manualy
/*
<?xml version="1.0" encoding="utf-16"?>
<ScanWizartState xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Advanved="false" ScanType="Long" IsEntry="true" IsStop="false" IsTarget="false" Category="ReversalBars" Description="Long ReversalBar with long tail and close of the bar is greater than the open of the bar" ArrowColor="LawnGreen" BarHighlightColor="">
  <Lexems xsi:type="ScanLexemSerie" Type="Open">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo>0</BarsAgo>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Less" />
  <Lexems xsi:type="ScanLexemSerie" Type="Close">
    <TimeFrame>0 Min</TimeFrame>
    <BarsAgo>0</BarsAgo>
  </Lexems>
  <Lexems xsi:type="ScanLexemCondition" Condition="And" />
  <Lexems xsi:type="ScanLexemCalculation">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemSerie" Type="Open">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemSerie" Type="Low">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Great" />
  <Lexems xsi:type="ScanLexemCalculation">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemSerie" Type="High">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemSerie" Type="Open">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemCondition" Condition="And" />
  <Lexems xsi:type="ScanLexemCalculation">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemSerie" Type="High">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemSerie" Type="Open">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>false</IsAdvanced>
  </Lexems>
  <Lexems xsi:type="ScanLexemOperation" Operation="Less" />
  <Lexems xsi:type="ScanLexemCalculation">
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemSerie" Type="High">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Minus" />
    <Lexems xsi:type="ScanLexemSerie" Type="Low">
      <TimeFrame>0 Min</TimeFrame>
      <BarsAgo>0</BarsAgo>
    </Lexems>
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Multiple" />
    <Lexems xsi:type="ScanLexemEol" />
    <Lexems xsi:type="ScanLexemOpenBracket" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="0.45" CharValue="0" />
    <Lexems xsi:type="ScanLexemArifmeticOperation" Operation="Multiple" />
    <Lexems xsi:type="ScanLexemValue" LexemType="Double" Value="1" CharValue="0" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <Lexems xsi:type="ScanLexemEol" />
    <Lexems xsi:type="ScanLexemCloseBracket" />
    <IsAdvanced>true</IsAdvanced>
  </Lexems>
  <CalculateOnBarClose xsi:nil="true" />
</ScanWizartState>
*/
#endregion
