Set BuildProject=Wcf31071.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf31071.csproj /rebuild "Release|AnyCPU" /project Wcf31071 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End