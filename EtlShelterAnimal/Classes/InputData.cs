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
        public string Castrated { get; set; }
        public string Breed { get; set; }
        public string Color { get; set; }
        public int DaysuponOutcome { get; set; }
        public string HasName { get; set; }
        public string IsMix { get; set; }
        public string IsSingleColor { get; set; }
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
        public string IsHoliday { get; set; }
        public string Holiday { get; set; }
        public string Season { get; internal set; }
        public string DogGroup { get; internal set; }
        public string AlternativeDogGroup { get; internal set; }
    }
}