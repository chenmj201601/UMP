Set BuildProject=Wcf31022.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf31022.csproj /rebuild "Release|AnyCPU" /project Wcf31022 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End