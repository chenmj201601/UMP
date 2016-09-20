create table *****
(
  c001 NUMBER(20) not null,
  c002 NUMBER(11) not null,
  c003 NUMBER(11) default 0 not null,
  c004 NUMBER(5) default 0 not null,
  c005 NVARCHAR2(1024) not null,
  c006 NUMBER(5) not null,
  c007 NVARCHAR2(1024),
  c008 NUMBER(5) default 0 not null,
  c009 NUMBER(11) default 0 not null,
  c010 VARCHAR2(1024),
  c011 NVARCHAR2(1024),
  c012 NVARCHAR2(1024),
  c013 VARCHAR2(512),
  c014 NUMBER(20),
  c015 VARCHAR2(512),
  c016 NVARCHAR2(1024),
  constraint PK_***** primary key (C001, C002)
);
-- Add comments to the table 
comment on table *****
  is '用户参数表';
-- Add comments to the columns 
comment on column *****.c001
  is '用户编号（19位）';
comment on column *****.c002
  is '参数编号';
comment on column *****.c003
  is '参数所在组编号（默认值 0，如果需要对该表中的参数进行分组，那么这里填写分组编号）';
comment on column *****.c004
  is '组内排序序号（默认值 0，在某一分组的排列顺序）';
comment on column *****.c005
  is '参数值';
comment on column *****.c006
  is '数据类型（1：smallint；2：int；3：bigint；4：number；11：char；12：nchar；13：varchar；14：nvarchar；21：datetime）';
comment on column *****.c007
  is '参数有效性检查方法，待考虑和设计';
comment on column *****.c008
  is '显示格式转换（默认值 0）';
comment on column *****.c009
  is '数据从BasicInformation表中来源的需要';
comment on column *****.c010
  is '正则表达式验证';
comment on column *****.c011
  is '参数值范围 - 最小(如果有必要校验 ParamValue ， 则在这里填写)';
comment on column *****.c012
  is '参数值范围 - 最大(如果有必要校验 ParamValue ， 则在这里填写)';
comment on column *****.c013
  is '最后修改时间';
comment on column *****.c014
  is 'Key Value Change ID';
comment on column *****.c015
  is '生效时间。加密保存';
comment on column *****.c016
  is '设置值,到达生效时间后,覆盖 C005 的值,该字段清空';


