@echo off
rmdir /S /Q NuGet
src\.nuget\NuGet pack src\ShipEngineApi -Symbols -SymbolPackageFormat snupkg -Properties Configuration=Release -OutputDirectory NuGet