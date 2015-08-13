@echo off

SET NUGET=NuGet.exe
SET OUTDIR=..\build\packages
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