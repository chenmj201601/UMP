echo off
@echo ...
@echo Start Build
@date /t
@time /t

@echo ====================
@echo UMPS1600
cd UMPS1600\Common1600
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@goto Finish

:error
@echo FAILED TO %BuildProject%
goto End

:Finish
@echo ALL BUILD END
@date /t
@time /t
:End