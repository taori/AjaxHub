$packages = @("AjaxHub.Core","AjaxHub.MVC5")

<#
	batch version for comprehension

	@echo off

	SET NUGET=NuGet.exe
	SET OUTDIR=.\packages
	SET /p VER= <package.publish.version.txt
	SET /p PACKNAME= <package.publish.name.txt
	SET /p APK= <package.publish.apiKey.txt

	rmdir /s /q %OUTDIR%\%VER%
	mkdir %OUTDIR%
	mkdir %OUTDIR%\%VER%

	@ECHO ===NUGET Publishing Version %PACKNAME% %VER% to %OUTDIR%\%VER%
	%NUGET% pack -Symbols -Version %VER% package.nuspec -OutputDirectory %OUTDIR%\%VER%

	IF [%APK%] == [] GOTO end
	IF [%PACKNAME%] == [] GOTO end

	 %NUGET% push %OUTDIR%\%VER%\%PACKNAME%.%VER%.symbols.nupkg -ApiKey %APK%

	:end

	pause

#>

$apiKey = [IO.File]::ReadAllText("publish.apiKey.txt")
	
foreach ($package in $packages){
	[xml]$xml = Get-Content($package + ".nuspec")
	$version = $xml.package.metadata.version;

	"Setting up packacking for $package $version"

	$packagePath = ".\packages\$package\$version";

	If(Test-Path $packagePath){
		Remove-Item -Recurse -Force $packagePath
	}

	md -Force $packagePath

	$packArguments = "pack -Symbols -Version $version $package.nuspec -OutputDirectory $packagePath";
	"Packaging with Nuget.exe $packArguments"
	Start-Process -FilePath ".\Nuget.exe" -WindowStyle Hidden -ArgumentList $packArguments -ErrorAction Stop

	
	$pushArguments = "push $packagePath\$package.$version.symbols.nupkg $apiKey"
	"Pushing with Nuget.exe $pushArguments"
		
	Start-Process -FilePath ".\Nuget.exe" -WindowStyle Hidden -ArgumentList $pushArguments -ErrorAction Stop
	Start-Process -FilePath "https://www.nuget.org/packages/$package"
}

Read-Host -Prompt "Script done. Press <enter>"