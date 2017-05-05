
create table T_31_025_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NVARCHAR2(1024),
  c004 NUMBER(20) not null,
  c005 DATE not null,
  c006 DATE,
  c007 NUMBER(20),
  c008 NUMBER(5),
  c009 NVARCHAR2(1024),
  c010 CHAR(1)
);
comment on table T_31_025_00000
  is '推荐录音列表';
comment on column T_31_025_00000.c001
  is '主键自增,推荐记录ID';
comment on column T_31_025_00000.c002
  is '录音流水号,对应 对应T_21_000.RecoredReference';
comment on column T_31_025_00000.c003
  is '推荐理由';
comment on column T_31_025_00000.c004
  is '推荐人ID,对应 T_11_034_BU.UserID';
comment on column T_31_025_00000.c005
  is '推荐时间';
comment on column T_31_025_00000.c006
  is '处理时间';
comment on column T_31_025_00000.c007
  is '处理人ID,对应 T_11_034_BU.UserID';
comment on column T_31_025_00000.c008
  is '处理动作ID';
comment on column T_31_025_00000.c009
  is '处理备注';
comment on column T_31_025_00000.c010
  is 'Y表示已经处理， N表示未处理';
alter table T_31_025_00000
  add constraint PK_31_025_0 primary key (C001);
