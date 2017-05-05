Set BuildProject=UMPCommunications.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPCommunications.csproj /rebuild "Release|AnyCPU" /project UMPCommunications /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End