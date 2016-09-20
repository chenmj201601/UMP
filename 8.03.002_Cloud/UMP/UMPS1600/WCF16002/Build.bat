Set BuildProject=Wcf16002.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf16002.csproj /rebuild "Release|AnyCPU" /project Wcf16002 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End