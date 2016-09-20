Set BuildProject=Wcf24041.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf24041.csproj /rebuild "Release|AnyCPU" /project Wcf24041 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End