--pengxi 20160801
delete from T_00_003 where c001=111000300 and c002=12;
insert into T_00_003 (C001, C002, C003, C004, C005, C006, C007)
values ('111000300', '12', '0', '1', '0', '12', 'Gerneral Tapi');

--pengxi 20160802
update T_00_010 set C009=2048 where C001=225 and C002=221 and C003=4;
update T_00_011 set C015=2048 where C001=221 and C002 = 25;