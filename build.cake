#addin "Cake.FileHelpers"

var TARGET = Argument ("target", Argument ("t", "Default"));

Task ("Default").Does (() =>
{

});

Task ("InjectKeys").Does (() =>
{
	// Get the API Key from the Environment variable
	var breweryDbApiKey = EnvironmentVariable ("BREWERY_DB_API_KEY") ?? "";

	// Replace the placeholder in our .cs files
	ReplaceTextInFiles ("./**/*.cs", "{BREWERY_DB_API_KEY}", breweryDbApiKey;

});

RunTarget (TARGET);