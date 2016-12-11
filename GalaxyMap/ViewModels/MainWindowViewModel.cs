using GalaxyMap.Models;
using GalaxyMap.Utils;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;

namespace GalaxyMap.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public List<Constellation> Constellations { get; set; }
        public List<SearchResult> LastSearchResult { get; set; }

        public MainWindowViewModel()
        {
            Constellations = ReadAllConstellations();
        }
        
        private List<Constellation> ReadAllConstellations()
        {
            var allConstellationList = new List<Constellation>();
            using (var context = new galaxyMapEntities1())
            {
                allConstellationList = context.Constellation.Include(x => x.Star).ToList();
            }
            return allConstellationList;
        }

        public void SearchStarsInDataBase(string searchString)
        {
            var foundStars = new List<SearchResult>();
            var allStarsList = new List<Star>();

            using (var context = new galaxyMapEntities1())
            {
                allStarsList = context.Star.ToList();
                foreach (var star in allStarsList)
                {
                    if (star.nameOfStar.ToLower().Contains(searchString))
                    {
                        foundStars.Add(new SearchResult
                        {
                            Star = star,
                            Constellation = "Созвездие: " + Constellations.First(x => x.id == star.idOfConstellation).ConstellationName
                        });
                    }
                }
            }
            LastSearchResult = foundStars;
        }

        public void WriteConstellations(List<Constellation> constellationsList)
        {
            using (var context = new galaxyMapEntities1())
            {
                var newConstellation = constellationsList.Select(c => c.ConstellationName);
                var oldConstellation = context.Constellation
                    .Where(c => newConstellation.Contains(c.ConstellationName))
                    .ToList();

                foreach (var i in constellationsList)
                {
                    var checkConstellation = oldConstellation
                        .SingleOrDefault(c => c.ConstellationName == i.ConstellationName);
                    if (checkConstellation == null)
                        context.Constellation.Add(i);
                }
                context.SaveChanges();
            }
        }

        public void WriteStars(List<Star> starsList)
        {
            using (var context = new galaxyMapEntities1())
            {
                var newStars = starsList.Select(c => c.nameOfStar);
                var oldStars = context.Star
                    .Where(c => newStars.Contains(c.nameOfStar))
                    .ToList();

                foreach (var i in starsList)
                {
                    var checkConstellation = oldStars
                        .SingleOrDefault(c => c.nameOfStar == i.nameOfStar);
                    if (checkConstellation == null)
                        context.Star.Add(i);
                }
                context.SaveChanges();
            }
        }
    }

    public class SearchResult
    {
        public Star Star { get; set; }
        public string Constellation { get; set; }

    }
}