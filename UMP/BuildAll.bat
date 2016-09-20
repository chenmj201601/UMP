echo off
@echo ...
@echo Start Build
@date /t
@time /t

@echo Common Library Start Build
@call BuildCommonLibs.bat
if %BuildSuc% == 0 goto error



@echo Module Common Build Start
@call BuildModuleCommons.bat
if %BuildSuc% == 0 goto error


@echo WinService Build Start
@call BuildWinServices.bat
if %BuildSuc% == 0 goto error


@echo Wcf Build Start
@call BuildWcf.bat
if %BuildSuc% == 0 goto error


@echo Wpf Build Start
@call BuildWpf.bat
if %BuildSuc% == 0 goto error


@goto Finish

:error
@echo FAILED TO %BuildProject%
goto End

:Finish
@echo ALL BUILD END
@date /t
@time /t
:End