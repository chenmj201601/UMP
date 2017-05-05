CREATE VIEW V_44_008_ViewScheduleSeatcode AS 
SELECT   t4420.ID, t4420.AgentID, t4420.AgentName, t4420.EmploymentNumber, t4420.Shift, t4420.Activity, t4420.ShiftDate, 
                t4420.ShiftStartTime, t4420.ShiftEndTime, t4420.SeatID, t4402.SeatCode
FROM      dbo.T_44_020_Schedule AS t4420 LEFT OUTER JOIN
                dbo.T_44_002_Seat AS t4402 ON t4420.SeatID = t4402.SeatID