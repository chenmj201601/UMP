//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    1c676c80-0089-4258-90cf-e8ba590b54f8
//        CLR Version:              4.0.30319.42000
//        Name:                     IPictureData
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                IPictureData
//
//        Created by Charley at 2016/9/30 15:37:03
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.UserModel
{
    public interface IPictureData
    {

        /**
         * Gets the picture data.
         *
         * @return the picture data.
         */
        byte[] Data { get; }

        /**
         * Suggests a file extension for this image.
         *
         * @return the file extension.
         */
        String SuggestFileExtension();
        /**
         * Returns the mime type for the image
         */
        String MimeType { get; }

        /**
         * @return the POI internal image type, 0 if unknown image type
         *
         * @see Workbook#PICTURE_TYPE_DIB
         * @see Workbook#PICTURE_TYPE_EMF
         * @see Workbook#PICTURE_TYPE_JPEG
         * @see Workbook#PICTURE_TYPE_PICT
         * @see Workbook#PICTURE_TYPE_PNG
         * @see Workbook#PICTURE_TYPE_WMF
         */
        PictureType PictureType { get; }
    }
}
