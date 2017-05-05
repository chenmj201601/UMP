
create table T_31_022_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 CHAR(1),
  c004 NUMBER(20),
  c005 DATE not null,
  c006 NUMBER(5),
  c007 NUMBER(20)
);
comment on table T_31_022_00000
  is '任务号与录音流水号相关联的表';
comment on column T_31_022_00000.c001
  is '任务号 对应 T_31_020_00000.C001';
comment on column T_31_022_00000.c002
  is '录音流水号,对应 对应T_21_000.RecoredReference';
comment on column T_31_022_00000.c003
  is 'Y被锁定（当任务为共享时）, N没被锁定';
comment on column T_31_022_00000.c004
  is '锁定人,对应 T_11_005_00000.C001';
comment on column T_31_022_00000.c005
  is '锁定时间';
comment on column T_31_022_00000.c006
  is '1任务分配过来，2从其它任务移动过来的 3推荐录音';
comment on column T_31_022_00000.c007
  is '如果该录音从其它任务调整过来的，为调整任务ID';
alter table T_31_022_00000
  add constraint PK_T_31_022_0 primary key (C001, C002);
