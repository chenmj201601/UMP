Set BuildProject=PFShareControls.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv PFShareControls.csproj /rebuild "Release|AnyCPU" /project PFShareControls /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End