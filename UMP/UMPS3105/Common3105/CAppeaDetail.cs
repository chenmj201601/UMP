// ***********************************************************************
// Assembly         : Common3105
// Author           : Luoyihua
// Created          : 12-15-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 12-15-2014
// ***********************************************************************
// <copyright file="CAppeaDetail.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3105
{
    public class CAppeaDetail
    {
        public string AppealDetailID { set; get; }
        public string AppealFlowID { set; get; }
        public string AppealFlowItemID { set; get; }
        public string Note { set; get; }
        public string ActionID { set; get; }
        public string OperationerID { set; get; }
        public string OperationerName { set; get; }
    }
}
