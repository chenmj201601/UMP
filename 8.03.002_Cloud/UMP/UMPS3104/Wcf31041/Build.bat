Set BuildProject=Wcf31041.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf31041.csproj /rebuild "Release|AnyCPU" /project Wcf31041 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End