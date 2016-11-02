using System;
using System.Globalization;
using System.Linq;

namespace EtlShelterAnimal.Classes
{
    public class InputData
    {
        

        public DateTime DateAndTime { get; set; }
        public string OutcomeType { get; set; }
        public string AnimalType { get; set; }
        public string Sex { get; set; }
        public int Castrated { get; set; }
        public string Breed { get; set; }
        public string Color { get; set; }
        public int DaysuponOutcome { get; set; }
        public int HasName { get; set; }
        public int IsMix { get; set; }
        public int IsSingleColor { get; set; }
        public string DayPeriod { get; set; }
        public string WeekDay
        {
            get
            {
                string input = DateAndTime.ToString("ddd", CultureInfo.CurrentCulture);
                return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
            }
        }
        public string Month
        {
            get
            {
                string input = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateAndTime.Month);
                return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
            }
        }
        public int IsHoliday { get; set; }
        public string Holiday { get; set; }
        public string Season { get; internal set; }
    }
}