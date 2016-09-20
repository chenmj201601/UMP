
create table T_31_004_00000
(
  c001 NUMBER(20) default 0 not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20),
  c004 NUMBER(5) not null,
  c005 NVARCHAR2(2000),
  c006 NUMBER(5),
  c007 NUMBER(5),
  c008 CHAR(1) not null,
  c009 NVARCHAR2(2000),
  c010 CHAR(1) not null,
  constraint PK_31_004_0 primary key (C001)
);
comment on table PT_31_004_00000
  is '备注表';
comment on column PT_31_004_00000.c001
  is '主键自增ID,评论备注ID';
comment on column PT_31_004_00000.c002
  is '评分表子项ID,对应 T_31_002_00000.c001';
comment on column PT_31_004_00000.c003
  is '评分表ID,对应T_31_001_00000.c001';
comment on column PT_31_004_00000.c004
  is '当一个点上有多个备注时，备注的顺序';
comment on column PT_31_004_00000.c005
  is '默认文本框内的内容';
comment on column PT_31_004_00000.c006
  is '为文本风格时，备注的字节长度,最大值为2000';
comment on column PT_31_004_00000.c007
  is '为文本风格时，备注输入最小的字节长度';
comment on column PT_31_004_00000.c008
  is '1为文本风格 2为下拉风格 3为复选风格 4为单选风格';
comment on column PT_31_004_00000.c009
  is '评论备注名称';
comment on column PT_31_004_00000.c010
  is ' Y为评分表的评论备注，N为评分表子项的评论备注';
