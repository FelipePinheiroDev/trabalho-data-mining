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
                return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
            }
        }
        public string Month
        {
            get
            {
                string input = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateAndTime.Month);
                return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
            }
        }
        public string IsHoliday { get; set; }
        public string Holiday { get; set; }
        public string Season { get; internal set; }
        public string BreedGroup { get; internal set; }
        public string AlternativeDogGroup { get; internal set; }

        /// <summary>
        /// If the breed of the animal is popular, then the value is "Yes", otherwise is "No".
        /// 
        /// For cats, the popular breeds are listed in http://cfa.org/. There is a list for years:
        /// 2015 - http://cfa.org/AboutCFA/News/PressReleases/PressRelease20160201Top10Breeds.aspx
        /// 2014 - http://cfa.org/AboutCFA/News/PressReleases/PressRelease20150216Top10Breeds.aspx
        /// 2013 - http://cfa.org/AboutCFA/News/PressReleases/PressRelease20140203Top10Breeds.aspx
        /// 2012 - http://www.cfa.org/Portals/0/documents/press/2012-popular-cat-breeds.pdf
        /// Years after 2015 will use the list for 2015.
        /// 
        /// For dogs, the popular breeds are listed in http://www.akc.org/. There is a list for years:
        /// 2015 - http://www.akc.org/news/the-most-popular-dog-breeds-in-america/
        /// 2014 - http://www.akc.org/news/the-most-popular-dog-breeds-in-america/
        /// 2013 - http://www.akc.org/news/the-most-popular-dog-breeds-in-america/
        /// 2012 - http://www.akc.org/press-center/most-popular-dog-breeds-in-america/
        /// Years after 2015 will use the list for 2015.
        /// </summary>
        public string IsPopularBreed { get; set; }
    }
}