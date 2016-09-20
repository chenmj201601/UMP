
create table T_31_019_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20) not null,
  c005 NUMBER(20) not null,
  c006 NUMBER(20) default -1 not null,
  c007 NVARCHAR2(1024),
  c008 NUMBER(5) default -1 not null,
  c009 NUMBER(20) default -1 not null,
  c010 NVARCHAR2(1024),
  c011 DATE,
  c012 NUMBER(5) default -1 not null,
  c013 NUMBER(20) default -1 not null,
  c014 NVARCHAR2(1024),
  c015 DATE,
  c016 NUMBER(5) default -1 not null,
  c017 NUMBER(20) default -1 not null,
  c018 NVARCHAR2(1024),
  c019 DATE,
  c020 NUMBER(5) default -1 not null,
  c021 NUMBER(20) default -1 not null,
  c022 DATE,
  c023 NVARCHAR2(1024),
  c024 NUMBER(5) default -1 not null,
  c025 NUMBER(5) default -1 not null,
  c026 CHAR(1),
  c027 NUMBER(20)
);
comment on table T_31_019_00000
  is '申诉详情表';
comment on column T_31_019_00000.c001
  is '主键自增';
comment on column T_31_019_00000.c002
  is 'T_31_008_00000.c001评分成绩表的成绩ID';
comment on column T_31_019_00000.c003
  is '录音记录表的ID,T_21_001.C001';
comment on column T_31_019_00000.c004
  is '录音所属座席工号,对应录音表的T_21_001.c039';
comment on column T_31_019_00000.c005
  is '申诉流程的那一步';
comment on column T_31_019_00000.c006
  is '申诉人ID,如果是座席自己申诉的，填写T_11_101_00000.C001';
comment on column T_31_019_00000.c007
  is '申诉内容';
comment on column T_31_019_00000.c008
  is '申诉动作 1是自己申诉，2是他人替申诉,T_31_037_00000.C001';
comment on column T_31_019_00000.c009
  is '第一复核人ID,T_11_005_00000.c00';
comment on column T_31_019_00000.c010
  is '第一复核人备注内容';
comment on column T_31_019_00000.c011
  is '第一复核人复核时间';
comment on column T_31_019_00000.c012
  is '第一复核人的动作,T_31_037_00000.C001';
comment on column T_31_019_00000.c013
  is '第二复核人ID,T_11_005_00000.c00';
comment on column T_31_019_00000.c014
  is '第二复核人备注内容';
comment on column T_31_019_00000.c015
  is '第二复核人复核时间';
comment on column T_31_019_00000.c016
  is '第二复核人的动作,T_31_037_00000.C001';
comment on column T_31_019_00000.c017
  is '第三复核人ID,T_11_005_00000.c00';
comment on column T_31_019_00000.c018
  is '第三复核人备注内容';
comment on column T_31_019_00000.c019
  is '第三复核人复核时间';
comment on column T_31_019_00000.c020
  is '第三复核人的动作,T_31_037_00000.C001';
comment on column T_31_019_00000.c021
  is '审批人ID,T_11_005_00000.c001';
comment on column T_31_019_00000.c022
  is '审批时间';
comment on column T_31_019_00000.c023
  is '审批备注';
comment on column T_31_019_00000.c024
  is '审批动作ID,T_31_037_00000.C001';
comment on column T_31_019_00000.c025
  is '当能多次申诉时启用，每再申诉一次+1';
comment on column T_31_019_00000.c026
  is 'Y为申诉流程完毕，N 在处理流程中';
comment on column T_31_019_00000.c027
  is '申诉流程的那一步T_031_017_00000.C001';
alter table T_31_019_00000
  add constraint PK_31_019_0 primary key (C001);