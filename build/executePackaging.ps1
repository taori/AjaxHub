$packages = @("AjaxHub.Core","AjaxHub.MVC5")

$apiKey = [IO.File]::ReadAllText("publish.apiKey.txt")
	
foreach ($package in $packages){
	[xml]$xml = Get-Content($package + ".nuspec")
	$version = $xml.package.metadata.version;

	"Setting up packacking for $package $version"

	$packagePath = ".\packages\$package\$version";

	If(Test-Path $packagePath){
		Remove-Item -Recurse -Force $packagePath
	}

	md -Force $packagePath | Out-Null

	$packArguments = "pack -Symbols -Version $version $package.nuspec -OutputDirectory $packagePath";
	"Packaging with Nuget.exe $packArguments"
	Start-Process -FilePath ".\Nuget.exe" -WindowStyle Hidden -ArgumentList $packArguments -ErrorAction Stop

	
	$pushArguments = "push $packagePath\$package.$version.symbols.nupkg $apiKey"
	"Pushing with Nuget.exe $pushArguments"
	
	Start-Process -FilePath ".\Nuget.exe" -WindowStyle Hidden -ArgumentList $pushArguments -ErrorAction Stop 
	Start-Process -FilePath "https://www.nuget.org/packages/$package"
	

	"";
	"";
}

Read-Host -Prompt "Script done. Press <enter>"