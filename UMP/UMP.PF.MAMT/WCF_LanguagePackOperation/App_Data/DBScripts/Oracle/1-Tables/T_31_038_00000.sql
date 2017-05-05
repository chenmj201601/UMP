
create table T_31_038_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 DATE not null,
  c005 NUMBER(10) not null,
  c006 NUMBER(5) default 1,
  constraint PK_31_038_0 primary key (C001)
);
comment on table T_31_038_00000
  is '查询历史播放列表';
comment on column T_31_038_00000.c001
  is '主键自增,播放历史ID';
comment on column T_31_038_00000.c002
  is '列名以逗号隔开';
comment on column T_31_038_00000.c003
  is '存储列的宽度';
comment on column T_31_038_00000.c004
  is '播放日期';
comment on column T_31_038_00000.c005
  is '播放时长';
comment on column T_31_038_00000.c006
  is '1为查询播放;2 CQC播放';
