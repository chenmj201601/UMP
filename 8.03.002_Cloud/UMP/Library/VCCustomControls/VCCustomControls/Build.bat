Set BuildProject=VCCustomControls.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv VCCustomControls.csproj /rebuild "Release|AnyCPU" /project VCCustomControls /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End