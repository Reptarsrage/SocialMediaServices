@echo off

echo Download FAKE and place it in packages/FAKE
nuget install FAKE -OutputDirectory packages -ExcludeVersion

echo Download NuGet and place it in packages/nuget
nuget install NuGet.CommandLine -OutputDirectory packages -ExcludeVersion

echo Run FAKE against the build script with args %*
packages\FAKE\tools\FAKE.exe %* 