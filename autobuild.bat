@echo off
setlocal

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.0.48f1\Editor\Unity.exe"

set PROJECT_PATH=C:\Users\madog\eat-or-die

set LOG_FILE=%PROJECT_PATH%\Build\build.log

%UNITY_PATH% ^
  -batchmode -quit ^
  -logFile "%LOG_FILE%" ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod BuildScript.PerformBuild

echo -----------------------------------
echo Build completed. Check log at:
echo %LOG_FILE%
echo -----------------------------------

endlocal
pause
