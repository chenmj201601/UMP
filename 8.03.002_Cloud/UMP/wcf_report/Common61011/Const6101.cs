using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common61011
{
    public class Const6101
    {
        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";
        public const string WriteTablixColumn = "//ab:Report/ab:Body/ab:ReportItems/ab:Tablix/ab:TablixBody/ab:TablixColumns";
        public const string WriteTablixCell = "//ab:Report/ab:Body/ab:ReportItems/ab:Tablix/ab:TablixBody/ab:TablixRows";
        public const string WriteField = "//ab:Report/ab:Body/ab:ReportItems/ab:Tablix/ab:TablixColumnHierarchy/ab:TablixMembers";
        public const string XmlNamespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
        public const string XmlNamespaceRD = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

        //查询条件保存用条件编号
        public const long Query_RecordTime = 3031602220000000001;
        public const long Query_OperationTime = 3031602220000000002;
        public const long Query_Time = 3031602220000000003;
        public const long Query_ReportType = 3031602220000000004;
        public const long Query_Call = 3031602220000000005;
        public const long Query_Reply = 3031602220000000006;
        public const long Query_OperationResult = 3031602220000000007;
        public const long Query_QueryType = 3031602220000000008;
        public const long Query_PointNumber = 3031602220000000009;
        public const long Query_TopNumber = 3031602220000000010;
        public const long Query_Agent = 3031602220000000011;
        public const long Query_Extension = 3031602220000000012;
        public const long Query_User = 3031602220000000013;
        public const long Query_Machine = 3031602220000000014;
        public const long Query_MachineIP = 3031602220000000015;
        public const long Query_RecordMachine = 3031602220000000016;
        public const long Query_ScoreTable = 3031602220000000017;
        public const long Query_Grader = 3031602220000000018;
        public const long Query_ScoreTableItem = 3031602220000000019;
        public const long Query_DepartmentBasic = 3031602220000000020;
        public const long Query_KeyWords = 3031602220000000021;
    }
}
