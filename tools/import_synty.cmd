@echo off
setlocal

set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe"
set "PROJECT=C:\Users\Philip\Projects\games\Language-Immersion-Virtual-Environment\client"
set "PACKS=C:\Users\Philip\Downloads\assets"
set "LOGDIR=D:\unity-import-logs"

if not exist "%LOGDIR%" mkdir "%LOGDIR%"

echo === Importing all .unitypackage files from %PACKS% ===
for %%F in ("%PACKS%\*.unitypackage") do (
  echo [%%~nxF] importing...
  "%UNITY%" ^
    -batchmode -nographics -accept-apiupdate ^
    -projectPath "%PROJECT%" ^
    -importPackage "%%~fF" ^
    -quit ^
    -logFile "%LOGDIR%\%%~nF.log"
)

echo === Final normalize pass ===
"%UNITY%" ^
  -batchmode -nographics -accept-apiupdate ^
  -projectPath "%PROJECT%" ^
  -executeMethod SyntyBulkImportAndNormalize.NormalizeOnly ^
  -quit ^
  -logFile "%LOGDIR%\normalize.log"

echo.
echo ✅ All imports complete.
echo Logs saved in: %LOGDIR%
echo.
pause
endlocal
