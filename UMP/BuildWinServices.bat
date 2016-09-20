@echo ...
@echo Start Build
@date /t
@time /t


@echo WinService Build Start


@echo ====================
@echo UMPService00
cd UMP.PF.Services\UMPService00\UMPService00
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService01
cd UMP.PF.Services\UMPService01\UMPService01
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService02
cd UMP.PF.Services\UMPService02\UMPService02
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService04
cd UMP.PF.Services\UMPService04\UMPService04
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPService05
cd UMP.PF.Services\UMPService05\UMPService05
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPService06
cd UMP.PF.Services\UMPService06\UMPService06
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService07
cd UMP.PF.Services\UMPService07\UMPService07
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService08
cd UMP.PF.Services\UMPService08\UMPService08
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService09
cd UMP.PF.Services\UMPService09\UMPService09
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPService10
cd UMP.PF.Services\UMPService10\UMPService10
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END



@echo WinService Build End


@goto end




:error
@Set BuildSuc=0

:end
@echo Build End