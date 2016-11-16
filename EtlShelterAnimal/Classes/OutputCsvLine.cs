namespace EtlShelterAnimal.Classes
{
    public class OutputCsvLine
    {
        public OutputCsvLine(InputData register)
        {
            HasName = register.HasName;
            DayPeriod = register.DayPeriod;
            WeekDay = register.WeekDay;
            Month = register.Month;
            IsHoliday = register.IsHoliday;
            Season = register.Season;
            AnimalType = register.AnimalType;
            Sex = register.Sex;
            Castrated = register.Castrated;
            DaysuponOutcome = register.DaysuponOutcome;
            IsMix = register.IsMix;
            BreedGroup = register.BreedGroup;
            IsPopularBreed = register.IsPopular;
            IsSingleColor = register.IsSingleColor;
            OutcomeType = register.OutcomeType;
        }

        public string HasName { get; set; }

        public string DayPeriod { get; set; }

        public string WeekDay { get; set; }

        public string Month { get; set; }

        public string IsHoliday { get; set; }

        public string Season { get; set; }

        public string AnimalType { get; set; }

        public string Sex { get; set; }

        public string Castrated { get; set; }

        public int DaysuponOutcome { get; set; }

        public string IsMix { get; set; }

        public string BreedGroup { get; set; }

        public string IsPopularBreed { get; set; }

        public string IsSingleColor { get; set; }

        public string OutcomeType { get; set; }
    }
}
