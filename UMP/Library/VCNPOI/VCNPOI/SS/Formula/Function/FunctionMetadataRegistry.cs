//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    9a75f9b9-41de-4170-ab3b-9b83b1d7e5ce
//        CLR Version:              4.0.30319.42000
//        Name:                     FunctionMetadataRegistry
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Function
//        File Name:                FunctionMetadataRegistry
//
//        Created by Charley at 2016/9/30 16:33:32
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections;


namespace VoiceCyber.NPOI.SS.Formula.Function
{
    /**
     * Allows clients to Get <c>FunctionMetadata</c> instances for any built-in function of Excel.
     * 
     * @author Josh Micich
     */
    public class FunctionMetadataRegistry
    {
        /**
         * The name of the IF function (i.e. "IF").  Extracted as a constant for clarity.
         */
        public const String FUNCTION_NAME_IF = "IF";

        public const int FUNCTION_INDEX_IF = 1;
        public const short FUNCTION_INDEX_SUM = 4;
        public const int FUNCTION_INDEX_CHOOSE = 100;
        public const short FUNCTION_INDEX_INDIRECT = 148;
        public const short FUNCTION_INDEX_EXTERNAL = 255;
        private static FunctionMetadataRegistry _instance;

        private FunctionMetadata[] _functionDataByIndex;
        private Hashtable _functionDataByName;

        private static FunctionMetadataRegistry GetInstance()
        {
            if (_instance == null)
            {
                _instance = FunctionMetadataReader.CreateRegistry();
            }
            return _instance;
        }

        /* package */
        public FunctionMetadataRegistry(FunctionMetadata[] functionDataByIndex, Hashtable functionDataByName)
        {
            _functionDataByIndex = functionDataByIndex;
            _functionDataByName = functionDataByName;
        }

        /* package */
        public ICollection GetAllFunctionNames()
        {
            return _functionDataByName.Keys;
        }


        public static FunctionMetadata GetFunctionByIndex(int index)
        {
            return GetInstance().GetFunctionByIndexInternal(index);
        }

        private FunctionMetadata GetFunctionByIndexInternal(int index)
        {
            return _functionDataByIndex[index];
        }
        /**
         * Resolves a built-in function index. 
         * @param name uppercase function name
         * @return a negative value if the function name is not found.
         * This typically occurs for external functions.
         */
        public static short LookupIndexByName(String name)
        {
            FunctionMetadata fd = GetInstance().GetFunctionByNameInternal(name);
            if (fd == null)
            {
                return -1;
            }
            return (short)fd.Index;
        }

        private FunctionMetadata GetFunctionByNameInternal(String name)
        {
            return (FunctionMetadata)_functionDataByName[name];
        }


        public static FunctionMetadata GetFunctionByName(String name)
        {
            return GetInstance().GetFunctionByNameInternal(name);
        }
    }
}
