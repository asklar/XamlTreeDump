@echo off
set /p version=< %~dp0\LastVersion.txt
echo Current version: %version%
set /a newVersion=%version% + 1
echo New version: %newVersion%
nuget pack %~dp0\src\TreeDumpLibrary\TreeDumpLibrary.csproj -Build -Symbols -Properties Configuration=Release -Version 1.0.%newVersion%
echo %newVersion% > %~dp0\LastVersion.txt
git add %~dp0\LastVersion.txt
