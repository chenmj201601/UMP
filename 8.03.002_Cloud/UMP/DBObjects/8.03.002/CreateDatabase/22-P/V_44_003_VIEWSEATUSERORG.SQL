CREATE VIEW V_44_003_ViewSeatUserOrg AS 
SELECT   t44002.SeatID, t44002.SeatCode, t44002.State, t44002.Description, t44002.Priority, t44002.OrgID, t44002.CreateID, 
                t44002.CreateDate, t44002.ModifyID, t44002.ModifyDate, t44002.Building, t44002.Floor, t44002.Area, 
                t1134.C003 as UserFullName, t1134.C002 as LoginAccount, t11031.C002 as OrgTenantName
FROM      dbo.T_44_002_Seat AS t44002 LEFT OUTER JOIN
                dbo.T_11_005_00000 AS t1134 ON t44002.ModifyID = t1134.C001 INNER JOIN
                dbo.T_11_006_00000 AS t11031 ON t44002.OrgID = t11031.C001