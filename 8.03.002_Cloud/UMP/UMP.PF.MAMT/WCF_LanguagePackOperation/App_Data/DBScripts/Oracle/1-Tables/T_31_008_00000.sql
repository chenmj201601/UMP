
create table T_31_008_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(10,2) not null,
  c005 NUMBER(20),
  c006 DATE,
  c007 CHAR(1),
  c008 NUMBER(10,2) not null,
  c009 CHAR(1) not null,
  c010 CHAR(1),
  c011 CHAR(1),
  c012 NUMBER(20) not null,
  c013 NUMBER(20) not null,
  c014 CHAR(1),
  c015 NUMBER(10),
  c016 NUMBER(10),
  constraint PK_T_31_008_0 primary key (C001)
);
comment on table T_31_008_00000
  is '评分成绩表';
comment on column T_31_008_00000.c001
  is '主键自增ID,成绩ID';
comment on column T_31_008_00000.c002
  is '录音ID,录音记录接口表的录音流水号';
comment on column T_31_008_00000.c003
  is '评分表ID,对应T_31_001_00000.C001';
comment on column T_31_008_00000.c004
  is '评分成绩';
comment on column T_31_008_00000.c005
  is '打分人ID,对应T_11_005_00000.C001';
comment on column T_31_008_00000.c006
  is '打分时间';
comment on column T_31_008_00000.c007
  is 'Y为控制项打分,N非控件项成绩';
comment on column T_31_008_00000.c008
  is '评分表有附加分时，显示附加分累加后成绩';
comment on column T_31_008_00000.c009
  is 'Y为最终分，N非最终分';
comment on column T_31_008_00000.c010
  is '1查询评分，2查询修改得分，3初检评分,4初检修改得分，5复检评分,6复检修改得分，7申诉评分';
comment on column T_31_008_00000.c011
  is 'Y表示该成绩打异常分数标志';
comment on column T_31_008_00000.c012
  is '该分数创建时该座席所属部门T_11_006_00000.C001';
comment on column T_31_008_00000.c013
  is '该录音所属座席的ID,对应T_11_005_00000.C001';
comment on column T_31_008_00000.c014
  is '是否被申诉，Y申诉，N否 C表未申诉完成';
comment on column T_31_008_00000.c015
  is '在申诉里的那步';
comment on column T_31_008_00000.c016
  is '整个评分花费时间，单位秒';
