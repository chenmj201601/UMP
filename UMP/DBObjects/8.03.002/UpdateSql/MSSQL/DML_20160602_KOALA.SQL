-- 隐藏“PCM文件参数”配置 2016/06/02 KOALA
update t_00_009 set C004 = 0 where C001 = 225 and C002 = 32;

--BRI中增加“反转呼叫方向”配置 2016/06/02 KOALA
delete from T_00_009 where c001=225 and c002=179;
insert into T_00_009 (C001, C002, C003, C004, C005, C006, C007, C008, C009, C010, C011, C012, C013, C014, C015, C016, C017, C018, C019, C020, C021, C022, C023, C024, C025, C026, C027, C028, C029, C030)
values ('225', '179', '2', '1', null, '201', '17', '2', '0', '0', null, null, null, null, null, '2016-01-01', '1020000000000000001', N'反转呼叫方向', '0', 'ReverseDirection', null, null, null, null, null, null, null, null, null, null);

