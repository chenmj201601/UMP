
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
  is '���������';
comment on column T_31_019_00000.c001
  is '��������';
comment on column T_31_019_00000.c002
  is 'T_31_008_00000.c001���ֳɼ���ĳɼ�ID';
comment on column T_31_019_00000.c003
  is '¼����¼���ID,T_21_001.C001';
comment on column T_31_019_00000.c004
  is '¼��������ϯ����,��Ӧ¼�����T_21_001.c039';
comment on column T_31_019_00000.c005
  is '�������̵���һ��';
comment on column T_31_019_00000.c006
  is '������ID,�������ϯ�Լ����ߵģ���дT_11_101_00000.C001';
comment on column T_31_019_00000.c007
  is '��������';
comment on column T_31_019_00000.c008
  is '���߶��� 1���Լ����ߣ�2������������,T_31_037_00000.C001';
comment on column T_31_019_00000.c009
  is '��һ������ID,T_11_005_00000.c00';
comment on column T_31_019_00000.c010
  is '��һ�����˱�ע����';
comment on column T_31_019_00000.c011
  is '��һ�����˸���ʱ��';
comment on column T_31_019_00000.c012
  is '��һ�����˵Ķ���,T_31_037_00000.C001';
comment on column T_31_019_00000.c013
  is '�ڶ�������ID,T_11_005_00000.c00';
comment on column T_31_019_00000.c014
  is '�ڶ������˱�ע����';
comment on column T_31_019_00000.c015
  is '�ڶ������˸���ʱ��';
comment on column T_31_019_00000.c016
  is '�ڶ������˵Ķ���,T_31_037_00000.C001';
comment on column T_31_019_00000.c017
  is '����������ID,T_11_005_00000.c00';
comment on column T_31_019_00000.c018
  is '���������˱�ע����';
comment on column T_31_019_00000.c019
  is '���������˸���ʱ��';
comment on column T_31_019_00000.c020
  is '���������˵Ķ���,T_31_037_00000.C001';
comment on column T_31_019_00000.c021
  is '������ID,T_11_005_00000.c001';
comment on column T_31_019_00000.c022
  is '����ʱ��';
comment on column T_31_019_00000.c023
  is '������ע';
comment on column T_31_019_00000.c024
  is '��������ID,T_31_037_00000.C001';
comment on column T_31_019_00000.c025
  is '���ܶ������ʱ���ã�ÿ������һ��+1';
comment on column T_31_019_00000.c026
  is 'YΪ����������ϣ�N �ڴ���������';
comment on column T_31_019_00000.c027
  is '�������̵���һ��T_031_017_00000.C001';
alter table T_31_019_00000
  add constraint PK_31_019_0 primary key (C001);