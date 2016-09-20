CREATE VIEW V_44_001_ViewPlanUser AS 
SELECT   T447.PlanID, T447.PlanName, T447.Status, T447.Remark, T447.CreateID, T447.CreateDate, T447.ModifyID, 
                T447.ModifyDate, T1134.C002 as LoginAccount, T1134.C003 as UserFullName
FROM      dbo.T_44_007_Plan AS T447 LEFT OUTER JOIN
                dbo.T_11_005_00000 AS T1134 ON T447.ModifyID = T1134.C001