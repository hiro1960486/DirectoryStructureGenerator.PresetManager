@echo off
setlocal
cd /d "%~dp0"
echo Building self-contained Windows x64 package...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o "%~dp0publish\win-x64"
if errorlevel 1 (
 echo Publish failed.
 pause
 exit /b 1
)
echo Publish completed: %~dp0publish\win-x64
explorer "%~dp0publish\win-x64"
