using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BreweryDB.Models;

namespace ContinuousBrewskies
{
    public partial class BeerListPage : ContentPage
    {
        public BeerListPage ()
        {
            InitializeComponent ();

            BindingContext = new BeerListViewModel ();

            listBeers.ItemTapped += (sender, e) => {
                if (e.Item != null) {
                    var beer = e.Item as Beer;
                    Navigation.PushAsync (new BeerDetailPage (beer));
                }
            };
            listBeers.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
        }
    }
}

