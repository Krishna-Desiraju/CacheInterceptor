@echo off
cls
@echo Executing FAKE Build...
".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion" "-version" "4.0.2"
"packages\FAKE\tools\Fake.exe" build.fsx "%1"
exit /b %errorlevel%
