--2016.5.4
alter table T_11_012_00000 modify c002 NUMBER(5);
alter table T_11_012_00000 drop constraint PK_11_102_00000;
alter table T_11_012_00000 add constraint PK_11_102_00000 primary key (C001, C002, C003, C004);