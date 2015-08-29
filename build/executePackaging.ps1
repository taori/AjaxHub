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

	<# todo: find out how to fix the push part
	$pushArguments = "push $packagePath\$package.$version.nupkg -ApiKey $apiKey -Timeout 60 -Verbosity normal"
	"Pushing with Nuget.exe $pushArguments"
	
	Start-Process -FilePath ".\Nuget.exe" -WindowStyle Hidden -ArgumentList $pushArguments -ErrorAction Stop 
	Start-Process -FilePath "https://www.nuget.org/packages/$package"
	#>
	
	Start-Process -FilePath "$packagePath\"	

	"";
	"";
}

Start-Process -FilePath "https://www.nuget.org/users/account/LogOn"

Read-Host -Prompt "Script done. Press <enter>"