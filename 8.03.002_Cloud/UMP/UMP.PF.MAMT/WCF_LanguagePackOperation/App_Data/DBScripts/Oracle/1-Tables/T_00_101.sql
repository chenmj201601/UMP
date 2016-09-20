create table T_00_101
(
  C001 NUMBER(5) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(5) not null,
  C004 CHAR(1),
  constraint PK_00_101 primary key (C002)
);
comment on table T_00_101
  is '可视化实体或视图';
comment on column T_00_101.c001
  is '模块编号';
comment on column T_00_101.c002
  is '实体/视图编号';
comment on column T_00_101.c003
  is '实体分组';
comment on column T_00_101.c004
  is '用户实体 = ''1''，系统维护实体 = ''S''';
create unique index IDX_00_101_C0102 on T_00_101 (C001, C002)
