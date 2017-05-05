
create table T_31_005_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) default 0 not null,
  c003 CHAR(1),
  c004 NVARCHAR2(2000) not null,
  constraint PK_31_005_0 primary key (C001)
);
comment on table T_31_005_00000
  is '备注信息子表';
comment on column T_31_005_00000.c001
  is '但评论为下拉风格等风格时，顺序ID，从1开始';
comment on column T_31_005_00000.c002
  is '备注表ID对应T_31_004_00000.C001';
comment on column T_31_005_00000.c003
  is '为非文本风格时，是否该备注子项被选中 Y为选中 N为不选中';
comment on column T_31_005_00000.c004
  is '标题文本';
