using System;
using System.Linq;
using System.Collections.Generic;

namespace fincalc
{
    public static class FinCalcs
    {

    	public static decimal TotalEstTax(decimal realizedGainST,
                                        decimal realizedGainLT,
                                        decimal dividends,
                                        decimal taxRateST,
                                        decimal taxRateLT,
                                        decimal divTaxRate)

	{
		var incomeTax = realizedGainST * taxRateST + realizedGainLT * taxRateLT;
		var dividendTax = dividends * divTaxRate;
		return incomeTax + dividendTax; 
	}

	public static decimal AfterTaxReturn(decimal pretaxReturn,
										 decimal timeWeightedMarketValue,
										 decimal realizedGainST,
										 decimal realizedGainLT,
										 decimal dividends,
										 decimal taxRateST,
										 decimal taxRateLT,
										 decimal divTaxRate)

	{
		var totTaxes = TotalEstTax(realizedGainST, realizedGainLT, dividends, taxRateST, taxRateLT, divTaxRate);
		return pretaxReturn - (totTaxes / timeWeightedMarketValue);
	}
	
	public static decimal TimeWeightedMarketValue(decimal beginningMV, List<Flow> flows) {
		decimal marketValue = beginningMV;
		foreach (var f in flows) {
			decimal daysInMonth = Calendar.DaysInMonth(f.EffectiveDate.Month,f.EffectiveDate.Year);
			decimal percentOfMonth = (daysInMonth - f.EffectiveDate.Day) / daysInMonth;
			marketValue += percentOfMonth * f.Amount;
		}
		return marketValue;
	}
	
	public static decimal LinkedReturns(List<decimal> periodReturns) {
		return LinkedReturns(periodReturns,0,periodReturns.Count-1);
	}
	
	public static decimal LinkedReturns(List<decimal> periodReturns, 
									   int startPeriod, 
									   int endPeriod) {
		decimal totalReturn = 1;
		for (int i = startPeriod; i <= endPeriod; i ++) {
			totalReturn = (1 + periodReturns.ElementAt(i)) * totalReturn;
		}
		return totalReturn - 1;
	}
	
	public static decimal LinkedReturns(List<decimal> returnToMonthEnd, 
									   decimal startingPeriodReturn, 
									   int startPeriod, 
									   int endPeriod) {
		return ((1 + startingPeriodReturn) * (returnToMonthEnd[startPeriod]/returnToMonthEnd[endPeriod]) - 1);
	}
	
	public static List<decimal> ReturnsToMonthEnd(List<decimal> periodReturns) {
		var cum = new decimal[periodReturns.Count + 1];
		var returns = periodReturns.Reverse<decimal>();
		int cumIdx = periodReturns.Count;
		decimal cumReturn = 1;
		decimal nextPeriodReturn = 0;
		for (int i=0; i < returns.Count(); i++) {
			cum[cumIdx] = cumReturn * (1 + nextPeriodReturn);
			cumReturn = cum[cumIdx];
			nextPeriodReturn = returns.ElementAt(i);
			cumIdx--;
			
		}
		return cum.Skip(1).ToList();
	}

    }
}
