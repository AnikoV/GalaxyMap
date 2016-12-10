using GalaxyMap.Models;
using GalaxyMap.Utils;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;

namespace GalaxyMap.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public List<Constellation> Constellations { get; set; }
        public MainWindowViewModel()
        {
            int a = 1;
        }


        public void WriteConstellations(List<Constellation> constellationsList)
        {
            using (var context = new galaxyMapEntities())
            {
                var newConstellation = constellationsList.Select(c => c.nameOfConstellation);
                var oldConstellation = context.Constellation
                    .Where(c => newConstellation.Contains(c.nameOfConstellation))
                    .ToList();

                foreach (var i in constellationsList)
                {
                    var checkConstellation = oldConstellation
                        .SingleOrDefault(c => c.nameOfConstellation == i.nameOfConstellation);
                    if (checkConstellation == null)
                        context.Constellation.Add(i);
                }
                context.SaveChanges();
            }
        }


        public void WriteStars(List<Star> starsList)
        {
            using (var context = new galaxyMapEntities())
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


        public List<Constellation> ReadAllConstellations()
        {
            var allConstellationList = new List<Constellation>();
            using (var context = new galaxyMapEntities())
            {
                allConstellationList = context.Constellation.ToList();
            }

            return allConstellationList;
        }


        public List<Star> ReadAllStars()
        {
            var allStarsList = new List<Star>();
            using (var context = new galaxyMapEntities())
            {
                allStarsList = context.Star.ToList();
            }

            return allStarsList;
        }



        public List<Constellation> SearchConstellations(string searchString)
        {
            var foundConstellations = new List<Constellation>();
            var allConstellationList = new List<Constellation>();

            using (var context = new galaxyMapEntities())
            {
                allConstellationList = context.Constellation.ToList();
                foreach (var i in allConstellationList)
                {
                    if (i.nameOfConstellation.Contains(searchString))
                        foundConstellations.Add(i);
                }
            }
            return foundConstellations;
        }

        public List<Star> SearchStars(string searchString)
        {
            var foundStars = new List<Star>();
            var allStarsList = new List<Star>();

            using (var context = new galaxyMapEntities())
            {
                allStarsList = context.Star.ToList();
                foreach (var i in allStarsList)
                {
                    if (i.nameOfStar.Contains(searchString))
                        foundStars.Add(i);
                }
            }
            return foundStars;
        }

        
    }
}