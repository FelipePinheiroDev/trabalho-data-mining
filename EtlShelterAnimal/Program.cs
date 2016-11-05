using CsvHelper;
using EtlShelterAnimal.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EtlShelterAnimal
{
    class Program
    {
        static void Main(string[] args)
        {
            string dest = args[1];
            if (string.IsNullOrEmpty(dest))
            {
                Console.WriteLine("A sintaxe do comando está incorreta");
                return;
            }
            if (!Directory.Exists(dest))
            {
                Console.WriteLine("Diretório de destino não encontrado");
                return;
            }

            List<InputData> results = ReadTrainFile().Where(register => register.Sex != "Unknow").ToList();

            ExtractHolidays(results);

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
                data.IsHoliday = 0;
                data.Season = GetSeason(data.DateAndTime);
                foreach (Holiday day in holidays)
                {
                    if (data.DateAndTime >= day.HolidayDay.AddDays(-7) && data.DateAndTime <= day.HolidayDay)
                    {
                        data.Holiday = day.HolidayName;
                        data.IsHoliday = 1;
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

        private static List<InputData> ReadTrainFile()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\train.csv");
            using (CsvReader csv = new CsvReader(File.OpenText(path)))
            {
                csv.Configuration.RegisterClassMap<Transform>();
                return csv.GetRecords<InputData>().ToList();
            }
        }
    }
}