@echo off
rem Creates the ExpandableWebAppDB database on SQL Server LocalDB.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Setup-Database.ps1" %*
pause
