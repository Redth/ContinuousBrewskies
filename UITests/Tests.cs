using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace ContinuousBrewskies.UITests
{
    [TestFixture (Platform.Android)]
    [TestFixture (Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests (Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest ()
        {
            app = AppInitializer.StartApp (platform);
        }

        //[Test]
        public void Repl ()
        {
            app.Repl ();
        }

        [Test]
        public void BeerListIsDisplayed ()
        {
            var results = app.WaitForElement (c => c.Marked ("\"Ah Me Joy\" Porter"));

            app.Screenshot ("List");

            Assert.IsTrue (results.Any ());
        }

        [Test]
        public void NavigatedToBeerDetail ()
        {
            app.Screenshot ("List");

            app.Tap (c => c.Marked ("\"Ah Me Joy\" Porter"));

            app.Screenshot ("Details");

            var results = app.Query (c => c.Marked ("Back"));
            Assert.IsTrue (results.Any ());
        }
    }
}

