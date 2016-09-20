Set BuildProject=VCLDAP.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv VCLDAP.csproj /rebuild "Release|AnyCPU" /project VCLDAP /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End