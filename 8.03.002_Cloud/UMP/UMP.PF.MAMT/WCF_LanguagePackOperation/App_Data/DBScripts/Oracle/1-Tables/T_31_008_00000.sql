
create table T_31_008_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(10,2) not null,
  c005 NUMBER(20),
  c006 DATE,
  c007 CHAR(1),
  c008 NUMBER(10,2) not null,
  c009 CHAR(1) not null,
  c010 CHAR(1),
  c011 CHAR(1),
  c012 NUMBER(20) not null,
  c013 NUMBER(20) not null,
  c014 CHAR(1),
  c015 NUMBER(10),
  c016 NUMBER(10),
  constraint PK_T_31_008_0 primary key (C001)
);
comment on table T_31_008_00000
  is '���ֳɼ���';
comment on column T_31_008_00000.c001
  is '��������ID,�ɼ�ID';
comment on column T_31_008_00000.c002
  is '¼��ID,¼����¼�ӿڱ��¼����ˮ��';
comment on column T_31_008_00000.c003
  is '���ֱ�ID,��ӦT_31_001_00000.C001';
comment on column T_31_008_00000.c004
  is '���ֳɼ�';
comment on column T_31_008_00000.c005
  is '�����ID,��ӦT_11_005_00000.C001';
comment on column T_31_008_00000.c006
  is '���ʱ��';
comment on column T_31_008_00000.c007
  is 'YΪ��������,N�ǿؼ���ɼ�';
comment on column T_31_008_00000.c008
  is '���ֱ��и��ӷ�ʱ����ʾ���ӷ��ۼӺ�ɼ�';
comment on column T_31_008_00000.c009
  is 'YΪ���շ֣�N�����շ�';
comment on column T_31_008_00000.c010
  is '1��ѯ���֣�2��ѯ�޸ĵ÷֣�3��������,4�����޸ĵ÷֣�5��������,6�����޸ĵ÷֣�7��������';
comment on column T_31_008_00000.c011
  is 'Y��ʾ�óɼ����쳣������־';
comment on column T_31_008_00000.c012
  is '�÷�������ʱ����ϯ��������T_11_006_00000.C001';
comment on column T_31_008_00000.c013
  is '��¼��������ϯ��ID,��ӦT_11_005_00000.C001';
comment on column T_31_008_00000.c014
  is '�Ƿ����ߣ�Y���ߣ�N�� C��δ�������';
comment on column T_31_008_00000.c015
  is '����������ǲ�';
comment on column T_31_008_00000.c016
  is '�������ֻ���ʱ�䣬��λ��';
