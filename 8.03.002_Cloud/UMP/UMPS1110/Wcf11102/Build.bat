Set BuildProject=Wcf11102.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf11102.csproj /rebuild "Release|AnyCPU" /project Wcf11102 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End