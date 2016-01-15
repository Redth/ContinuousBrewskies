#addin "Cake.FileHelpers"
#addin "Cake.Xamarin"

var TARGET = Argument ("target", Argument ("t", "Default"));

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
	// Try and find a test-cloud-exe from the installed nugets
	var testCloudExePath = GetFiles ("./**/test-cloud.exe").FirstOrDefault ();

	// Build the .apk to test
	var apk = AndroidPackage ("./Droid/ContinuousBrewskies.Droid.csproj", true);

	var xtcApiKey = EnvironmentVariable ("XTC_API_KEY") ?? "";
	var xtcEmail = EnvironmentVariable ("XTC_EMAIL") ?? "";
	var xtcDeviceSet = EnvironmentVariable ("XTC_DEVICES") ?? "";

	// Run testcloud
	TestCloud (apk, xtcApiKey, xtcDeviceSet, xtcEmail, "./UITests/bin/Release/", new TestCloudSettings { ToolPath = testCloudExePath });

});

Task ("TestCloud").IsDependentOn ("TestCloudAndroid");

RunTarget (TARGET);