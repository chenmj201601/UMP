﻿CREATE VIEW V_44_010_VIEWTEMPRESULTAGENT AS 
SELECT   t4414.ID, t4414.AgentID, t4414.PlanID, t4414.SeatID, t4414.Shift, t4414.ShiftDate, t4414.StartTime, t4414.EndTime,
                t4414.Status, t4414.CreateID, t4414.CreateDate, t4414.ModifyID, t4414.ModifyDate, t4402.SeatCode,
                t1134.C003 AS UserFullName, t4403.RSID, t4403.RegionID, t4403.SeatID AS Expr1, t4403.MLeft, t4403.MTop,
                t4403.CreateID AS Expr2, t4403.CreateDate AS Expr3, t4403.ModifyID AS Expr4, t4403.ModifyDate AS Expr5
FROM      T_44_014_TempPlanResult t4414 INNER JOIN
                T_44_002_Seat t4402 ON t4414.SeatID = t4402.SeatID INNER JOIN
                T_11_005_00000 t1134 ON t4414.AgentID = t1134.C001 INNER JOIN
                T_44_003_R_Seat t4403 ON t4414.SeatID = t4403.SeatID