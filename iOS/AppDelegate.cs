using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;

namespace ContinuousBrewskies.iOS
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            Forms.Init ();

            Forms.ViewInitialized += (object sender, ViewInitializedEventArgs e) => {
                // http://developer.xamarin.com/recipes/testcloud/set-accessibilityidentifier-ios/
                if (null != e.View.StyleId)
                    e.NativeView.AccessibilityIdentifier = e.View.StyleId;
            };
                
            Xamarin.Calabash.Start();

            LoadApplication (new App ());

            return base.FinishedLaunching (app, options);
        }
    }
}

