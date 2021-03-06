﻿CREATE VIEW V_44_012_VIEWTEMPAGENTSEAT AS 
SELECT   t4414.ID, t4414.AgentID, t4414.PlanID, t4414.SeatID, t4414.Shift, t4414.ShiftDate, t4414.StartTime, t4414.EndTime,
                t4414.Status, t4414.CreateID, t4414.CreateDate, t4414.ModifyID, t4414.ModifyDate, v4411.RSID, v4411.RegionID,
                v4411.MLeft, v4411.MTop, t1134.C003 as UserFullName, v4411.SeatCode, t1134.C002 as LoginAccount
FROM      T_44_014_TempPlanResult t4414 LEFT OUTER JOIN
                V_44_011_VIEWPLANSEATPOSTIONI v4411 ON t4414.PlanID = v4411.PlanID AND
                t4414.SeatID = v4411.SeatID INNER JOIN
                T_11_005_00000 t1134 ON t4414.AgentID = t1134.C001