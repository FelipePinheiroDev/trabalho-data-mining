using CsvHelper;
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

            List<InputData> results = ReadTrainFile();
            ExtractHolidays(results);
            ExtractBreedGroup(results);
            ExtractIsPopular(results);

            string destPath = string.Concat(Path.Combine(dest, "train"), ".csv");
            using (FileStream fs = new FileStream(destPath, FileMode.CreateNew))
            using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("iso-8859-1")))
            using (CsvWriter csvWriter = new CsvWriter(writer))
            {
                List<OutputCsvLine> outputData = results.Select(register => new OutputCsvLine(register)).ToList();
                csvWriter.WriteRecords(outputData);
            }
        }

        private static IEnumerable<List<InputData>> DivideForCrossValidation(IEnumerable<InputData> data, int numberOfSubsets)
        {
            List<List<InputData>> crossValidationSubsets = new List<List<InputData>>();

            Random random = new Random();

            // Separar por classes
            List<List<InputData>> dataDividedByClass = data.ToLookup(register => register.OutcomeType).Select(classGroup => classGroup.ToList()).ToList();

            Dictionary<int, int> numberOfElementsIndexedByIndex = new Dictionary<int, int>();
            for (int i = 0; i < dataDividedByClass.Count; i++)
            {
                numberOfElementsIndexedByIndex.Add(i, dataDividedByClass[i].Count);
            }

            for (int i = 0; i < numberOfSubsets; i++)
            {
                List<InputData> crossValidationSubset = new List<InputData>();

                for (int j = 0; j < dataDividedByClass.Count; j++)
                {
                    List<InputData> classList = dataDividedByClass[j];

                    int added = 0;
                    while (added < (numberOfElementsIndexedByIndex[j] / numberOfSubsets) && classList.Any())
                    {
                        int index = random.Next(classList.Count);
                        crossValidationSubset.Add(classList[index]);
                        classList.RemoveAt(index);

                        added++;
                    }
                }

                crossValidationSubsets.Add(crossValidationSubset);
            }

            foreach (List<InputData> classList in dataDividedByClass)
            {
                while (classList.Any())
                {
                    int crossValidationSubsetNumber = random.Next(numberOfSubsets);
                    crossValidationSubsets[crossValidationSubsetNumber].Add(classList[0]);
                    classList.RemoveAt(0);
                }
            }

            return crossValidationSubsets;
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

        private static List<InputData> ReadTrainFile()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\train.csv");
            using (CsvReader csv = new CsvReader(File.OpenText(path)))
            {
                csv.Configuration.RegisterClassMap<Transform>();
                return csv.GetRecords<InputData>().ToList();
            }
        }

        private static void WriteCrossValidationFiles(List<List<InputData>> crossValidationSubsets, string destinationFolder)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < crossValidationSubsets.Count; i++)
            {
                indexes.Add(i);
            }

            for (int i = 0; i < crossValidationSubsets.Count; i++)
            {
                string trainDestinationPath = string.Concat(Path.Combine(destinationFolder, string.Concat("train-", i)), ".csv");

                using (FileStream fileStream = new FileStream(trainDestinationPath, FileMode.CreateNew))
                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding("iso-8859-1")))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter))
                {
                    List<InputData> test = crossValidationSubsets[i];
                    List<OutputCsvLine> train = crossValidationSubsets.Where(subset => !subset.Equals(test)).SelectMany(data => data).ToList().Select(data => new OutputCsvLine(data)).ToList();

                    csvWriter.WriteRecords(train);
                }

                string testDestinationPath = string.Concat(Path.Combine(destinationFolder, string.Concat("test-", i)), ".csv");

                using (FileStream fileStream = new FileStream(testDestinationPath, FileMode.CreateNew))
                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding("iso-8859-1")))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter))
                {
                    List<OutputCsvLine> test = crossValidationSubsets[i].Select(data => new OutputCsvLine(data)).ToList();

                    csvWriter.WriteRecords(test);
                }
            }
        }
    }
}