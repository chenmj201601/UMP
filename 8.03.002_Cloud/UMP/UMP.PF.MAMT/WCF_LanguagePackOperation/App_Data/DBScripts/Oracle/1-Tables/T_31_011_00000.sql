create table T_31_011_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NVARCHAR2(2000) not null,
  c004 NUMBER(5) not null
);
comment on table T_31_011_00000
  is '成绩备注表';
comment on column T_31_011_00000.c001
  is '评分成绩表ID,对应T_31_006_00000.c001';
comment on column T_31_011_00000.c002
  is '备注表ID,对应T_31_004_00000.c001';
comment on column T_31_011_00000.c003
  is '文本项时备注里的内容';
comment on column T_31_011_00000.c004
  is '备注信息子表ID,对应T_31_005_00000.C001(记录非文本风格的备注时，记录被选中的子项)';
alter table T_31_011_00000
  add constraint PK_31_011_0 primary key (C001, C002, C004);


