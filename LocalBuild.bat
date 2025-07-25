@echo off
setlocal

:: [🔧 설정 영역]
:: Unity 에디터 경로
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.0.48f1\Editor\Unity.exe"

:: 현재 스크립트 위치 기준으로 Unity 프로젝트 루트 경로 설정
cd /d "%~dp0"
set PROJECT_PATH=%cd%

:: 빌드 결과물이 저장될 경로 (프로젝트 루트 기준 상대 경로)
set BUILD_PATH=%PROJECT_PATH%\Build\eatOrDie.exe

:: 로그 파일 경로
set LOG_FILE=%PROJECT_PATH%\Build\build.log

:: [🚀 빌드 실행]
echo 🔨 Starting Unity Build...
%UNITY_PATH% ^
  -batchmode -quit ^
  -logFile "%LOG_FILE%" ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod BuildScript.PerformBuild

:: [📦 결과 출력]
echo -----------------------------------
echo ✅ Build completed. Check log at:
echo %LOG_FILE%
echo Output: %BUILD_PATH%
echo -----------------------------------

endlocal
pause
