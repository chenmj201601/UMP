--20160815 pengxi
update T_11_003_00000 set C005='G3102' ,C004=9 where C001=31 and C002=3101;

delete from T_00_003 where c001='111000300' and c002='13';
insert into T_00_003 (C001, C002, C003, C004, C005, C006, C007)
values ('111000300', '13', '0', '1', '0', '13', 'HuaweiICD');