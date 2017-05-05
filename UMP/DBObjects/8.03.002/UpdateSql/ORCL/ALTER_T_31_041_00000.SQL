--增加T_31_041_00000主键 2016/05/31 cloud
drop index PK_31_041_00000;
alter table T_31_041_00000 add constraint  PK_31_041_00000 primary key (C001);

--PENGXI 20160907 增加一个字段：是否禁止修改该条评分(0：允许修改；1：禁止修改)  
alter table T_31_041_00000 add C018 char(1) default '0';