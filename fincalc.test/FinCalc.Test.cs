using System;
using NUnit.Framework;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using fincalc;

namespace fincalc.test
{
    public class Tests
    {
        List<Return> ru1000 = null;
        List<Return> sp500 = null;
        
        [SetUp]
        public void SetUp() {
            ru1000 = JsonSerializer.Deserialize<List<Return>>(
		        File.ReadAllText(@"ru1000.json"));
            sp500 = JsonSerializer.Deserialize<List<Return>>(
		        File.ReadAllText(@"sp500.json"));
        }

        [Test]
        public void CanGetDaysInMonth()
        {
            Assert.AreEqual(31,Calendar.DaysInMonth(1,2020));
        }

        [Test]
        public void CanCalculateLinkedReturnsFromList() {
            var janreturnRU1000 = 
                FinCalcs.LinkedReturns(
                    ru1000
                        .Where(r=>r.EffectiveDate.Month == 1)
                        .Select(r=>r.Rate)
                        .ToList());
            Assert.AreEqual(-0.000101m,Math.Round(janreturnRU1000,6));
        }

        [Test]
        public void CanCalculatePartialMonthReturns() {
            var janreturnSP500 = 
                FinCalcs.LinkedReturns(
                    sp500
                        .Where(
                            r=>r.EffectiveDate.Month == 1 &&
                               r.EffectiveDate.Day >= 15
                        )
                        .Select(r=>r.Rate)
                        .ToList());
            Assert.AreEqual(-0.017554m,Math.Round(janreturnSP500,6));
        }

        [Test]
        public void CanGetWeekDaysInMonth() {
            Assert.AreEqual(20,Calendar.WeekdaysInMonth(2,2020));
        }

        [Test]
        public void CanGetTimeWeightedMarketValue() {
            decimal startingMarketValue = 1000m;           
            var flows = new List<Flow> {
                new Flow { Type = Flow.FlowType.Cash, 
                           Amount = 100m, EffectiveDate = new DateTime(2020,1,9) },
                new Flow { Type = Flow.FlowType.Cash, 
                           Amount = -100m, EffectiveDate = new DateTime(2020,1,22) },
            };
	        var wgtMktVal = FinCalcs.TimeWeightedMarketValue(startingMarketValue,flows);
            Assert.AreEqual(1042.00m,Math.Round(wgtMktVal));
        }

        [Test]
        public void CanCalculateAfterTaxReturn() {
            decimal pretaxReturn = 0.05m;
            decimal returnAT = FinCalcs.AfterTaxReturn(
                pretaxReturn: pretaxReturn,
                timeWeightedMarketValue: 1042m,
                realizedGainST: 10m,
                realizedGainLT: 50m,
                dividends: 10m,
                taxRateST: .35m,
                taxRateLT: .15m,
                divTaxRate: .15m
            );
            Assert.AreEqual(0.038004m,Math.Round(returnAT,6));
        }

        [Test]
        public void CanCalculateReturnToMonthEnd() {
            var cumReturns = FinCalcs.ReturnsToMonthEnd(
                sp500
                    .Where(r=>r.EffectiveDate.Month == 1)
                    .Select(r=>r.Rate)
                    .ToList()
            );

            Assert.AreEqual(0.990074m,Math.Round(cumReturns.First(),6));
            Assert.AreEqual(1m,Math.Round(cumReturns.Last()));

        }

        [Test]
        public void CanCalculateLinkedReturnsWithCumReturnsMonthToEnd() {
            var janSP500returns = sp500
                    .Where(r=>r.EffectiveDate.Month == 1)
                    .Select(r=>r.Rate)
                    .ToList();

            var cumReturns = FinCalcs.ReturnsToMonthEnd(janSP500returns);            

            var longCalc = FinCalcs.LinkedReturns(
                                periodReturns: janSP500returns,
                                startPeriod: 0,
                                endPeriod: janSP500returns.Count()-1);

            var quickCalc = FinCalcs.LinkedReturns(
                returnToMonthEnd: cumReturns,
                startingPeriodReturn: janSP500returns.ElementAt(0),
                startPeriod: 0,
                endPeriod: janSP500returns.Count()-1
            );

            Assert.AreEqual(
                Math.Round(longCalc,6),
                Math.Round(quickCalc,6));
        }

    }
}