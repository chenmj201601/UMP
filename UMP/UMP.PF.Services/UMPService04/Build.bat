Set BuildProject=UMPService04.sln
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService04.sln /rebuild "Release|AnyCPU" /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End