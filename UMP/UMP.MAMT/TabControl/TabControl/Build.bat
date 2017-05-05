Set BuildProject=YoungWPFTabControl.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wpf.TabControl.csproj /rebuild "Release|AnyCPU" /project Wpf.TabControl /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End