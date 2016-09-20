Set BuildProject=VCRibbon.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv VCRibbon.csproj /rebuild "Release|AnyCPU" /project VCRibbon /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End