echo off
@echo ...
@echo Start Build
@date /t
@time /t


@echo Wcf Build Start


@echo ====================
@echo Wcf00000
cd UMP.PF.WCF\Wcf00000\Wcf00000
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf00001
cd UMP.PF.WCF\Wcf00001\Wcf00001
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf00003
cd UMP.PF.WCF\Wcf00003
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11901
cd UMP.PF.WCF\Wcf11901\Wcf11901
@call Build.bat
cd ..\..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11012
cd UMPOUM\Wcf11012
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf000A1
cd UMPS000A\Wcf000A1
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11011
cd UMPOUM\Wcf11011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11021
cd UMPS1102\Wcf11021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11101
cd UMPS1110\Wcf11101
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11102
cd UMPS1110\Wcf11102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11121
cd UMPS1112\Wcf11121
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf11111
cd UMPS1111\Wcf1111
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf16001
cd UMPS1600\Wcf16001
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf16001
cd UMPS1600\Wcf16002
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf12001
cd UMPS1200\Wcf12001
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf21011
cd UMPS2101\Wcf21011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf21021
cd UMPS2102\Wcf21021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf21061
cd UMPS2106\Wcf21061
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf24011
cd UMPS2400\Wcf24011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf24021
cd UMPS2400\Wcf24021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf24041
cd UMPS2400\Wcf24041
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf25011
cd UMPS2501\Wcf25011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf31011
cd UMPS3101\Wcf31011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf31021
cd UMPS3102\Wcf31021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf31031
cd UMPS3103\Wcf31031
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo WcfService31032
cd UMPS3103\WcfService31032
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf31041
cd UMPS3104\Wcf31041
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf31051
cd UMPS3105\Wcf31051
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf31061
cd UMPS3106\Wcf31061
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf31071
cd UMPS3107\Wcf31071
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf31081
cd UMPS3108\Wcf31081
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf36011
cd UMPS3601\Wcf36011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf36021
cd UMPS3602\Wcf36021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf36031
cd UMPS3603\Wcf36031
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END



@echo ====================
@echo Wcf46011
cd UMPS4601\Wcf46011
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf46012
cd UMPS4601\Wcf46012
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf61011
cd wcf_report\WcfService1
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf61012
cd wcf_report\Wcf61012
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END


@echo ====================
@echo Wcf61061
cd UMPS6106\Wcf61061
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf98001
cd UMPS9800\Wcf98001
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf51021
cd UMPS5102\Wcf51021
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo UMPS5102
cd UMPS5102\UMPS5102
@call Build.bat
cd ..\..
if %BuildSuc% == 0 goto error
@echo %BuildProject% BUILD END

@echo ====================
@echo Wcf44101
cd UMPS4410\Wcf44101
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