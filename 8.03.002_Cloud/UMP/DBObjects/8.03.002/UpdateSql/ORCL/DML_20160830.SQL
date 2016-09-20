--xue 20160901
DELETE from t_25_006 where c001=2020000000000000111;
insert into T_25_006 (C001, C002, C003, C004, C005, C006, C007, C101, C102, C103, C104, C105, C106, C107, C108, C109, C110)
values ('2020000000000000111', '3', '6411', '12', '0', '1', '日志内容', null, null, null, null, null, null, null, null, null, null);
DELETE from t_25_006 where c001=2020000000000000112;
insert into T_25_006 (C001, C002, C003, C004, C005, C006, C007, C101, C102, C103, C104, C105, C106, C107, C108, C109, C110)
values ('2020000000000000112', '3', '6411', '12', '1', '1', '日志内容异常', null, null, null, null, null, null, null, null, null, null);
COMMIT;


