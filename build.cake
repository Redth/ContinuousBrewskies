#addin "Cake.FileHelpers"
#addin "Cake.Xamarin"

var TARGET = Argument ("target", Argument ("t", "Default"));

// Try and find a test-cloud-exe from the installed nugets
var testCloudExePath = GetFiles ("./**/test-cloud.exe").FirstOrDefault ();

// Get our test cloud parameters from Environment variables set in CI
var xtcApiKey = EnvironmentVariable ("XTC_API_KEY") ?? "";
var xtcEmail = EnvironmentVariable ("XTC_EMAIL") ?? "";
var xtcAndroidDevices = EnvironmentVariable ("XTC_DEVICES_ANDROID") ?? "";
var xtciOSDevices = EnvironmentVariable ("XTC_DEVICES_IOS") ?? "";

// If there's a .xtc file, it will override the environment variables
// 1st line = XTC_API_KEY, 2nd = XTC_EMAIL, 3rd = XTC_DEVICES
if (FileExists ("./.xtc")) {
	var xtcFile = FileReadLines ("./.xtc");
	xtcApiKey = xtcFile [0];
	xtcEmail = xtcFile [1];
	xtcAndroidDevices = xtcFile [2];
	xtciOSDevices = xtcFile [3];
}

Task ("Default").Does (() =>
{
	NuGetRestore ("./ContinuousBrewskies.sln");

	DotNetBuild ("./ContinuousBrewskies.sln", c => c.Configuration = "Release");
});

Task ("InjectKeys").Does (() =>
{
	// Get the API Key from the Environment variable
	var breweryDbApiKey = EnvironmentVariable ("BREWERY_DB_API_KEY") ?? "";

	// Replace the placeholder in our Configuration.cs files
	ReplaceTextInFiles ("./**/Configuration.cs", "{BREWERY_DB_API_KEY}", breweryDbApiKey);
});

Task ("TestCloudAndroid")
	.IsDependentOn ("Default")
	.Does (() => 
{
	// Build the .apk to test
	var apk = AndroidPackage ("./Droid/ContinuousBrewskies.Droid.csproj", true, c => c.Configuration = "Release");

	// Run testcloud
	TestCloud (apk, xtcApiKey, xtcAndroidDevices, xtcEmail, "./UITests/bin/Release/", new TestCloudSettings { ToolPath = testCloudExePath });

});

Task ("TestCloudiOS")
	.WithCriteria (!IsRunningOnWindows ()) // Mac only
	.Does (() => 
{
	// Build Project to produce an .ipa file
	DotNetBuild ("./ContinuousBrewskies.sln", c => {
		c.Configuration = "Release";
		c.Properties.Add ("Platform", new List<string> { "iPhone" });
		c.Properties.Add ("BuildIpa", new List<string> { "true" });
		c.Targets.Add ("ContinuousBrewskies_iOS");
	});

	var ipa = GetFiles ("./**/iPhone/Release/**/*.ipa").FirstOrDefault ();

	// Run testcloud
	TestCloud (ipa, xtcApiKey, xtciOSDevices, xtcEmail, "./UITests/bin/Release/", new TestCloudSettings { ToolPath = testCloudExePath });

});

// Link both tasks together
Task ("TestCloud")
	.IsDependentOn ("TestCloudAndroid")
	.IsDependentOn ("TestCloudiOS");

RunTarget (TARGET);