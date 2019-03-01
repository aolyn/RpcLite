del bin\Debug\*.nupkg
dotnet build -c Debug
dotnet pack -c Debug
nuget push bin\Debug\*.nupkg -Source https://www.nuget.org
