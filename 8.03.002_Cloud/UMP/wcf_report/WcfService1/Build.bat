Set BuildProject=Wcf61011.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf61011.csproj /rebuild "Release|AnyCPU" /project Wcf61011 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End