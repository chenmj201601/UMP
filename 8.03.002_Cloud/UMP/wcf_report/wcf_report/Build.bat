Set BuildProject=UMPS6101.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS6101.csproj /rebuild "Release|AnyCPU" /project UMPS6101 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End