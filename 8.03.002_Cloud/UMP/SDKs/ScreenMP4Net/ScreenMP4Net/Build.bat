Set BuildProject=ScreenMP4Net.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv ScreenMP4Net.csproj /rebuild "Release|AnyCPU" /project ScreenMP4Net /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End