Set BuildProject=CircleQueueClass.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv CircleQueueClass.csproj /rebuild "Release|AnyCPU" /project CircleQueueClass /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End