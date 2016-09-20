Set BuildProject=Wcf24011.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf24011.csproj /rebuild "Release|AnyCPU" /project Wcf24011 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End