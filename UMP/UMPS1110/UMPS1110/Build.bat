Set BuildProject=UMPS1110.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS1110.csproj /rebuild "Release|AnyCPU" /project UMPS1110 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End