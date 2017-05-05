CREATE VIEW V_44_012_ViewTempAgentSeat AS 
SELECT   t4414.ID, t4414.AgentID, t4414.PlanID, t4414.SeatID, t4414.Shift, t4414.ShiftDate, t4414.StartTime, t4414.EndTime, 
                t4414.Status, t4414.CreateID, t4414.CreateDate, t4414.ModifyID, t4414.ModifyDate, v4411.RSID, v4411.RegionID, 
                v4411.MLeft, v4411.MTop, t1134.C003 as UserFullName, v4411.SeatCode, t1134.C002 as LoginAccount
FROM      dbo.T_44_014_TempPlanResult AS t4414 LEFT OUTER JOIN
                dbo.V_44_011_ViewPlanSeatPostionInfo AS v4411 ON t4414.PlanID = v4411.PlanID AND 
                t4414.SeatID = v4411.SeatID INNER JOIN
                dbo.T_11_005_00000 AS t1134 ON t4414.AgentID = t1134.C001