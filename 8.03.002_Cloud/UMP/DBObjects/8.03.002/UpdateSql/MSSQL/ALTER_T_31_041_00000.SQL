--增加T_31_041_00000主键  2016/05/31 cloud

alter table T_31_041_00000 drop constraint PK_31_041_00000;;

ALTER TABLE T_31_041_00000 ADD  CONSTRAINT PK_31_041_00000 PRIMARY KEY CLUSTERED 
	  ([C001] ASC);
	  
--PENGXI 20160907 增加一个字段：是否禁止修改该条评分(0：允许修改；1：禁止修改)  
alter table T_31_041_00000 add C018 char(1) default '0';