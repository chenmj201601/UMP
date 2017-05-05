CREATE VIEW V_44_007_VIEWPLANRESULTSEAT AS 
SELECT   t4415.ID, t4415.AgentID, t4415.PlanID, t4415.SeatID, t4415.Shift, t4415.ShiftDate, t4415.StartTime, t4415.EndTime, 
                t4415.Status, t4415.CreateID, t4415.CreateDate, t4415.ModifyID, t4415.ModifyDate, t4402.SeatCode
FROM      T_44_015_PlanResult t4415 INNER JOIN
                T_44_002_Seat t4402 ON t4415.SeatID = t4402.SeatID