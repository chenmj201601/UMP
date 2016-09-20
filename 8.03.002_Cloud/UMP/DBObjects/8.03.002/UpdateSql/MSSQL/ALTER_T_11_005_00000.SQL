--增加T_11_005_00000增加两个字段  2016/08/22 cloud	832功能不需要在831里体现

alter table T_11_005_00000 add C027 char(1) default 0;
alter table T_11_005_00000 add C028 bigint default 0;