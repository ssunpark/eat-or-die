@echo off
setlocal

:: Jenkins의 현재 작업 디렉토리 기준으로 Unity 프로젝트 루트로 이동 (Client 폴더)
cd /d "%~dp0\.."

:: 현재 디렉토리를 PROJECT_PATH로 설정
set PROJECT_PATH=%cd%

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