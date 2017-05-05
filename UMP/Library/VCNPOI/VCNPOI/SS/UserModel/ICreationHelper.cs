//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    ae9af1b2-dbdc-4484-85de-59f5ac4e726a
//        CLR Version:              4.0.30319.42000
//        Name:                     ICreationHelper
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                ICreationHelper
//
//        Created by Charley at 2016/9/30 15:38:22
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.UserModel
{
    /**
   * An object that handles instantiating concrete
   *  classes of the various instances one needs for
   *  HSSF and XSSF.
   * Works around a major shortcoming in Java, where we
   *  can't have static methods on interfaces or abstract
   *  classes.
   * This allows you to get the appropriate class for
   *  a given interface, without you having to worry
   *  about if you're dealing with HSSF or XSSF, despite
   *  Java being quite rubbish.
   */
    public interface ICreationHelper
    {
        /**
         * Creates a new RichTextString instance
         * @param text The text to Initialise the RichTextString with
         */
        IRichTextString CreateRichTextString(String text);

        /**
         * Creates a new DataFormat instance
         */
        IDataFormat CreateDataFormat();

        /**
         * Creates a new Hyperlink, of the given type
         */
        IHyperlink CreateHyperlink(HyperlinkType type);

        /**
         * Creates FormulaEvaluator - an object that Evaluates formula cells.
         *
         * @return a FormulaEvaluator instance
         */
        IFormulaEvaluator CreateFormulaEvaluator();

        IClientAnchor CreateClientAnchor();
    }
}
