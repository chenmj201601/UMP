Set BuildProject=UMPControls.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPControls.csproj /rebuild "Release|AnyCPU" /project UMPControls /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End