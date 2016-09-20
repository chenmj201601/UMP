Set BuildProject=VCAvalonDock.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv VCAvalonDock.csproj /rebuild "Release|AnyCPU" /project VCAvalonDock /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End