Set BuildProject=UMPS2101.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS2101.csproj /rebuild "Release|AnyCPU" /project UMPS2101 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End