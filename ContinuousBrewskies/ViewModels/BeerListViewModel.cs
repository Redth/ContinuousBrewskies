using System;
using System.ComponentModel;
using BreweryDB;
using System.Collections.ObjectModel;
using BreweryDB.Models;
using System.Threading.Tasks;

namespace ContinuousBrewskies
{
    public class BeerListViewModel
    {
        readonly BreweryDbClient breweryDbClient = new BreweryDbClient (Configuration.BreweryDbApiKey);

        public BeerListViewModel ()
        {
            LoadBeers ();
        }

        public ObservableCollection<Beer> Beers { get; set; } = new ObservableCollection<Beer> ();

        public async Task LoadBeers ()
        {
            try {
                var beers = await breweryDbClient.Beers.GetAll ();

                Xamarin.Forms.Device.BeginInvokeOnMainThread (() => {
                    foreach (var beer in beers.Data)
                        Beers.Add (beer);
                });
            } catch (Exception ex) {
                var e = ex;
            }
        }
    }
}

