Set BuildProject=VCPropertyGrid.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv VCPropertyGrid.csproj /rebuild "Release|AnyCPU" /project VCPropertyGrid /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End