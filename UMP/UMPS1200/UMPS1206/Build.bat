Set BuildProject=UMPS1206.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS1206.csproj /rebuild "Release|AnyCPU" /project UMPS1206 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End