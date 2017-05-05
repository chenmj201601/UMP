
create table T_31_023_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(5) not null,
  c003 CHAR(1) default 'Y' not null,
  c004 DATE,
  c005 DATE,
  c006 NUMBER(20) not null,
  c007 CHAR(1) not null,
  c008 CHAR(1) default 'N' not null,
  c009 CHAR(1) default 'N' not null,
  c010 VARCHAR2(2000),
  c011 VARCHAR2(2000),
  c012 VARCHAR2(2000),
  c013 NUMBER(20),
  c014 NUMBER(20) default 1 not null,
  c015 NUMBER(20) default 1 not null,
  c016 CHAR(1) default 'N' not null,
  c017 NUMBER(5),
  c018 NUMBER(5),
  c019 NUMBER(5),
  c020 CHAR(1) default 'N' not null,
  c021 NVARCHAR2(50),
  c022 NVARCHAR2(1024),
  c023 CHAR(1)
);
comment on table T_31_023_00000
  is '自动任务分配以及智能任务分配设置表';
comment on column T_31_023_00000.c001
  is '主键自增,自动任务分配ID';
comment on column T_31_023_00000.c002
  is '1 自动初检任务 2自动复检任务 3智能任务分配（新任务夹)';
comment on column T_31_023_00000.c003
  is '启用/禁用  Y/N';
comment on column T_31_023_00000.c004
  is '该配置的启用开始时间';
comment on column T_31_023_00000.c005
  is '该配置的启用过期时间';
comment on column T_31_023_00000.c006
  is '创建人，对应 T_11_005_00000.C001';
comment on column T_31_023_00000.c007
  is '是否共享任务 Y共享任务  N非共享任务';
comment on column T_31_023_00000.c008
  is '是否平均分配 Y平均分配  N 多人发送的一样的录音';
comment on column T_31_023_00000.c009
  is '平均分配不完的录音处理办法  Y丢弃 N随机平均到质检任务里';
comment on column T_31_023_00000.c010
  is '质检人员ID,以逗号隔开,当为任务共享时，可以没质检人员';
comment on column T_31_023_00000.c011
  is '质检人员ID,以逗号隔开';
comment on column T_31_023_00000.c012
  is '质检人员ID,以逗号隔开';
comment on column T_31_023_00000.c013
  is '该分数创建时该创建人所属部门T_11_006_00000.C001';
comment on column T_31_023_00000.c014
  is '查询参数ID,对应 T_31_024_00000.C001';
comment on column T_31_023_00000.c015
  is '运行周期ID,对应T_31_026_00000.C001';
comment on column T_31_023_00000.c016
  is 'Y为旬设置 N非旬设置';
comment on column T_31_023_00000.c017
  is '上旬每座席分配数量';
comment on column T_31_023_00000.c018
  is '中旬每座席分配数量';
comment on column T_31_023_00000.c019
  is '下旬每座席分配数量';
comment on column T_31_023_00000.c020
  is 'Y为当上旬不足，中旬补齐 ，中旬不足时，下旬补齐 N为不补';
comment on column T_31_023_00000.c021
  is '任务名称';
comment on column T_31_023_00000.c022
  is '任务描述';
comment on column T_31_023_00000.c023
  is '当该任务为新任务夹时 ，用于两个不同任务夹记录，是否能有相同的记录 Y能，N不能';
alter table T_31_023_00000
  add constraint PK_T_31_023_0 primary key (C001);
