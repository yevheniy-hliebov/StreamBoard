@echo off
set "TARGET_DIR=bin\x86\publish"

echo =======================================================
echo Cleaning up: %TARGET_DIR%
echo =======================================================

if exist "%TARGET_DIR%" (
    rmdir /s /q "%TARGET_DIR%"
    echo Old files deleted.
) else (
    echo Folder is already clean.
)

echo.
echo =======================================================
echo Publishing StreamBoard (Release mode)...
echo =======================================================
echo.

dotnet publish StreamBoard.csproj -c Release -o "%TARGET_DIR%"

echo.
echo =======================================================
echo Publish completed!
echo =======================================================
exit