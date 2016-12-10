using GalaxyMap.Models;
using GalaxyMap.Utils;
using System.Collections.Generic;
using System.Diagnostics;

namespace GalaxyMap.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public List<Galaxy> Galaxies { get; set; }
        public MainWindowViewModel()
        {
            int a = 1;
        }

        public void Search(string searchText)
        {
            Debug.WriteLine("Search " + searchText);
        }
    }
}