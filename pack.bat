@echo off
rmdir /S /Q NuGet
src\.nuget\NuGet pack src\ShipEngineApi -Symbols -Properties Configuration=Release -OutputDirectory NuGet