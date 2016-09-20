Set BuildProject=NMon4Net.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv NMon4Net.csproj /rebuild "Release|AnyCPU" /project NMon4Net /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End