Set BuildProject=UMPS2102.sln
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS2102.sln /rebuild "Release|AnyCPU" /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End