CREATE VIEW V_44_011_VIEWPLANSEATPOSTIONI AS 
SELECT   RSID, RegionID, SeatID, MLeft, MTop, CreateID, CreateDate, ModifyID, ModifyDate, PlanID, SeatCode
FROM      (SELECT DISTINCT
                                 t4403.RSID, t4403.RegionID, t4403.SeatID, t4403.MLeft, t4403.MTop, t4403.CreateID, t4403.CreateDate,
                                 t4403.ModifyID, t4403.ModifyDate, t4408.PlanID, t4402.SeatCode
                 FROM      T_44_008_PlanSeats  t4408 INNER JOIN
                                 T_44_002_Seat  t4402 ON t4408.SeatID = t4402.SeatID AND t4408.Type = 0 INNER JOIN
                                 T_44_001_Manager_Region  t4401 ON t4402.OrgID = t4401.OrgID AND t4401.IsDefault = 1 INNER JOIN
                                 T_44_003_R_Seat  t4403 ON t4401.RegionID = t4403.RegionID AND t4408.SeatID = t4403.SeatID
                 UNION
                 SELECT   t4403.RSID, t4403.RegionID, t4403.SeatID, t4403.MLeft, t4403.MTop, t4403.CreateID, t4403.CreateDate,
                                 t4403.ModifyID, t4403.ModifyDate, t4408.PlanID, t4402.SeatCode
                 FROM      T_44_008_PlanSeats t4408 INNER JOIN
                                 T_44_001_Manager_Region t4401 ON t4408.SeatID = t4401.OrgID AND t4408.Type = 1 INNER JOIN
                                 T_44_003_R_Seat t4403 on t4401.RegionID = t4403.RegionID AND t4401.IsDefault = 1 INNER JOIN
                                 T_44_002_Seat t4402 on t4403.SeatID = t4402.SeatID)  temp