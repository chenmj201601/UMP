Set BuildProject=UMPService10.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService10.csproj /rebuild "Release|AnyCPU" /project UMPService10 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End