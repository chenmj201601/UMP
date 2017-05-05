Set BuildProject=Wcf00001.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf00001.csproj /rebuild "Release|AnyCPU" /project Wcf00001 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End