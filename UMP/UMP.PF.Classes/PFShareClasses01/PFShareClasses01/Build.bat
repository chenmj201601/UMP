Set BuildProject=PFShareClasses01.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv PFShareClasses01.csproj /rebuild "Release|AnyCPU" /project PFShareClasses01 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End