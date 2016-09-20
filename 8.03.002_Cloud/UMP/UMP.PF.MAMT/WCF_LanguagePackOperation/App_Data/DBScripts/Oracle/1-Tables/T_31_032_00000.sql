
create table T_31_032_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(200) not null,
  c003 NUMBER(5) not null,
  c004 NUMBER(5) not null,
  c005 CHAR(1) not null,
  c006 CHAR(1) not null,
  constraint PK_31_032_0 primary key (C001)
);
comment on table T_31_032_00000
  is '对新任务夹附加时长和录音条数的限制';
comment on column T_31_032_00000.c001
  is '附加条件，主键自增';
comment on column T_31_032_00000.c002
  is '存储条件参数,以逗号隔开';
comment on column T_31_032_00000.c003
  is '1代表=,2代表>=,3代表>,4代表<=,5代表< 6 between';
comment on column T_31_032_00000.c004
  is '附加条件的优先级';
comment on column T_31_032_00000.c005
  is 'Y为启用 N为禁用';
comment on column T_31_032_00000.c006
  is 'R为录音条数的限制，L为录音时长的限制';
