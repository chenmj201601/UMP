Set BuildProject=UMP.MAMT.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMP.MAMT.csproj /rebuild "Release|AnyCPU" /project UMP.MAMT /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End