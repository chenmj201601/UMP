CREATE VIEW V_44_010_ViewTempResultAgentInfo AS 
SELECT   t4414.ID, t4414.AgentID, t4414.PlanID, t4414.SeatID, t4414.Shift, t4414.ShiftDate, t4414.StartTime, t4414.EndTime, 
                t4414.Status, t4414.CreateID, t4414.CreateDate, t4414.ModifyID, t4414.ModifyDate, t4402.SeatCode, 
                t1134.C003 as UserFullName, t4403.RSID, t4403.RegionID, t4403.SeatID AS Expr1, t4403.MLeft, t4403.MTop, 
                t4403.CreateID AS Expr2, t4403.CreateDate AS Expr3, t4403.ModifyID AS Expr4, t4403.ModifyDate AS Expr5
FROM      dbo.T_44_014_TempPlanResult AS t4414 INNER JOIN
                dbo.T_44_002_Seat AS t4402 ON t4414.SeatID = t4402.SeatID INNER JOIN
                dbo.T_11_005_00000 AS t1134 ON t4414.AgentID = t1134.C001 INNER JOIN
                dbo.T_44_003_R_Seat AS t4403 ON t4414.SeatID = t4403.SeatID