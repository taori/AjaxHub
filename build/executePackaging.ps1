$packages = @("AjaxHub.Core","AjaxHub.MVC5")

$apiKey = [IO.File]::ReadAllText("publish.apiKey.txt")

$nugetPath = ".\Nuget.exe";

Resolve-DnsName "www.nuget.org" -ErrorAction Stop | Out-Null
Resolve-DnsName "www.symbolsource.org" -ErrorAction Stop | Out-Null
	
foreach ($package in $packages){
	[xml]$xml = Get-Content($package + ".nuspec")
	$version = $xml.package.metadata.version;

	"Setting up packacking for $package $version"

	$packagePath = ".\packages\$package\$version";

	If(Test-Path $packagePath){
		Remove-Item -Recurse -Force $packagePath
	}

	md -Force $packagePath | Out-Null

	# todo add sources for -Symbols pack process
	#$packArguments = "pack -Symbols -Version $version $package.nuspec -OutputDirectory $packagePath";
	$packArguments = "pack -Version $version $package.nuspec -OutputDirectory $packagePath";
	"Packaging with Nuget.exe $packArguments"
	Start-Process -FilePath $nugetPath -WindowStyle Hidden -ArgumentList $packArguments -ErrorAction Stop -Wait
	
	$pushArguments = "push $packagePath\$package.$version.nupkg -ApiKey $apiKey -Timeout 60 -Verbosity normal"
	"Pushing with Nuget.exe $pushArguments"	
	Start-Process -FilePath $nugetPath -WindowStyle Hidden -ArgumentList $pushArguments -ErrorAction Stop -Wait

	Start-Process -FilePath "https://www.nuget.org/packages/$package"	

	"";
	"";
}

Read-Host -Prompt "Script done. Press <enter>"