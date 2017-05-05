@Set BuildProject=UMPSFTP.exe
@echo Build %BuildProject%

Set path=%DefaultPath%;
Set Include=
Set Lib=
Set LibPath=

call "%MSVS_2012%\vcvarsall.bat" x86
@if "%1"=="Release" (
@echo devenv /Rebuild "Release" "UMPSFTP.sln"  /out d:\a.log
devenv /Rebuild "Release" "UMPSFTP.sln"  /out d:\a.log
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end
)

@if "%1"=="Debug" (
@echo devenv UMPSFTP.sln /Rebuild "Debug|Win32" /project umpsadrs
devenv UMPSFTP.sln  /Rebuild "Debug|Win32" /project umpsadrs
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end
) else (
goto error
)

:error
@Set BuildSuc=0
goto end

:defile
del .\SpeechAnalysis\%1 /q
rd .\SpeechAnalysis\%1

:end
