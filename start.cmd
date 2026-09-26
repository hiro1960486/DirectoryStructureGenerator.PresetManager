@echo off
setlocal
cd /d "%~dp0"
dotnet run
if errorlevel 1 pause
