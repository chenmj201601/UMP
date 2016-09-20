@date /t
@time /t
@echo on


Set RootPath=D:\InstallPacket\UMP\8.03.002\UMP.PF.Site


@echo ===START COPY===


@call CopyWinServices.bat

@call CopyWcfServices.bat

@call CopyWcf2Client.bat


@date /t
@time /t
@echo copy File Finish.

@echo All File Copy End.