using System;
using BreweryDB.Models;

namespace ContinuousBrewskies
{
    public class BeerDetailViewModel
    {
        public BeerDetailViewModel (Beer beer)
        {
            Beer = beer;
        }

        public Beer Beer { get; set; }
    }
}

