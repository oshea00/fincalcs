using System;

namespace fincalc
{
    public static class Calendar {
	public static int WeekdaysInMonth(int month, int year) {
		var day1 = new DateTime(year,month,1);
		var daysInMonth = DaysInMonth(month,year);
		var days = 0;
		for (int d = 0; d < daysInMonth; d++) {
			var currDOW = day1.AddDays(d).DayOfWeek;
			if (currDOW == DayOfWeek.Saturday || currDOW == DayOfWeek.Sunday)
				continue;
			days++;
		}
		return days;
	}

	public static int DaysInMonth(int month, int year)
	{
		var day1 = new DateTime(year, month, 1);
		var daysInMonth = day1.AddMonths(1).AddDays(-1).Day;
		return daysInMonth;
	}
}

}