using System.Collections.Generic;
using System.Linq;

namespace EtlShelterAnimal.Classes
{
    public class DogPopularity : IPopularity
    {
        private Dictionary<int, List<string>> popularDogBreedsIndexedByYear;

        public DogPopularity()
        {
            popularDogBreedsIndexedByYear = new Dictionary<int, List<string>>();
            popularDogBreedsIndexedByYear.Add(2012, PopularDogBreedsFrom2012().ToList());
            popularDogBreedsIndexedByYear.Add(2013, PopularDogBreedsFrom2013().ToList());
            popularDogBreedsIndexedByYear.Add(2014, PopularDogBreedsFrom2014().ToList());
            popularDogBreedsIndexedByYear.Add(2015, PopularDogBreedsFrom2015().ToList());
        }

        public bool IsPopular(int year, string breed)
        {
            if (year < 2012) year = 2012;
            else if (year > 2015) year = 2015;

            return popularDogBreedsIndexedByYear[year].Contains(breed);
        }

        private static IEnumerable<string> PopularDogBreedsFrom2015()
        {
            return new List<string>
            {
                "Labrador Retriever",
                "German Shepherd",
                "Golden Retriever",
                "American Bulldog",
                "Bulldog",
                "English Bulldog",
                "Beagle",
                "French Bulldog",
                "Yorkshire",
                "Yorkshire Terrier",
                "Miniature Poodle",
                "Standard Poodle",
                "Toy Poodle",
                "Rottweiler",
                "Boxer"
            };
        }

        private static IEnumerable<string> PopularDogBreedsFrom2014()
        {
            return new List<string>
            {
                "Labrador Retriever",
                "German Shepherd",
                "Golden Retriever",
                "American Bulldog",
                "Bulldog",
                "English Bulldog",
                "Beagle",
                "Yorkshire",
                "Yorkshire Terrier",
                "Miniature Poodle",
                "Standard Poodle",
                "Toy Poodle",
                "Rottweiler",
                "Boxer",
                "French Bulldog",
                "Rottweiler",
            };
        }

        private static IEnumerable<string> PopularDogBreedsFrom2013()
        {
            return new List<string>
            {
                "Labrador Retriever",
                "German Shepherd",
                "Golden Retriever",
                "Beagle",
                "American Bulldog",
                "Bulldog",
                "English Bulldog",
                "Yorkshire",
                "Yorkshire Terrier",
                "Boxer",
                "Miniature Poodle",
                "Standard Poodle",
                "Toy Poodle",
                "French Bulldog",
                "Dachshund",
                "Dachshund Longhair",
                "Dachshund Wirehair"
            };
        }

        private static IEnumerable<string> PopularDogBreedsFrom2012()
        {
            return new List<string>
            {
                "Labrador Retriever",
                "German Shepherd",
                "Golden Retriever",
                "Beagle",
                "American Bulldog",
                "Bulldog",
                "English Bulldog",
                "Yorkshire",
                "Yorkshire Terrier",
                "Boxer",
                "Miniature Poodle",
                "Standard Poodle",
                "Toy Poodle",
                "French Bulldog",
                "Dachshund",
                "Dachshund Longhair",
                "Dachshund Wirehair"
            };
        }
    }
}
