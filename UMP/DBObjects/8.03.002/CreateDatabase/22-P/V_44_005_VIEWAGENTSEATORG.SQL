CREATE VIEW V_44_005_ViewAgentSeatOrg AS 
SELECT   T1134.C001 as UserID, T1134.C002 as LoginAccount, T1134.C003 as UserFullName, T1134.C004 as LoginPassword, T1134.C006 as InOrgTenant, 
                T1134.C007 as FirstSource, T1134.C008 as IsLocked, T1134.C009 as LockedMethod, T1134.C010 as IsActive, T1134.C011 as IsDisabled, T1134.C012 as OtherStatus, 
                T1134.C013 as LastLoginTime, T1134.C014 as LastLoginHost, T1134.C015 as LastLoginIP, T1134.C016 as LastLoginModuleID, T1134.C017 as LimitDurationBegin, 
                T1134.C018 as LimitDurationEnd, T1134.C019 as Creator, T1134.C020 as CreateTime, T1134.C023 as LastModifyTime,                 
                T1134.C021 as EntryDateUtc, T1134.C022 as UTCOffSet,
                T4413.SeatID, T4402.SeatCode, T4413.PlanID
FROM      dbo.T_44_013_AgentSeat AS T4413 LEFT OUTER JOIN
                dbo.T_11_005_00000 AS T1134 ON T4413.AgentID = T1134.C001 INNER JOIN
                dbo.T_44_002_Seat AS T4402 ON T4413.SeatID = T4402.SeatID INNER JOIN
                dbo.T_11_006_00000 AS T1131 ON T1134.C006 = T1131.C001