@date /t
@time /t
@echo on


@echo ===START COPY WcfServices===

xcopy /y /f ".\UMPS000A\Common000A1\bin\Release\Common000A1.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1102\Common1102\bin\Release\Common1102.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1111\Common1111\bin\Release\Common1111.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1600\Common1600\bin\Release\Common1600.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2400\Common2400\bin\Release\Common2400.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3103\Common3103\bin\Release\Common3103.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPOUM\Common11011\bin\Release\Common11011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1110\Common11101\bin\Release\Common11101.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1200\Common12001\bin\Release\Common12001.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1200\Common12002\bin\Release\Common12002.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2101\Common21011\bin\Release\Common21011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2102\Common21021\bin\Release\Common21021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2501\Common25011\bin\Release\Common25011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3101\Common31011\bin\Release\Common31011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3102\Common31021\bin\Release\Common31021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3104\Common31041\bin\Release\Common31041.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3105\Common3105\bin\Release\Common3105.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3106\Common3106\bin\Release\Common3106.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3107\Common3107\bin\Release\Common3107.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3108\Common31081\bin\Release\Common31081.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS4601\Common4601\bin\Release\Common4601.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS4601\Common4602\bin\Release\Common4602.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\wcf_report\Common61011\bin\Release\Common61011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS9800\Common98001\bin\Release\Common98001.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMP.PF.Services\UMPService03\CommonService03\bin\Release\CommonService03.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS6106\Common6106\bin\Release\Common6106.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS5100\Common5100\bin\Release\Common5100.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1112\Common11121\bin\Release\Common11121.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3601\Common3601\bin\Release\Common3601.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3603\Common3603\bin\Release\Common3603.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3602\Common3602\bin\Release\Common3602.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS4410\Common44101\bin\Release\Common44101.dll" "%RootPath%\WcfServices\bin\"


xcopy /y /f ".\UMP.PF.Classes\CircleQueueClass\CircleQueueClass\bin\Release\CircleQueueClass.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMP.PF.Classes\PFShareClasses01\PFShareClasses01\bin\Release\PFShareClasses01.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMP.PF.Classes\PFShareClassesS\PFShareClassesS\bin\Release\PFShareClassesS.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\Library\UMPEncryption\UMPEncryptionS\bin\Release\UMPEncryptionS_Secure\UMPEncryptionS.dll" "%RootPath%\WcfServices\bin\"


xcopy /y /f ".\UMPCommon\UMPCommon\bin\Release\UMPCommon.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPCommon\UMPCommunications\bin\Release\UMPCommunications.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1110\UMPResourceXmls\bin\Release\UMPResourceXmls.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPTemplate\UMPScoreSheet\bin\Release\UMPScoreSheet.dll" "%RootPath%\WcfServices\bin\"


xcopy /y /f ".\VCCommon\VCCommon\bin\Release\VCCommon.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\Library\VCCustomControls\VCCustomControls\bin\Release\VCCustomControls.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\Library\VCDBAccess\VCDBAccess\bin\Release\VCDBAccess.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\Library\VCLDAP\VCLDAP\bin\Release\VCLDAP.dll" "%RootPath%\WcfServices\bin\"

xcopy /y /f ".\UMP.PF.WCF\Wcf00000\Wcf00000\bin\Wcf00000.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMP.PF.WCF\Wcf00001\Wcf00001\bin\Wcf00001.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMP.PF.WCF\Wcf00003\bin\Wcf00003.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMP.PF.WCF\Wcf11901\Wcf11901\bin\Wcf11901.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS000A\Wcf000A1\bin\Wcf000A1.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1111\Wcf1111\bin\Wcf11111.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1112\Wcf11121\bin\Wcf11121.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPOUM\Wcf11011\bin\Wcf11011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPOUM\Wcf11012\bin\Wcf11012.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1102\Wcf11021\bin\Wcf11021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1102\Wcf11021\bin\Wcf11021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1110\Wcf11101\bin\Wcf11101.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1110\Wcf11102\bin\Wcf11102.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1600\WCF16002\bin\WCF16002.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS1200\Wcf12001\bin\Wcf12001.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2101\Wcf21011\bin\Wcf21011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2102\Wcf21021\bin\Wcf21021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2400\Wcf24011\bin\Wcf24011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2400\Wcf24021\bin\Wcf24021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2400\Wcf24041\bin\Wcf24041.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS2501\Wcf25011\bin\Wcf25011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3101\Wcf31011\bin\Wcf31011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3102\Wcf31021\bin\Wcf31021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3103\Wcf31031\bin\Wcf31031.dll" "%RootPath%\WcfServices\bin\" 
xcopy /y /f ".\UMPS3103\WcfService31032\bin\WcfService31032.dll" "%RootPath%\WcfServices\bin\"

