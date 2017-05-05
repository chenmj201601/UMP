//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    2f1466b5-f465-43d3-87b9-7124139a7e54
//        CLR Version:              4.0.30319.42000
//        Name:                     FunctionMetadata
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Function
//        File Name:                FunctionMetadata
//
//        Created by Charley at 2016/9/30 16:34:09
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Text;


namespace VoiceCyber.NPOI.SS.Formula.Function
{
    /**
      * Holds information about Excel built-in functions.
      * 
      * @author Josh Micich
      */
    public class FunctionMetadata
    {

        private int _index;
        private String _name;
        private int _minParams;
        private int _maxParams;
        private byte _returnClassCode;
        private byte[] _parameterClassCodes;
        private const short FUNCTION_MAX_PARAMS = 30;
        /* package */
        internal FunctionMetadata(int index, String name, int minParams, int maxParams,
            byte returnClassCode, byte[] parameterClassCodes)
        {
            _index = index;
            _name = name;
            _minParams = minParams;
            _maxParams = maxParams;
            _returnClassCode = returnClassCode;
            _parameterClassCodes = parameterClassCodes;
        }
        public int Index
        {
            get { return _index; }
        }
        public String Name
        {
            get { return _name; }
        }
        public int MinParams
        {
            get { return _minParams; }
        }
        public int MaxParams
        {
            get { return _maxParams; }
        }
        public bool HasFixedArgsLength
        {
            get { return _minParams == _maxParams; }
        }
        public byte ReturnClassCode
        {
            get { return _returnClassCode; }
        }
        public byte[] ParameterClassCodes
        {
            get { return (byte[])_parameterClassCodes.Clone(); }
        }
        public bool HasUnlimitedVarags
        {
            get
            {
                return FUNCTION_MAX_PARAMS == _maxParams;
            }
        }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append(GetType().Name).Append(" [");
            sb.Append(_index).Append(" ").Append(_name);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
