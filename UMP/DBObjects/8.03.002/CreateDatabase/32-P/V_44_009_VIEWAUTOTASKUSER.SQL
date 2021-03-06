﻿CREATE VIEW V_44_009_VIEWAUTOTASKUSER AS 
SELECT   t4410.ID, t4410.PeriodValue, t4410.PeriodType, t4410.StartDate, t4410.PlanID, t4410.Status, t4410.LastDate,
                t4410.NextDate, t4410.CreateID, t4410.CreateDate, t4410.ModifyID, t4410.ModifyDate, t4410.cron, t1134.C003 AS UserFullName,
                t4407.PlanName
FROM      T_44_010_AutoPlanTaskConfig t4410 LEFT OUTER JOIN
                T_11_005_00000 t1134 ON t4410.ModifyID = t1134.C001 INNER JOIN
                T_44_007_Plan t4407 ON t4410.PlanID = t4407.PlanID