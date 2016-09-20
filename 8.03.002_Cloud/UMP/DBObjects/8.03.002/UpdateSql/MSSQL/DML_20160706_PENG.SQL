--资源管理模块PBXDevice添加属性BindDevice和添加CTI类型CnCall  2016/07/06 pengxi
delete from T_00_009 where c001=220 and c002=16;
delete from T_00_003 where c001=111000300 and c002=11;


insert into T_00_009 (C001, C002, C003, C004, C005, C006, C007, C008, C009, C010, C011, C012, C013, C014, C015, C016, C017, C018, C019, C020, C021, C022, C023, C024, C025, C026, C027, C028, C029, C030)
values ('220', '16', '14', '1', null, '100', '0', '16', null, '0', null, null, null, null, null, ('2016/1/26'), '1020000000000000001', '绑定设备', '0', 'BindDevice', null, null, null, null, null, null, null, null, null, null);
insert into T_00_003 (C001, C002, C003, C004, C005, C006, C007)
values ('111000300', '11', '0', '1', '0', '11', 'CnCall');


