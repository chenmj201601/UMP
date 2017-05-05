//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    e652122a-1502-4ada-bf5d-26c07188c54c
//        CLR Version:              4.0.30319.42000
//        Name:                     AggregatingUDFFinder
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Udf
//        File Name:                AggregatingUDFFinder
//
//        Created by Charley at 2016/9/30 15:57:36
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VoiceCyber.NPOI.SS.Formula.Udf
{
    /**
      * Collects Add-in libraries and VB macro functions toGether into one UDF Finder
      *
      * @author PUdalau
      */
    public class AggregatingUDFFinder : UDFFinder
    {

        private List<UDFFinder> _usedToolPacks = new List<UDFFinder>();

        public AggregatingUDFFinder(params UDFFinder[] usedToolPacks)
        {
            _usedToolPacks = new List<UDFFinder>(usedToolPacks.Length);
            _usedToolPacks.AddRange(usedToolPacks);
        }

        /// <summary>
        /// Returns executor by specified name. 
        /// </summary>
        /// <param name="name">Name of function.</param>
        /// <returns>Function executor. null if not found</returns>
        public override FreeRefFunction FindFunction(String name)
        {
            FreeRefFunction evaluatorForFunction;
            foreach (UDFFinder pack in _usedToolPacks)
            {
                evaluatorForFunction = pack.FindFunction(name);
                if (evaluatorForFunction != null)
                {
                    return evaluatorForFunction;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a new toolpack
        /// </summary>
        /// <param name="toolPack"></param>
        public void Add(UDFFinder toolPack)
        {
            _usedToolPacks.Add(toolPack);
        }
    }
}
