using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlShelterAnimal.Classes
{
    public static class HolidaySearch
    {
        public static List<Holiday> GetHolidays(int year)
        {
            List<Holiday> holidays = new List<Holiday>();

            //New Year
            DateTime newYearsDate = AdjustForWeekendHoliday(new DateTime(year, 1, 1).Date);
            holidays.Add(new Holiday() { HolidayName = "New Year", HolidayDay = newYearsDate });

            //Memorial Day - último dia de maio
            DateTime memorialDay = new DateTime(year, 5, 31);
            DayOfWeek dayOfWeek = memorialDay.DayOfWeek;
            while (dayOfWeek != DayOfWeek.Monday)
            {
                memorialDay = memorialDay.AddDays(-1);
                dayOfWeek = memorialDay.DayOfWeek;
            }
            holidays.Add(new Holiday() { HolidayName = "Memorial Day", HolidayDay = memorialDay.Date });

            //Independence Day
            DateTime independenceDay = AdjustForWeekendHoliday(new DateTime(year, 7, 4).Date);
            holidays.Add(new Holiday() { HolidayName = "Independence Day", HolidayDay = independenceDay });

            //Labor Day - primeira segunda de setembro
            DateTime laborDay = new DateTime(year, 9, 1);
            dayOfWeek = laborDay.DayOfWeek;
            while (dayOfWeek != DayOfWeek.Monday)
            {
                laborDay = laborDay.AddDays(1);
                dayOfWeek = laborDay.DayOfWeek;
            }
            holidays.Add(new Holiday() { HolidayName = "Labor Day", HolidayDay = laborDay.Date });

            //Thanksgiving - 4ª quinta de Novembro
            var thanksgiving = (from day in Enumerable.Range(1, 30)
                                where new DateTime(year, 11, day).DayOfWeek == DayOfWeek.Thursday
                                select day).ElementAt(3);
            DateTime thanksgivingDay = new DateTime(year, 11, thanksgiving);
            holidays.Add(new Holiday() { HolidayName = "Thanksgiving", HolidayDay = thanksgivingDay.Date });

            //Christmas
            DateTime christmasDay = AdjustForWeekendHoliday(new DateTime(year, 12, 25).Date);
            holidays.Add(new Holiday() { HolidayName = "Christmas", HolidayDay = christmasDay });
            return holidays;
        }

        public static DateTime AdjustForWeekendHoliday(DateTime holiday)
        {
            if (holiday.DayOfWeek == DayOfWeek.Saturday)
            {
                return holiday.AddDays(-1);
            }
            else if (holiday.DayOfWeek == DayOfWeek.Sunday)
            {
                return holiday.AddDays(1);
            }
            else
            {
                return holiday;
            }
        }
    }
}
