using System.Collections.Generic;

namespace GalaxyMap.Models
{
    public class Galaxy
    {
        int Id { get; set; }
        string Name { get; set; }
        List<Star> Stars { get; set; }
    }
}