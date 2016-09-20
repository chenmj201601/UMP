//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    346cd5bf-4b8d-4a53-b700-5e8012d8a5c6
//        CLR Version:              4.0.30319.18444
//        Name:                     S1110Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                S1110Consts
//
//        created by Charley at 2014/12/19 11:06:20
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    public class S1110Consts
    {
        //操作编码
        public const long OPT_SAVECURRENTCONFIG = 1110001;
        public const long OPT_SAVEALLCONFIG = 1110002;
        public const long OPT_RELOADCONFIGDATA = 1110003;
        public const long OPT_WIZARD = 1110004;

        //资源编码
        public const int RESOURCE_MACHINE = 210;
        public const int RESOURCE_LICENSESERVER = 211;
        public const int RESOURCE_DATATRANSFERSERVER = 212;
        public const int RESOURCE_CTIHUBSERVER = 213;
        public const int RESOURCE_STORAGEDEVICE = 214;
        public const int RESOURCE_DBBRIDGE = 215;
        public const int RESOURCE_DBBRIDGENAME = 216;
        public const int RESOURCE_ALARMMONITOR = 217;
        public const int RESOURCE_ALARMSERVER = 218;
        public const int RESOURCE_SFTP = 219;
        public const int RESOURCE_PBXDEVICE = 220;
        public const int RESOURCE_VOICESERVER = 221;
        public const int RESOURCE_NETWORKCARD = 222;
        public const int RESOURCE_VOIPPROTOCAL = 223;
        public const int RESOURCE_CONCURRENT = 224;
        public const int RESOURCE_CHANNEL = 225;
        public const int RESOURCE_NTIDRVPATH = 226;
        public const int RESOURCE_BOARDSPBXTYPE = 227;
        public const int RESOURCE_RECOVERSERVER = 228;
        public const int RESOURCE_CAPTURESERVER = 229;
        public const int RESOURCE_CAPTURENETWORKCARD = 230;
        public const int RESOURCE_SCREENSERVER = 231;
        public const int RESOURCE_SCREENCHANNEL = 232;
        public const int RESOURCE_ISASERVER = 233;
        public const int RESOURCE_CMSERVER = 236;
        public const int RESOURCE_CMSERVERVOICE = 237;
        public const int RESOURCE_CTICONNECTION = 241;
        public const int RESOURCE_CTICONNECTIONGROUP = 242;
        public const int RESOURCE_CTICONNECTIONGROUPCOLLECTION = 243;
        public const int RESOURCE_CTIPOLICY = 245;
        public const int RESOURCE_FILEOPERATOR = 251;
        public const int RESOURCE_ARCHIVESTRATEGY = 256;
        public const int RESOURCE_FILTERCONDITION = 257;
        public const int RESOURCE_ALARMMONITORPARAM = 261;
        public const int RESOURCE_ALARMHARDDISK = 262;
        public const int RESOURCE_ALARMPROCESS = 263;
        public const int RESOURCE_ALARMSERVICE = 264;
        public const int RESOURCE_ALARMLOGCONTENT = 265;
        public const int RESOURCE_ALARMCONTENTMATCH = 266;
        public const int RESOURCE_ALARMSERVERPARAM = 270;
        public const int RESOURCE_ALARMEMAILSERVER = 271;
        public const int RESOURCE_ALARMABNORMALRECORD = 272;
        public const int RESOURCE_ALARMSENDRULE = 273;
        public const int RESOURCE_ALARMRECORDNUMBER = 274;
        public const int RESOURCE_ALARMRECORDNUMBERCHECKTIME = 275;
        public const int RESOURCE_ALARMRECORDNUMBERDEVICE = 276;
        public const int RESOURCE_ALARMNORECORD = 277;
        public const int RESOURCE_ALARMNORECORDCHECKTIME = 278;
        public const int RESOURCE_ALARMNORECORDCHECKDEVICE = 279;
        public const int RESOURCE_SPEECHANALYSISPARAM = 281;
        public const int RESOURCE_CTIDBBRIDGE = 282;
        public const int RESOURCE_ENGINE = 283;
        public const int RESOURCE_KEYGENERATOR = 286;
        public const int RESOURCE_ALARMHEALTHCHECK = 287;
        public const int RESOURCE_ALARMHEALTHCHECKRECIVER = 288;
        public const int RESOURCE_ALARMLOGGINGPARAM = 289;
        public const int RESOURCE_DOWNLOADPARAM = 291;
        public const int RESOURCE_ALARMSCREENNUMBER = 292;
        public const int RESOURCE_ALARMSCREENNUMBERCHECKTIME = 293;
        public const int RESOURCE_ALARMSCREENNUMBERDEVICE = 294;
      

        //特定的属性编号
        public const int PROPERTYID_KEY = 1;
        public const int PROPERTYID_ID = 2;
        public const int PROPERTYID_PARENTOBJID = 3;
        public const int PROPERTYID_MODULENUMBER = 4;
        public const int PROPERTYID_MASTERSLAVER = 5;
        public const int PROPERTYID_ENABLEDISABLE = 6;
        public const int PROPERTYID_HOSTADDRESS = 7;
        public const int PROPERTYID_HOSTNAME = 8;
        public const int PROPERTYID_HOSTPORT = 9;
        public const int PROPERTYID_MACHINE = 10;

        public const int PROPERTYID_CONTINENT = 901;
        public const int PROPERTYID_COUNTRY = 902;
        public const int PROPERTYID_AUTHUSERNAME = 911;
        public const int PROPERTYID_AUTHPASSWORD = 912;
        public const int PROPERTYID_XMLKEY = 921;
        public const int PROPERTYID_XMLOBJID = 922;

        //树节点的类型
        public const int OBJECTITEMTYPE_CONFIGOBJECT = 0;
        public const int OBJECTITEMTYPE_CONFIGGROUP = 1;
        public const int OBJECTITEMTYPE_DIRINFO = 10;
        public const int OBJECTITEMTYPE_FILEINFO = 11;

        #region Monitor 相关

        //Monitor 指令
        public const int MONITOR_COMMAND_GETCONFIGOBJECT = 101;

        #endregion

        //SourceID
        public const int SOURCEID_CONTINENT = 111010000;
        public const int SOURCEID_COUNTRY = 111011000;
        public const int SOURCEID_VOICECHANNEL_STARTTYPE = 111000100;
        public const int SOURCEID_STORAGE_DEVICETYPE = 111000110;
        public const int SOURCEID_DOWNLOADMETHOD = 111000115;
        public const int SOURCEID_CONCURRENTNUMBER = 111000150;
        public const int SOURCEID_VOIPPROTOCOL_TYPE = 111000210;
        public const int SOURCEID_CTITYPE = 111000300;
        public const int SOURCEID_PBX_DEVICETYPE = 111000301;
        public const int SOURCEID_PBX_MONITORMODE = 111000302;
        public const int SOURCEID_CTICONNECTION_VERSIONPROTOCOL = 111000303;

        //LicenseID
        public const int LICENSEID_VOICE1 = 353632257;
        public const int LICENSEID_VOICE2 = 353632258;
        public const int LICENSEID_VOICE3 = 353632259;
        public const int LICENSEID_VOICE4 = 353632260;
        public const int LICENSEID_VOICE5 = 353632261;
        public const int LICENSEID_VOICE6 = 353632262;
        public const int LICENSEID_VOICE7 = 353632263;
        public const int LICENSEID_VOICE8 = 353632264;

        public const int LICENSEID_VOICE1X = 353632267;
        public const int LICENSEID_VOICE2X = 353632268;
        public const int LICENSEID_VOICE4X = 353632269;
        public const int LICENSEID_VOICE6X = 353632270;

        public const int LICENSEID_VOICE1REG = 353632307;
        public const int LICENSEID_VOICE7REG = 353632313;
    }
}
