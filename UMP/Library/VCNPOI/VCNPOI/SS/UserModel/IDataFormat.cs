//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    3e8616bb-4e21-4e79-9935-76cadc9e962e
//        CLR Version:              4.0.30319.42000
//        Name:                     IDataFormat
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                IDataFormat
//
//        Created by Charley at 2016/9/30 15:33:04
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.UserModel
{
    public interface IDataFormat
    {
        /**
         * get the format index that matches the given format string.
         * Creates a new format if one is not found.  Aliases text to the proper format.
         * @param format string matching a built in format
         * @return index of format.
         */
        short GetFormat(String format);

        /**
         * get the format string that matches the given format index
         * @param index of a format
         * @return string represented at index of format or null if there is not a  format at that index
         */
        String GetFormat(short index);
    }
}
