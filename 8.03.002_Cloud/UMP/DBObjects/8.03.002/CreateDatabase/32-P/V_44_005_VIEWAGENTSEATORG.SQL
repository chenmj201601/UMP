CREATE VIEW V_44_005_VIEWAGENTSEATORG AS 
SELECT   T1134.C001 as UserID, T1134.C002 AS LoginAccount, T1134.C003 AS UserFullName, T1134.C004 AS LoginPassword, T1134.C006 AS InOrgTenant, 
                T1134.C007 AS FirstSource, T1134.C008 AS IsLocked, T1134.C009 AS LockedMethod, T1134.C010 AS IsActive, T1134.C011 AS IsDisabled, T1134.C012 AS OtherStatus, 
                T1134.C013 AS LastLoginTime, T1134.C014 AS LastLoginHost, T1134.c015 AS LastLoginIP, T1134.C016 AS LastLoginModuleID, T1134.C017 AS LimitDurationBegin, 
                T1134.C018 as LimitDurationEnd, T1134. C019 AS Creator, T1134.C020 AS CreateTime, T1134.C023 AS LastModifyTime,                  
                T1134.C021 AS EntryDateUtc, T1131.c002 as OrgTenantName, T4413.CreateDate, 
                T4413.SeatID, T4402.SeatCode, T4413.PlanID
FROM      T_44_013_AgentSeat T4413 LEFT OUTER JOIN
                T_11_005_00000 T1134 ON T4413.AgentID = T1134.C001 INNER JOIN
                T_44_002_Seat T4402 ON T4413.SeatID = T4402.SeatID INNER JOIN
                T_11_006_00000 T1131 ON T1134.c006 = T1131.c001