xcopy /y /f ".\UMPS3104\Wcf31041\bin\Wcf31041.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3105\Wcf31051\bin\Wcf31051.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3106\Wcf31061\bin\Wcf31061.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3107\Wcf31071\bin\Wcf31071.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3108\Wcf31081\bin\Wcf31081.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS4601\Wcf46011\bin\Wcf46011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS4601\Wcf46012\bin\Wcf46012.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\wcf_report\WcfService1\bin\Wcf61011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\wcf_report\Wcf61012\bin\Wcf61012.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS9800\Wcf98001\bin\Wcf98001.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS6106\Wcf61061\bin\Wcf61061.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS5100\Wcf51001\bin\Wcf51001.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3601\Wcf36011\bin\Wcf36011.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3603\Wcf36031\bin\Wcf36031.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3602\Wcf36021\bin\Wcf36021.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS3103\WcfService31032\bin\WcfService31032.dll" "%RootPath%\WcfServices\bin\"
xcopy /y /f ".\UMPS4410\Wcf44101\bin\Wcf44101.dll" "%RootPath%\WcfServices\bin\"


xcopy /y /f ".\UMP.PF.WCF\Wcf00000\Wcf00000\Service00000.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMP.PF.WCF\Wcf00001\Wcf00001\Service00001.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMP.PF.WCF\Wcf00003\Service00003.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMP.PF.WCF\Wcf11901\Wcf11901\Service11901.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS000A\Wcf000A1\Service000A1.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1111\Wcf1111\Service11111.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1112\Wcf11121\Service11121.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPOUM\Wcf11011\Service11011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPOUM\Wcf11012\Service11012.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1102\Wcf11021\Service11021.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1102\Wcf11021\Service11021.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1110\Wcf11101\Service11101.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1110\Wcf11102\Service11102.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1600\WCF16002\Service16002.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS1200\Wcf12001\Service12001.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS2101\Wcf21011\Service21011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS2102\Wcf21021\Service21021.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS2400\Wcf24011\Service24011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS2400\Wcf24021\Service24021.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS2400\Wcf24041\Service24041.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS2501\Wcf25011\Service25011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3101\Wcf31011\Service31011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3102\Wcf31021\Service31021.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3102\Wcf31022\Service31022.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3103\Wcf31031\Service31031.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3104\Wcf31041\Service31041.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3105\Wcf31051\Service31051.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3106\Wcf31061\Service31061.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3107\Wcf31071\Service31071.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3108\Wcf31081\Service31081.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS4601\Wcf46011\Service46011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS4601\Wcf46012\Service46012.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\wcf_report\WcfService1\Service61011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\wcf_report\Wcf61012\Service61012.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS9800\Wcf98001\Service98001.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS6106\Wcf61061\Service61061.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS5100\Wcf51001\Service51001.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3601\Wcf36011\Service36011.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3603\Wcf36031\Service36031.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3602\Wcf36021\Service36021.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS3103\WcfService31032\Service31032.svc" "%RootPath%\WcfServices\"
xcopy /y /f ".\UMPS4410\Wcf44101\Service44101.svc" "%RootPath%\WcfServices\"



::WCF16001
xcopy /y /f ".\UMPS1600\WCF16001\bin\Common1600.dll" "%RootPath%\WCF1600\bin\"
xcopy /y /f ".\UMPS1600\WCF16001\bin\PFShareClasses01.dll" "%RootPath%\WCF1600\bin\"
xcopy /y /f ".\UMPS1600\WCF16001\bin\PFShareClassesS.dll" "%RootPath%\WCF1600\bin\"
xcopy /y /f ".\UMPS1600\WCF16001\bin\UMPCommunications.dll" "%RootPath%\WCF1600\bin\"
xcopy /y /f ".\UMPS1600\WCF16001\bin\VCCommon.dll" "%RootPath%\WCF1600\bin\"
xcopy /y /f ".\UMPS1600\WCF16001\bin\WCF16001.dll" "%RootPath%\WCF1600\bin\"
xcopy /y /f ".\UMPS1600\WCF16001\Service16001.svc" "%RootPath%\WCF1600\"
xcopy /y /f ".\UMPS1600\WCF16001\Web.config" "%RootPath%\WCF1600\"

xcopy /y /f ".\UMPCommon\UMPCommon\bin\Release\UMPCommon.dll" "%RootPath%\WCF1600\bin\"




@date /t
@time /t
@echo copy File Finish.