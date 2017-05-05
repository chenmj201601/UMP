
create table T_31_026_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) not null,
  c003 DATE not null,
  c004 NUMBER(5),
  c005 NUMBER(5),
  c006 CHAR(1),
  c007 NUMBER(5),
  c008 NUMBER(5),
  c009 NUMBER(5),
  c010 NUMBER(5),
  c011 CHAR(1),
  c012 NUMBER(5),
  c013 NUMBER(5),
  c014 NUMBER(5),
  c015 NUMBER(5),
  c016 NUMBER(5),
  c017 NUMBER(5)
);
comment on table T_31_026_00000
  is '所有任务以及服务类运行时间及周期';
comment on column T_31_026_00000.c001
  is '主键自增，运行周期ID';
comment on column T_31_026_00000.c002
  is '运行周期 D:天,W:周,P:旬,M:月,S:季 Y:月  O:只运行一次';
comment on column T_31_026_00000.c003
  is '运行时间';
comment on column T_31_026_00000.c004
  is '每周的星期几运行 1~7';
comment on column T_31_026_00000.c005
  is '每月的第几天运行1~31,那个月没有29，30，31则那个月的最后一天运行';
comment on column T_31_026_00000.c006
  is '是否分别设置旬运行 Y旬分开设置 N统一旬设置';
comment on column T_31_026_00000.c007
  is '统一设置的时间 1~11, 如果那个旬没有9，10，11则旬的最后一天运行';
comment on column T_31_026_00000.c008
  is '上旬的第几天运行1~10';
comment on column T_31_026_00000.c009
  is '中旬的第几天运行1~10';
comment on column T_31_026_00000.c010
  is '下旬的第几天运行1~11，下旬没有9，10，11的，按最后一天运行';
comment on column T_31_026_00000.c011
  is '是否分别设置旬运行 Y季分开设置 N统一季设置';
comment on column T_31_026_00000.c012
  is '统一设置的时间 0~92, 如果那个季没有90，91，92，则取那个季的最后一天运行';
comment on column T_31_026_00000.c013
  is '第一季的第几天';
comment on column T_31_026_00000.c014
  is '第二季
的第几天';
comment on column T_31_026_00000.c015
  is '第三季的第几天';
comment on column T_31_026_00000.c016
  is '第四季的第几天';
comment on column T_31_026_00000.c017
  is '年的第几天运行(0~366)';
alter table T_31_026_00000
  add constraint PK_T_31_026_0 primary key (C001)
