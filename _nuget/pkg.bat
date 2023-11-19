dotnet build  --configuration release ..\Cross.CQRS.EF.sln
nuget.exe pack config.nuspec -Symbols -SymbolPackageFormat snupkg
