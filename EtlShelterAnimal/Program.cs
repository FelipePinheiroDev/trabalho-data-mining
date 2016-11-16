﻿using CsvHelper;
using EtlShelterAnimal.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EtlShelterAnimal
{

    class Program
    {
        private static Dictionary<string, string> breedGroups = ReadBreedGroupsFile();

        static void Main(string[] args)
        {
            //args = new string[] { @"C:\Users\eduar\Documents\Git\EtlShelterAnimal\Data", @"C:\Users\eduar\Documents\Git\EtlShelterAnimal\Data" };
            string dest = args[0];
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

            //List<InputData> results = ReadTrainFile();
            //ExtractHolidays(results);
            //ExtractBreedGroup(results);
            //ExtractIsPopular(results);

            //string destPath = string.Concat(Path.Combine(dest, "train"), ".csv");
            //using (FileStream fs = new FileStream(destPath, FileMode.CreateNew))
            //using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("iso-8859-1")))
            //using (CsvWriter csvWriter = new CsvWriter(writer))
            //{
            //    List<OutputCsvLine> outputData = results.Select(register => new OutputCsvLine(register)).ToList();
            //    csvWriter.WriteRecords(outputData);
            //}

            //List<InputData> resultsTest = ReadTestFile();
            //ExtractHolidays(resultsTest);
            //ExtractBreedGroup(resultsTest);
            //ExtractIsPopular(resultsTest);

            //string destPathTest = string.Concat(Path.Combine(dest, "test"), ".csv");
            //using (FileStream fs = new FileStream(destPathTest, FileMode.CreateNew))
            //using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("iso-8859-1")))
            //using (CsvWriter csvWriter = new CsvWriter(writer))
            //{
            //    List<OutputCsvLine> outputData = resultsTest.Select(register => new OutputCsvLine(register)).ToList();
            //    csvWriter.WriteRecords(outputData);
            //}

            //List<InputData> testInput = ReadTestFile();
            //Dictionary<int, string> predictionsIndexedById = ReadPredictionFile().ToDictionary(prediction => prediction.ID, prediction => prediction.OutcomeTypePredicted);

            //List<Submission> submissions = new List<Submission>();
            //for (int i = 1; i <= testInput.Count; i++)
            //{
            //    submissions.Add(new Submission(i, predictionsIndexedById[i]));
            //}

            //WriteSubmissionFile(dest, submissions);
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

        private static void ExtractBreedGroup(List<InputData> results)
        {
            string value;
            string[] groups;
            foreach (InputData data in results)
            {
                if (!string.IsNullOrEmpty(data.Breed))
                {
                    value = Regex.Replace(data.Breed, "mix", string.Empty, RegexOptions.IgnoreCase).TrimEnd();
                    if (value.Contains("/"))
                    {
                        groups = value.Split('/');
                        if (breedGroups.ContainsKey(groups[0]))
                        {
                            data.BreedGroup = breedGroups[groups[0]];
                        }
                        else
                        {
                            data.BreedGroup = Constants.Unknown;
                        }
                        if (breedGroups.ContainsKey(groups[1]))
                        {
                            data.AlternativeDogGroup = breedGroups[groups[1]];
                        }
                        else
                        {
                            data.AlternativeDogGroup = Constants.Unknown;
                        }
                    }
                    else
                    {
                        if (breedGroups.ContainsKey(value))
                        {
                            data.BreedGroup = breedGroups[value];
                            data.AlternativeDogGroup = data.BreedGroup;
                        }
                        else
                        {
                            data.BreedGroup = Constants.Unknown;
                            data.AlternativeDogGroup = Constants.Unknown;
                        }
                    }
                }
            }
        }

        private static void ExtractIsPopular(List<InputData> results)
        {
            IPopularity catPopularity = new CatPopularity();
            IPopularity dogPopularity = new DogPopularity();

            foreach (InputData result in results)
            {
                int year = result.DateAndTime.Year;

                IPopularity popularity = result.AnimalType == "Cat" ? catPopularity : dogPopularity;

                bool isPopular = false;
                string breedWithouMix = result.Breed.Replace(" Mix", string.Empty);
                string[] breeds = breedWithouMix.Split('/');
                foreach (string breed in breeds)
                {
                    isPopular |= popularity.IsPopular(year, breed);
                }

                result.IsPopularBreed = isPopular ? "Yes" : "No";
            }
        }

        private static string GetSeason(DateTime date)
        {
            float value = (float)date.Month + date.Day / 100;
            if (value < 3.21 || value >= 12.22)
                return "Winter";
            if (value < 6.21)
                return "Spring";
            if (value < 9.23)
                return "Summer";
            return "Fall";
        }

        private static Dictionary<string, string> ReadBreedGroupsFile()
        {
            Dictionary<string, string> dogGroups = new Dictionary<string, string>();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\breedGroups.csv");
            using (CsvReader csv = new CsvReader(File.OpenText(path)))
            {
                csv.Configuration.Delimiter = ";";

                while (csv.Read())
                {
                    dogGroups.Add(csv.GetField<string>(0), csv.GetField<string>(1));
                }
            }

            return dogGroups;
        }

        private static List<Prediction> ReadPredictionFile()
        {
            List<Prediction> predictions = new List<Prediction>();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\prediction_by_weka.csv");
            using (CsvReader csv = new CsvReader(File.OpenText(path)))
            {
                while (csv.Read())
                {
                    predictions.Add(new Prediction
                    {
                        ID = csv.GetField<int>("inst#"),
                        OutcomeTypePredicted = csv.GetField<string>("predicted").Substring(2)
                    });
                }
            }

            return predictions;
        }

        private static List<InputData> ReadTestFile()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\test.csv");
            using (CsvReader csv = new CsvReader(File.OpenText(path)))
            {
                csv.Configuration.RegisterClassMap<TransformTest>();
                return csv.GetRecords<InputData>().ToList();
            }
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

        private static void WriteSubmissionFile(string destinationFolder, IEnumerable<Submission> submissions)
        {
            string destination = string.Concat(Path.Combine(destinationFolder, "submission"), ".csv");
            using (FileStream fs = new FileStream(destination, FileMode.CreateNew))
            using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("iso-8859-1")))
            using (CsvWriter csvWriter = new CsvWriter(writer))
            {
                csvWriter.WriteRecords(submissions);
            }
        }
    }
}