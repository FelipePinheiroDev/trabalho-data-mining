using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlShelterAnimal.Classes
{

    public sealed class Transform : CsvClassMap<InputData>
    {
        public Transform()
        {
            Map(m => m.DateAndTime).Index(2);
            Map(m => m.OutcomeType).Index(3);
            Map(m => m.AnimalType).Index(5);
            Map(m => m.Breed).Index(8);
            Map(m => m.Color).Index(9);
            Map(m => m.DaysuponOutcome).ConvertUsing(row =>
            {
                string rowValue = row.GetField<string>(7);
                int mult = 0;
                int value = 0;
                if (!string.IsNullOrEmpty(rowValue))
                {
                    string[] values = rowValue.Split(' ');
                    value = int.Parse(values[0]);
                    switch (values[1])
                    {
                        case "months":
                            {
                                mult = 30;
                                break;
                            }
                        case "years":
                            {
                                mult = 365;
                                break;
                            }
                        case "year":
                            {
                                mult = 365;
                                break;
                            }
                        case "weeks":
                            {
                                mult = 7;
                                break;
                            }
                        case "week":
                            {
                                mult = 7;
                                break;
                            }
                        case "month":
                            {
                                mult = 30;
                                break;
                            }
                        case "days":
                            {
                                mult = 1;
                                break;
                            }
                    }
                }

                return mult * value;
            });
            Map(m => m.HasName).ConvertUsing(r => string.IsNullOrEmpty(r.GetField<string>(1)) ? 0 : 1);
            Map(m => m.DayPeriod).ConvertUsing(r =>
            {
                int hour = r.GetField<DateTime>(2).Hour;
                if (hour > 5 & hour <= 12)
                    return "Manhã";
                if (hour > 12 & hour <= 18)
                    return "Tarde";
                return "Noite";
            });
            Map(m => m.IsMix).ConvertUsing(r =>
            {
                string value = r.GetField<string>(8);
                int mix = 0;
                if (value.IndexOf("mix", StringComparison.OrdinalIgnoreCase) >= 0)
                    mix = 1;
                return mix;
            });
            Map(m => m.Castrated).ConvertUsing(r =>
            {
                string value = r.GetField<string>(6);
                int castrated = 0;
                if (value.IndexOf("intact", StringComparison.OrdinalIgnoreCase) >= 0)
                    castrated = 0;
                if (value.IndexOf("neutered", StringComparison.OrdinalIgnoreCase) >= 0)
                    castrated = 1;
                if (value.IndexOf("spayed", StringComparison.OrdinalIgnoreCase) >= 0)
                    castrated = 1;
                return castrated;
            });
            Map(m => m.Sex).ConvertUsing(r =>
            {
                string value = r.GetField<string>(6);
                if (value.IndexOf("female", StringComparison.OrdinalIgnoreCase) >= 0)
                    return "Female";
                if (value.IndexOf("male", StringComparison.OrdinalIgnoreCase) >= 0)
                    return "Male";
                return "Unknow";
            });
            Map(m => m.IsSingleColor).ConvertUsing(r =>
            {
                string value = r.GetField<string>(9);
                if (!string.IsNullOrEmpty(value))
                {
                    var colors = value.Split('/');
                    if (colors.Length > 1)
                    {
                        return 0;
                    }
                }
                return 1;
            });
        }
    }
}
