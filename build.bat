@echo off
setlocal

:: 현재 스크립트 기준으로 프로젝트 루트 경로 지정
set SCRIPT_DIR=%~dp0
set PROJECT_PATH=%SCRIPT_DIR%

:: 뒤에 \ 가 붙는 경우 제거 (깨끗한 경로로 만들기)
if %PROJECT_PATH:~-1%==\ set PROJECT_PATH=%PROJECT_PATH:~0,-1%

:: Unity 실행 파일 경로
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.0.48f1\Editor\Unity.exe"

:: 로그 파일 경로
set LOG_FILE=%PROJECT_PATH%\Build\build.log

:: Unity CLI 빌드 수행
%UNITY_PATH% ^
  -batchmode -quit ^
  -logFile "%LOG_FILE%" ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod BuildScript.PerformBuild

:: 결과 출력
echo -----------------------------------
echo Build completed. Check log at:
echo %LOG_FILE%
echo -----------------------------------

endlocal
pause