Set BuildProject=UMPS3601.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS3601.csproj /rebuild "Release|AnyCPU" /project UMPS3601 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End