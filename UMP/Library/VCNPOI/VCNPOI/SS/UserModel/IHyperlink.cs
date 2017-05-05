//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    18a01c01-5197-4f05-975f-1838d2e9808a
//        CLR Version:              4.0.30319.42000
//        Name:                     IHyperlink
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                IHyperlink
//
//        Created by Charley at 2016/9/30 15:39:17
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.UserModel
{
    public enum HyperlinkType
    {
        Unknown = 0,
        /// <summary>
        /// Link to an existing file or web page
        /// </summary>
        Url = 1,
        /// <summary>
        /// Link to a place in this document
        /// </summary>
        Document = 2,
        /// <summary>
        /// Link to an E-mail Address
        /// </summary>
        Email = 3,
        /// <summary>
        /// Link to a file
        /// </summary>
        File = 4
    }
    /// <summary>
    /// Represents an Excel hyperlink.
    /// </summary>
    public interface IHyperlink
    {
        /// <summary>
        /// Hyperlink address. Depending on the hyperlink type it can be URL, e-mail, patrh to a file, etc.
        /// </summary>
        String Address { get; set; }

        /// <summary>
        /// text label for this hyperlink
        /// </summary>
        String Label { get; set; }

        /// <summary>
        /// the type of this hyperlink
        /// </summary>
        HyperlinkType Type { get; }

        /// <summary>
        /// the row of the first cell that Contains the hyperlink
        /// </summary>
        int FirstRow { get; set; }
        /// <summary>
        /// the row of the last cell that Contains the hyperlink
        /// </summary>
        int LastRow { get; set; }

        /// <summary>
        /// the column of the first cell that Contains the hyperlink
        /// </summary>
        int FirstColumn { get; set; }

        /// <summary>
        /// the column of the last cell that Contains the hyperlink
        /// </summary>
        int LastColumn { get; set; }

        string TextMark { get; set; }
    }
}
