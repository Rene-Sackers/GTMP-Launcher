@echo off

set exeName=%1

if [%1] == [] (
    set exeName=GrandTheftMultiplayer.Launcher.exe
)

taskkill /f /im %exeName%

:loop
tasklist | find "%exeName%" >nul
if not errorlevel 1 (
    timeout /t 1 >nul
    goto :loop
)

move /Y ".\tempstorage\%exeName%" "%exeName%"
start "" "%exeName%"

del "%~f0"