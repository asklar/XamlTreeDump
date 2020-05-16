@echo off
msbuild /restore %~dp0\src\TreeDumpLibrary\ /p:configuration=Release
nuget pack %~dp0\src\TreeDumpLibrary
