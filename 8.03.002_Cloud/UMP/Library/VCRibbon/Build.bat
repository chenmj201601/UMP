Set BuildProject=VCRibbon.sln
@echo Build %BuildProject%
@date /t
@time /t

devenv VCRibbon.sln /rebuild "Release|AnyCPU" /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End