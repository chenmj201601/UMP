echo off
@echo ...
@echo Start Build
@date /t
@time /t


@echo Common Library Start Build


@echo ====================
@echo VCCommon
cd VCCommon\VCCommon
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCWindowsShells
cd Library\VCAvalonDock\VCWindowsShell
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCAvalonDock
cd Library\VCAvalonDock\VCAvalonDock
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCCustomControls
cd Library\VCCustomControls\VCCustomControls
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCNAudio
cd Library\VCNAudio\VCNAudio
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCNAudioControls
cd Library\VCNAudio\VCNAudioControls
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCPropertyGrid
cd Library\VCPropertyGrid\VCPropertyGrid
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCRibbon
cd Library\VCRibbon\VCRibbon
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCSharpZipLib
cd Library\VCSharpZipLib\VCSharpZipLib
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCDBAccess
cd Library\VCDBAccess\VCDBAccess
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo VCWebSocket
cd Library\VCWebSocket\VCWebSocket
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo CircleQueueClass
cd UMP.PF.Classes\CircleQueueClass\CircleQueueClass
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo PFShareClasses01
cd UMP.PF.Classes\PFShareClasses01\PFShareClasses01
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo PFShareClassesS
cd UMP.PF.Classes\PFShareClassesS\PFShareClassesS
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo PFShareClassesC
cd UMP.PF.Classes\PFShareClassesC\PFShareClassesC
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo YoungWPFTabControl
cd UMP.MAMT\TabControl\TabControl
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo PFShareControls
cd UMP.PF.Controls\PFShareControls\PFShareControls
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPCommon
cd UMPCommon\UMPCommon
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPCommunications
cd UMPCommon\UMPCommunications
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo VCLDAP
cd Library\VCLDAP\VCLDAP
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo Common Library Build End

@goto end



:error
@Set BuildSuc=0

:end
@echo Build End