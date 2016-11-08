using GalaxyMap.Models;
using GalaxyMap.Utils;
using System.Collections.Generic;

namespace GalaxyMap.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public List<Star> ListOfStars { get; set; }
        public MainWindowViewModel()
        {

        }
         
    }
}