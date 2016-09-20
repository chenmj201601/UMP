CREATE VIEW V_44_002_ViewRegionUserOrg AS 
SELECT   t44001.RegionID, t44001.RegionName, t44001.Description, t44001.OrgID, t44001.Type, t44001.State, t44001.Width, 
                t44001.Height, t44001.BgColor, t44001.BgimagePath, t44001.CreateID, t44001.CreateDate, t44001.ModifyID, 
                t44001.ModifyDate, t44001.IsDefault, t1134.C003 as UserFullName, t1134.C002 as LoginAccount, t11031.C002 as OrgTenantName
FROM      dbo.T_44_001_Manager_Region AS t44001 LEFT OUTER JOIN
                dbo.T_11_005_00000 AS t1134 ON t44001.ModifyID = t1134.C001 INNER JOIN
                dbo.T_11_006_00000 AS t11031 ON t44001.OrgID = t11031.C001