Set BuildProject=Wcf24021.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf24021.csproj /rebuild "Release|AnyCPU" /project Wcf24021 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End