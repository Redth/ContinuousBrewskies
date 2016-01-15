using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BreweryDB.Models;

namespace ContinuousBrewskies
{
    public partial class BeerDetailPage : ContentPage
    {
        public BeerDetailPage (Beer beer)
        {
            InitializeComponent ();

            BindingContext = new BeerDetailViewModel (beer);
        }
    }
}

