Set BuildProject=UMPResourceXmls.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPResourceXmls.csproj /rebuild "Release|AnyCPU" /project UMPResourceXmls /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End