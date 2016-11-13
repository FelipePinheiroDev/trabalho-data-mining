using System.Collections.Generic;
using System.Linq;

namespace EtlShelterAnimal.Classes
{
    public class CatPopularity : IPopularity
    {
        private Dictionary<int, List<string>> popularCatBreedsIndexedByYear;

        public CatPopularity()
        {
            popularCatBreedsIndexedByYear = new Dictionary<int, List<string>>();
            popularCatBreedsIndexedByYear.Add(2012, PopularCatBreedsFrom2012().ToList());
            popularCatBreedsIndexedByYear.Add(2013, PopularCatBreedsFrom2013().ToList());
            popularCatBreedsIndexedByYear.Add(2014, PopularCatBreedsFrom2014().ToList());
            popularCatBreedsIndexedByYear.Add(2015, PopularCatBreedsFrom2015().ToList());
        }

        public bool IsPopular(int year, string breed)
        {
            if (year < 2012) year = 2012;
            else if (year > 2015) year = 2015;

            return popularCatBreedsIndexedByYear[year].Contains(breed);
        }

        private static IEnumerable<string> PopularCatBreedsFrom2012()
        {
            return new List<string>
            {
                "Persian",
                "Exotic",
                "Maine Coon",
                "Ragdoll",
                "Abyssinian",
                "Sphynx",
                "American Shorthair",
                "British Shorthair",
                "Siamese",
                "Devon Rex",
                "Oriental",
                "Cornish Rex",
                "Norwegian Forest Cat",
                "Scottish Fold",
                "Birman",
                "Tonkinese",
                "Burmese",
                "Siberian",
                "Russian Blue",
                "Ocicat",
                "Egyptian Mau",
                "Japanese Bobtail",
                "Selkirk Rex",
                "Turkish Angora",
                "Singapura"
            };
        }

        private static IEnumerable<string> PopularCatBreedsFrom2013()
        {
            return new List<string>
            {
                "Persian",
                "Exotic",
                "Maine Coon",
                "Ragdoll",
                "British Shorthair",
                "Abyssinian",
                "American Shorthair",
                "Sphynx",
                "Siamese",
                "Devon Rex",
                "Norwegian Forest Cat",
                "Oriental",
                "Scottish Fold",
                "Cornish Rex",
                "Birman",
                "Burmese",
                "Tonkinese",
                "Siberian",
                "Russian Blue",
                "Egyptian Mau",
                "Ocicat",
                "Japanese Bobtail",
                "Turkish Angora",
                "Manx",
                "Singapura"
            };
        }

        private static IEnumerable<string> PopularCatBreedsFrom2014()
        {
            return new List<string>
            {
                "Exotic",
                "Persian",
                "Maine Coon",
                "Ragdoll",
                "British Shorthair",
                "American Shorthair",
                "Abyssinian",
                "Sphynx",
                "Siamese",
                "Scottish Fold",
                "Cornish Rex",
                "Devon Rex",
                "Oriental",
                "Norwegian Forest Cat",
                "Birman",
                "Burmese",
                "Siberian",
                "Tonkinese",
                "Russian Blue",
                "Egyptian Mau",
                "Ocicat",
                "Singapura",
                "Manx",
                "Japanese Bobtail",
                "Selkirk Rex"
            };
        }

        private static IEnumerable<string> PopularCatBreedsFrom2015()
        {
            return new List<string>
            {
                "Exotic",
                "Persian",
                "Maine Coon",
                "Ragdoll",
                "British Shorthair",
                "American Shorthair",
                "Scottish Fold",
                "Abyssinian",
                "Sphynx",
                "Oriental",
                "Devon Rex",
                "Siamese",
                "Cornish Rex",
                "Norwegian Forest Cat",
                "Birman",
                "Russian Blue",
                "Tonkinese",
                "Siberian",
                "Burmese",
                "Ocicat",
                "Egyptian Mau",
                "Japanese Bobtail",
                "Selkirk Rex",
                "Manx",
                "Turkish Angora"
            };
        }
    }
}
