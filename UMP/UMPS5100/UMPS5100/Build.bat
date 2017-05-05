Set BuildProject=UMPS5100.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS5100.csproj /rebuild "Release|AnyCPU" /project UMPS5100 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End