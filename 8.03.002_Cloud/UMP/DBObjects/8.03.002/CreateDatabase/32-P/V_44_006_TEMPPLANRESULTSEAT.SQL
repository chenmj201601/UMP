CREATE VIEW V_44_006_TEMPPLANRESULTSEAT AS 
SELECT   t4414.ID, t4414.AgentID, t4414.PlanID, t4414.SeatID, t4414.Shift, t4414.ShiftDate, t4414.StartTime, t4414.EndTime,
                t4414.Status, t4414.CreateID, t4414.CreateDate, t4414.ModifyID, t4414.ModifyDate, t4402.SeatCode
FROM      T_44_014_TempPlanResult t4414 INNER JOIN
                T_44_002_Seat t4402 ON t4414.SeatID = t4402.SeatID
