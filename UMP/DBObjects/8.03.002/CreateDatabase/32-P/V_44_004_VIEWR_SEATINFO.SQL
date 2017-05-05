CREATE VIEW V_44_004_VIEWR_SEATINFO AS 
SELECT   t44003.RSID, t44003.RegionID, t44003.SeatID, t44003.MLeft, t44003.MTop, t44003.CreateID, t44003.CreateDate,
                t44003.ModifyID, t44003.ModifyDate, t44002.SeatCode, t44002.Ext
FROM      T_44_003_R_Seat t44003 INNER JOIN
                T_44_002_Seat t44002 ON t44003.SeatID = t44002.SeatID
