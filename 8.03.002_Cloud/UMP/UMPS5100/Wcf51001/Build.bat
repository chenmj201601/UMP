Set BuildProject=Wcf51001.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf51001.csproj /rebuild "Release|AnyCPU" /project Wcf51001 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End