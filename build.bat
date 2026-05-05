@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:: 1. Check version argument
if "%~1"=="" (
  echo Error: Version not specified.
  echo Usage: build.bat 1.0.2
  exit /b 1
)

set "VERSION=%~1"
set "ROOT_DIR=%~dp0"
set "BUILD_DIR=%ROOT_DIR%Build"
set "DESKTOP_DIR=%ROOT_DIR%Desktop"
set "CLIENT_DIR=%ROOT_DIR%Client"

echo =======================================================
echo Starting release build v%VERSION%
echo =======================================================

:: 2. Update version in StreamTabula.csproj
echo Updating version in Desktop/StreamTabula.csproj...
powershell -Command "(Get-Content '%DESKTOP_DIR%\StreamTabula.csproj') -replace '<Version>.*</Version>', '<Version>%VERSION%</Version>' | Set-Content '%DESKTOP_DIR%\StreamTabula.csproj'"

:: 3. Update version in pubspec.yaml
echo Updating version in Client/pubspec.yaml...
powershell -Command "(Get-Content '%CLIENT_DIR%\pubspec.yaml') -replace 'version: .*', 'version: %VERSION%+1' | Set-Content '%CLIENT_DIR%\pubspec.yaml'"

:: 4. Clean root Build directory
if exist "%BUILD_DIR%" (
  echo Cleaning Build folder...
  rmdir /s /q "%BUILD_DIR%"
)
mkdir "%BUILD_DIR%"

:: 5. Build Desktop project
echo.
echo =======================================================
echo Building Desktop (WPF)...
echo =======================================================
cd /d "%DESKTOP_DIR%"
:: Use a temporary publish folder inside the project
set "TEMP_PUB=%DESKTOP_DIR%\bin\Release\publish"
if exist "%TEMP_PUB%" rmdir /s /q "%TEMP_PUB%"

dotnet publish StreamTabula.csproj -c Release -o "%TEMP_PUB%"

:: 7-8. Archive Windows version
echo Creating archive StreamTabula_Windows_v%VERSION%.zip...
powershell -Command "Compress-Archive -Path '%TEMP_PUB%\*' -DestinationPath '%BUILD_DIR%\StreamTabula_Windows_v%VERSION%.zip' -Force"

:: 9. Build Flutter APK
echo.
echo =======================================================
echo Building Client (Android APK)...
echo =======================================================
cd /d "%CLIENT_DIR%"
call flutter build apk --release

:: 10. Rename and copy APK
echo Copying APK to Build folder...
set "ORIGINAL_APK=%CLIENT_DIR%\build\app\outputs\flutter-apk\app-release.apk"
if exist "%ORIGINAL_APK%" (
  copy "%ORIGINAL_APK%" "%BUILD_DIR%\StreamTabula_Android_v%VERSION%.apk" >nul
  ) else (
  echo Error: APK file not found!
)

:: 11. Finish
echo.
echo =======================================================
echo Build completed successfully!
echo Files in folder: %BUILD_DIR%
dir "%BUILD_DIR%" /b
echo =======================================================
cd /d "%ROOT_DIR%"
pause
