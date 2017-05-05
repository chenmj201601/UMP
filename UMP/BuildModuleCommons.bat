echo off
@echo ...
@echo Start Build
@date /t
@time /t



@echo Module Common Build Start

@echo ====================
@echo UMPEncryptionC
cd Library\UMPEncryption\UMPEncryptionC
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPEncryptionS
cd Library\UMPEncryption\UMPEncryptionS
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo DEC4Net
cd SDKs\DEC4Net\DEC4Net
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo NMon4Net
cd SDKs\NMon4Net\NMon4Net
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo License4Net
cd SDKs\License4Net\License4Net
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo ScreenMP4Net
cd SDKs\ScreenMP4Net\ScreenMP4Net
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Windows4Net
cd SDKs\Windows4Net\Windows4Net
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPScoreSheet
cd UMPTemplate\UMPScoreSheet
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common000A1
cd UMPS000A\Common000A1
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common11011
cd UMPOUM\Common11011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common1102
cd UMPS1102\Common1102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common11101
cd UMPS1110\Common11101
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common1111
cd UMPS1111\Common1111
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common11121
cd UMPS1112\Common11121
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS1600
cd UMPS1600\Common1600
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common12001
cd UMPS1200\Common12001
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common12002
cd UMPS1200\Common12002
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common21011
cd UMPS2101\Common21011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common21021
cd UMPS2102\Common21021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common21061
cd UMPS2106\Common21061
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common2400
cd UMPS2400\Common2400
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common25011
cd UMPS2501\Common25011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common31011
cd UMPS3101\Common31011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common31021
cd UMPS3102\Common31021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common3103
cd UMPS3103\Common3103
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common31041
cd UMPS3104\Common31041
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common3105
cd UMPS3105\Common3105
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common3106
cd UMPS3106\Common3106
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common3107
cd UMPS3107\Common3107
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common31081
cd UMPS3108\Common31081
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common44101
cd UMPS4410\Common44101
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common5102
cd UMPS5102\Common5102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common61011
cd wcf_report\Common61011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Common6106
cd UMPS6106\Common6106
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Common98001
cd UMPS9800\Common98001
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo CommonService03
cd UMP.PF.Services\UMPService03\CommonService03
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo CommonService04
cd UMP.PF.Services\UMPService04\CommonService04
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo CommonService07
cd UMP.PF.Services\UMPService07\CommonService07
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo CommonService10
cd UMP.PF.Services\UMPService10\CommonService10
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPResourceXmls
cd UMPS1110\UMPResourceXmls
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS1600
cd UMPS1600\UMPS1600
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPControls
cd UMPCommon\UMPControls
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPPlayers
cd UMPCommon\UMPPlayers
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPClient
cd UMPClient\UMPClient
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPUninstall
cd Other\UMPSetup\UMPUninstall
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UUMP.MAMT
cd UMP.MAMT\UMP.MAMT
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END



@echo Module Common Build End


@goto end




:error
@Set BuildSuc=0

:end
@echo Build End