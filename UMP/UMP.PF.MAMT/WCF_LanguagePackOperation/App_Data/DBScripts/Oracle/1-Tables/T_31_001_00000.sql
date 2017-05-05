
create table T_31_001_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(1024) not null,
  c003 CHAR(1) default 'T' not null,
  c004 NUMBER(10,4),
  c005 NUMBER(20),
  c006 DATE,
  c007 DATE,
  c008 NUMBER(20),
  c009 DATE,
  c010 DATE,
  c011 NUMBER(10,4) not null,
  c012 NUMBER(5) not null,
  c013 CHAR(1) default 'Y' not null,
  c014 CHAR(1) default 'S' not null,
  c015 NUMBER(20) not null,
  c016 DATE,
  c017 NUMBER(20) not null,
  c018 CHAR(1) default 'N',
  constraint PK_31_001_0 primary key (C001)
);
comment on table T_31_001_00000
  is '评分表';
comment on column T_31_001_00000.c001
  is '主键自增ID,评分表ID';
comment on column T_31_001_00000.c002
  is '评分表名称';
comment on column T_31_001_00000.c003
  is '评分表样式T：树状评分表C：交叉表 ';
comment on column T_31_001_00000.c004
  is '评分表总分，当为纯是否题时可以为0';
comment on column T_31_001_00000.c005
  is '评分表创建人ID,对应T_11_005_00000.c001';
comment on column T_31_001_00000.c006
  is '评分表创建时间';
comment on column T_31_001_00000.c007
  is '最后修改时间';
comment on column T_31_001_00000.c008
  is '最后修改人的ID,对应T_11_005_00000.c001';
comment on column T_31_001_00000.c009
  is '评分表启用开始时间';
comment on column T_31_001_00000.c010
  is '评分表的过期时间';
comment on column T_31_001_00000.c011
  is '合格线，为纯是否题时则为多少个是算是合格的数量';
comment on column T_31_001_00000.c012
  is '评分表子项总数';
comment on column T_31_001_00000.c013
  is '评分表启用/禁用  Y/N';
comment on column T_31_001_00000.c014
  is 'P：百分比 F:纯是非 S:数值';
comment on column T_31_001_00000.c015
  is '机构部门ID,对应T_11_006_00000.c001';
comment on column T_31_001_00000.c016
  is '最后一次使用时间';
comment on column T_31_001_00000.c017
  is '0表示未使用的，用一次+1';
comment on column T_31_001_00000.c018
  is '评分表是否完整 Y完整 N不完整';
