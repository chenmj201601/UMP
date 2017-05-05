Set BuildProject=UMPScoreSheet.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPScoreSheet.csproj /rebuild "Release|AnyCPU" /project UMPScoreSheet /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End