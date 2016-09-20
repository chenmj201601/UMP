CREATE VIEW V_44_013_VIEWAUTOPLANTASKHISTO AS 
SELECT   t4417.ID, t4417.PlanID, t4417.Runer, t4417.RunDate, t4417.RunResult, t4417.ShiftNo, t4417.SeatNo, t4417.DayNo,
                t4417.AgentNo, t4407.PlanName, t1134.C003 AS UserFullName
FROM      T_44_017_AutoPlanTaskHistory t4417 INNER JOIN
                T_44_007_Plan t4407 ON t4417.PlanID = t4407.PlanID INNER JOIN
                T_11_005_00000 t1134 ON t4417.Runer = t1134.C001
