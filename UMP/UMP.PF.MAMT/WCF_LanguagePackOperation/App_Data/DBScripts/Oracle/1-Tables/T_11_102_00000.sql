
create table T_11_102_00000
(
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(5) not null,
  C004 CHAR(1) not null,
  C005 NVARCHAR2(1024),
  C006 NUMBER(5) not null,
  C007 NUMBER(5) not null,
  C008 NUMBER(5),
  C009 NVARCHAR2(1024),
  C010 NUMBER(11) not null,
  C011 VARCHAR2(1024),
  C012 NVARCHAR2(1024),
  C013 NVARCHAR2(1024),
  C014 NVARCHAR2(1024),
  C015 NVARCHAR2(1024),
  C016 DATE not null,
  C017 NUMBER(20),
 constraint PK_11_102_00000 primary key (C001, C002)
);
comment on table T_11_102_00000
  is '系统资源属性，每个属性的定义';
comment on column T_11_102_00000.C001
  is '资源类型。';
comment on column T_11_102_00000.C002
  is '属性编号（1-90）';
comment on column T_11_102_00000.C003
  is '数据类型（1：smallint；2：int；3：bigint；4：number；11：char；12：nchar；13：varchar；14：nvarchar；21：datetime）';
comment on column T_11_102_00000.C004
  is '是否为参数';
comment on column T_11_102_00000.C005
  is '参数有效性检查方法，待考虑和设计';
comment on column T_11_102_00000.C006
  is '显示格式转换（默认值 0， UMP系统中不使用，DBTool使用）';
comment on column T_11_102_00000.C007
  is '参数所在组编号';
comment on column T_11_102_00000.C008
  is '组内排序序号';
comment on column T_11_102_00000.C009
  is '默认值';
comment on column T_11_102_00000.C010
  is '数据从BasicInformation表中来源的需要';
comment on column T_11_102_00000.C011
  is '正则表达式验证';
comment on column T_11_102_00000.C012
  is '参数值范围 - 最小(如果有必要校验 ParamValue ， 则在这里填写)';
comment on column T_11_102_00000.C013
  is '参数值范围 - 最大(如果有必要校验 ParamValue ， 则在这里填写)';
comment on column T_11_102_00000.C014
  is '生效时间。加密保存';
comment on column T_11_102_00000.C015
  is '设置值,到达生效时间后,覆盖 C006 的值,该字段清空';
comment on column T_11_102_00000.C016
  is '最后修改时间';
comment on column T_11_102_00000.C017
  is '最后修改人';
