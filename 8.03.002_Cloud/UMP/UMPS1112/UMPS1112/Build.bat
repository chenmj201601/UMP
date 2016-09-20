Set BuildProject=UMPS1112.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPS1112.csproj /rebuild "Release|AnyCPU" /project UMPS1112 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End