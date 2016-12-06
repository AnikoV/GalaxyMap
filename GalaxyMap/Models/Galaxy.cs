﻿using System.Collections.Generic;

namespace GalaxyMap.Models
{
    public class Galaxy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Star> Stars { get; set; }
    }
}