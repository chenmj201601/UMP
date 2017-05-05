//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    2a6cf0de-ca48-474e-b0e3-7bb5324dd8f0
//        CLR Version:              4.0.30319.42000
//        Name:                     FunctionDataBuilder
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Function
//        File Name:                FunctionDataBuilder
//
//        Created by Charley at 2016/9/30 16:35:11
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections;


namespace VoiceCyber.NPOI.SS.Formula.Function
{
    /**
      * Temporarily collects <c>FunctionMetadata</c> instances for creation of a
      * <c>FunctionMetadataRegistry</c>.
      * 
      * @author Josh Micich
      */
    class FunctionDataBuilder
    {
        private int _maxFunctionIndex;
        private Hashtable _functionDataByName;
        private Hashtable _functionDataByIndex;
        /** stores indexes of all functions with footnotes (i.e. whose definitions might Change) */
        private ArrayList _mutatingFunctionIndexes;

        public FunctionDataBuilder(int sizeEstimate)
        {
            _maxFunctionIndex = -1;
            _functionDataByName = new Hashtable(sizeEstimate * 3 / 2);
            _functionDataByIndex = new Hashtable(sizeEstimate * 3 / 2);
            _mutatingFunctionIndexes = new ArrayList();
        }

        public void Add(int functionIndex, String functionName, int minParams, int maxParams,
                byte returnClassCode, byte[] parameterClassCodes, bool hasFootnote)
        {
            FunctionMetadata fm = new FunctionMetadata(functionIndex, functionName, minParams, maxParams,
                    returnClassCode, parameterClassCodes);

            int indexKey = functionIndex;


            if (functionIndex > _maxFunctionIndex)
            {
                _maxFunctionIndex = functionIndex;
            }
            // allow function definitions to Change only if both previous and the new items have footnotes
            FunctionMetadata prevFM;
            prevFM = (FunctionMetadata)_functionDataByName[functionName];
            if (prevFM != null)
            {
                if (!hasFootnote || !_mutatingFunctionIndexes.Contains(indexKey))
                {
                    throw new Exception("Multiple entries for function name '" + functionName + "'");
                }
                _functionDataByIndex.Remove(prevFM.Index);
            }
            prevFM = (FunctionMetadata)_functionDataByIndex[indexKey];
            if (prevFM != null)
            {
                if (!hasFootnote || !_mutatingFunctionIndexes.Contains(indexKey))
                {
                    throw new Exception("Multiple entries for function index (" + functionIndex + ")");
                }
                _functionDataByName.Remove(prevFM.Name);
            }
            if (hasFootnote)
            {
                _mutatingFunctionIndexes.Add(indexKey);
            }
            _functionDataByIndex[indexKey] = fm;
            _functionDataByName[functionName] = fm;
        }

        public FunctionMetadataRegistry Build()
        {

            FunctionMetadata[] jumbledArray = new FunctionMetadata[_functionDataByName.Count];
            IEnumerator values = _functionDataByName.Values.GetEnumerator();
            FunctionMetadata[] fdIndexArray = new FunctionMetadata[_maxFunctionIndex + 1];
            while (values.MoveNext())
            {
                FunctionMetadata fd = (FunctionMetadata)values.Current;
                fdIndexArray[fd.Index] = fd;
            }

            return new FunctionMetadataRegistry(fdIndexArray, _functionDataByName);
        }
    }
}
