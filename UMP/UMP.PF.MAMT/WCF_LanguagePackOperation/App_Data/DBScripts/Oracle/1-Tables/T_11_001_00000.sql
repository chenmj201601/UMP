create table T_11_001_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(11) not null,
  C004 NUMBER(11) not null,
  C005 NUMBER(5) not null,
  C006 NVARCHAR2(1024) not null,
  C007 NUMBER(5) not null,
  C008 NVARCHAR2(1024),
  C009 NUMBER(5) not null,
  C010 CHAR(1) not null,
  C011 NUMBER(11) not null,
  C012 VARCHAR2(1024),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 CHAR(1) not null,
  C016 VARCHAR2(512),
  C017 VARCHAR2(512),
  C018 VARCHAR2(512),
  C019 NUMBER(20),
  C020 NUMBER(20),
  C021 NVARCHAR2(1024) not null,
  C022 NVARCHAR2(1024),
  constraint PK_11_001_0 primary key (C001, C002, C003)
);
-- Add comments to the table 
comment on table T_11_001_00000
  is '全局参数表';
-- Add comments to the columns 
comment on column T_11_001_00000.C001
  is '租户编号，如果 = 0 则为主用户的参数配置';
comment on column T_11_001_00000.C002
  is '模块编号';
comment on column T_11_001_00000.C003
  is '参数编号';
comment on column T_11_001_00000.C004
  is '参数所在组编号（默认值 0，如果需要对该表中的参数进行分组，那么这里填写分组编号）';
comment on column T_11_001_00000.C005
  is '组内排序序号（默认值 0，在某一分组的排列顺序）';
comment on column T_11_001_00000.C006
  is '参数值';
comment on column T_11_001_00000.C007
  is '数据类型（1：smallint；2：int；3：bigint；4：number；11：char；12：nchar；13：varchar；14：nvarchar；21：datetime）';
comment on column T_11_001_00000.C008
  is '参数有效性检查方法，待考虑和设计';
comment on column T_11_001_00000.C009
  is '显示格式转换（默认值 0， UMP系统中不使用，DBTool使用）';
comment on column T_11_001_00000.C010
  is '租户是否可以继承和配置该参数，''1'' 可以，''0'' 不可以';
comment on column T_11_001_00000.C011
  is '数据从BasicInformation表中来源的需要';
comment on column T_11_001_00000.C012
  is '正则表达式验证';
comment on column T_11_001_00000.C013
  is '参数值范围 - 最小(如果有必要校验 ParamValue ， 则在这里填写)';
comment on column T_11_001_00000.C014
  is '参数值范围 - 最大(如果有必要校验 ParamValue ， 则在这里填写)';
comment on column T_11_001_00000.C015
  is '该参数值改变后，租户参数是否必须与此值相同，也就是一起改变 ''1'' 改变，''0'' 不改变';
comment on column T_11_001_00000.C016
  is '该参数值修改后，可以同步到哪些模块，只针对 ModuleID = 11 有效';
comment on column T_11_001_00000.C017
  is '该参数值修改后，可以同步到哪些模块，只针对 ModuleID != 11 有效';
comment on column T_11_001_00000.C018
  is '最后修改时间';
comment on column T_11_001_00000.C019
  is '最后修改人';
comment on column T_11_001_00000.C020
  is 'Key Value Change ID';
comment on column T_11_001_00000.C021
  is '生效时间。加密保存';
comment on column T_11_001_00000.C022
  is '设置值,到达生效时间后,覆盖 C006 的值,该字段清空';
