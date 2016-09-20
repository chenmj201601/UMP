Set BuildProject=UMPS3106.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS3106.csproj /rebuild "Release|AnyCPU" /project UMPS3106 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End