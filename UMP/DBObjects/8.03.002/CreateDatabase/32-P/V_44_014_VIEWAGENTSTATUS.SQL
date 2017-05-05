CREATE VIEW V_44_014_VIEWAGENTSTATUS AS 
SELECT   t4418.ID, t4418.staNumber, t4418.StaName, t4418.Color, t4418.icopath, t4418.Remark, t4418.CreateID, t4418.CreateDate,
                t4418.ModifyID, t4418.ModifyDate, t4418.Param1, t4418.Param2, t1134.c003 AS modifyUserName,
                t4418.IsWorkTime, t4418.PID, t4418.AlarmTimeLength, t4418.TwoAlarmTimeLength, t4418.Status,
                t4418a.StaName AS PstaName
FROM      T_44_018_AgentStatus  t4418 LEFT OUTER JOIN
                 T_11_005_00000  t1134 ON t4418.ModifyID = t1134.c001 LEFT OUTER JOIN
                 T_44_018_AgentStatus  t4418a ON t4418.PID = t4418a.ID
