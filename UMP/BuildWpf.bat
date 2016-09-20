echo off
@echo ...
@echo Start Build
@date /t
@time /t


@echo Wpf Build Start


@echo ====================
@echo UMPS1101
cd UMPOUM\UMPS1101
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1102
cd UMPS1102\UMPS1102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1110
cd UMPS1110\UMPS1110
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1111
cd UMPS1111\UMPS1111
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1112
cd UMPS1112\UMPS1112
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1201
cd UMPS1200\UMPS1201
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1202
cd UMPS1200\UMPS1202
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1203
cd UMPS1200\UMPS1203
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1204
cd UMPS1200\UMPS1204
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS1206
cd UMPS1200\UMPS1206
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS2101
cd UMPS2101\UMPS2101
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS2102
cd UMPS2102\UMPS2102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS2400
cd UMPS2400\UMPS2400
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS2501
cd UMPS2501\UMPS2501
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS3101
cd UMPS3101\UMPS3101
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS3102
cd UMPS3102\UMPS3102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS3103
cd UMPS3103\UMPS3103
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END



@echo ====================
@echo UMPS3105
cd UMPS3105\UMPS3105
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS3106
cd UMPS3106\UMPS3106
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS3107
cd UMPS3107\UMPS3107
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS3108
cd UMPS3108\UMPS3108
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS3601
cd UMPS3601\UMPS3601
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS3602
cd UMPS3602\UMPS3602
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS3603
cd UMPS3603\UMPS3603
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS4601
cd UMPS4601\UMPS4601
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS4601
cd UMP.MAMT\UMP.MAMT
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS6101
cd wcf_report\wcf_report
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS6106
cd UMPS6106\UMPS6106
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS9800
cd UMPS9800\UMPS9800
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo UMPS5100
cd UMPS5100\UMPS5100
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END




@echo Wcf Build End




@goto end


:error
@Set BuildSuc=0

:end
@echo Build End