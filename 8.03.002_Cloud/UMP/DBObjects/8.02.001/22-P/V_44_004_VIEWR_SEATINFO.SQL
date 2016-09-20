CREATE VIEW V_44_004_ViewR_SeatInfo AS 
SELECT   t44003.RSID, t44003.RegionID, t44003.SeatID, t44003.MLeft, t44003.MTop, t44003.CreateID, t44003.CreateDate, 
                t44003.ModifyID, t44003.ModifyDate, t44002.SeatCode, t44002.Ext
FROM      dbo.T_44_003_R_Seat AS t44003 INNER JOIN
                dbo.T_44_002_Seat AS t44002 ON t44003.SeatID = t44002.SeatID