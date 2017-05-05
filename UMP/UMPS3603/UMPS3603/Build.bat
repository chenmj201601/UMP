Set BuildProject=UMPS3603.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS3603.csproj /rebuild "Release|AnyCPU" /project UMPS3603 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End