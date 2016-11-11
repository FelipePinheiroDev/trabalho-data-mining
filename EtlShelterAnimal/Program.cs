using CsvHelper;
using EtlShelterAnimal.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EtlShelterAnimal
{

    class Program
    {
        const string group = "dogGroups.csv";
        const string train = "train.csv";
        const string unknow = "Unknow";
        public static Dictionary<string, string> dogGroups;

        static void Main(string[] args)
        {
            string trainPath = args[0] + train;
            string dogGroupPatch = args[0] + group;
            string dest = args[1];
            if (string.IsNullOrEmpty(trainPath) || string.IsNullOrEmpty(dest))
            {
                Console.WriteLine("A sintaxe do comando está incorreta");
                return;
            }
            if (!File.Exists(trainPath))
            {
                Console.WriteLine("Caminho de arquivo não encontrado");
                return;
            }
            if (!Directory.Exists(dest))
            {
                Console.WriteLine("Diretório de destino não encontrado");
                return;
            }


            List<InputData> results = new List<InputData>();
            List<string> erros = new List<string>();

            using (TextReader reader = File.OpenText(dogGroupPatch))
            {
                CsvReader csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                dogGroups = new Dictionary<string, string>();
                while (csv.Read())
                {
                    dogGroups.Add(csv.GetField<string>(0), csv.GetField<string>(1));
                }
            }

            using (TextReader reader = File.OpenText(trainPath))
            {
                CsvReader csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ",";
                csv.Configuration.RegisterClassMap<Transform>();
                int line = 1;
                while (csv.Read())
                {
                    line++;
                    try
                    {
                        string hasKnowSex = csv.GetField<string>(6);
                        if (!string.IsNullOrEmpty(hasKnowSex) && hasKnowSex.IndexOf("unknown", StringComparison.OrdinalIgnoreCase) == 0)
                            continue;
                        results.Add(csv.GetRecord<InputData>()); //12/02/2014 18:22 
                    }
                    catch (Exception e)
                    {
                        erros.Add(string.Format("Erro na linha {0}: {1}", line, e.Message));
                    }
                }
                Console.WriteLine(string.Format("{0} linhas encontradas. {1} registros importados. {2} erros", line - 1, results.Count, erros.Count));
            }

            ExtractHolidays(results);
            ExtractDogGroup(results);

            string destPath = Path.Combine(dest, DateTime.Now.Ticks.ToString()) + ".csv";
            FileStream fs = new FileStream(destPath, FileMode.CreateNew);

            using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("iso-8859-1")))
            {
                var csvWriter = new CsvWriter(writer);
                csvWriter.Configuration.CultureInfo = CultureInfo.CurrentCulture;
                csvWriter.Configuration.Delimiter = ";";
                csvWriter.WriteRecords(results);
            }
        }

        private static void ExtractHolidays(List<InputData> results)
        {
            List<int> years = results.Select(r => r.DateAndTime.Year).Distinct().ToList();
            List<Holiday> holidays = years.SelectMany(h => HolidaySearch.GetHolidays(h)).ToList();
            foreach (InputData data in results)
            {
                data.Holiday = "Common Day";
                data.IsHoliday = "No";
                data.Season = GetSeason(data.DateAndTime);
                foreach (Holiday day in holidays)
                {
                    if (data.DateAndTime >= day.HolidayDay.AddDays(-7) && data.DateAndTime <= day.HolidayDay)
                    {
                        data.Holiday = day.HolidayName;
                        data.IsHoliday = "Yes";
                    }
                }
            }
        }

        private static void ExtractDogGroup(List<InputData> results)
        {
            string value;
            string[] groups;
            foreach (InputData data in results)
            {
                if (!string.IsNullOrEmpty(data.Breed) && data.AnimalType.Equals("Dog"))
                {
                    value = Regex.Replace(data.Breed, "mix", string.Empty, RegexOptions.IgnoreCase).TrimEnd();
                    if (value.Contains("/"))
                    {
                        groups = value.Split('/');
                        if (dogGroups.ContainsKey(groups[0]))
                        {
                            data.DogGroup = dogGroups[groups[0]];
                        }
                        else
                        {
                            data.DogGroup = unknow;
                        }
                        if (dogGroups.ContainsKey(groups[1]))
                        {
                            data.AlternativeDogGroup = dogGroups[groups[1]];
                        }
                        else
                        {
                            data.AlternativeDogGroup = unknow;
                        }
                    }
                    else
                    {
                        if (dogGroups.ContainsKey(value))
                        {
                            data.DogGroup = dogGroups[value];
                            data.AlternativeDogGroup = data.DogGroup;
                        }
                        else
                        {
                            data.DogGroup = unknow;
                            data.AlternativeDogGroup = unknow;
                        }
                    }
                }
            }
        }

        private static string GetSeason(DateTime date)
        {
            float value = (float)date.Month + date.Day / 100;
            if (value < 3.21 || value >= 12.22)
                return "Inverno";
            if (value < 6.21)
                return "Primavera";
            if (value < 9.23)
                return "Verão";
            return "Outuno";
        }
    }
}//