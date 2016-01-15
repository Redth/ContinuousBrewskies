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
	var apk = AndroidPackage ("./Droid/ContinuousBrewskies.Droid.csproj", true, c => c.Configuration = "Release");

	// Get our test cloud parameters from Environment variables set in CI
	var xtcApiKey = EnvironmentVariable ("XTC_API_KEY") ?? "";
	var xtcEmail = EnvironmentVariable ("XTC_EMAIL") ?? "";
	var xtcDeviceSet = EnvironmentVariable ("XTC_DEVICES") ?? "";

	// If there's a .xtc file, it will override the environment variables
	// 1st line = XTC_API_KEY, 2nd = XTC_EMAIL, 3rd = XTC_DEVICES
	if (FileExists ("./.xtc")) {
		var xtcFile = FileReadLines ("./.xtc");
		xtcApiKey = xtcFile [0];
		xtcEmail = xtcFile [1];
		xtcDeviceSet = xtcFile [2];
	}

	// Run testcloud
	TestCloud (apk, xtcApiKey, xtcDeviceSet, xtcEmail, "./UITests/bin/Release/", new TestCloudSettings { ToolPath = testCloudExePath });

});

Task ("TestCloud").IsDependentOn ("TestCloudAndroid");

RunTarget (TARGET);