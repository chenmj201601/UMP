
create table T_31_001_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(1024) not null,
  c003 CHAR(1) default 'T' not null,
  c004 NUMBER(10,4),
  c005 NUMBER(20),
  c006 DATE,
  c007 DATE,
  c008 NUMBER(20),
  c009 DATE,
  c010 DATE,
  c011 NUMBER(10,4) not null,
  c012 NUMBER(5) not null,
  c013 CHAR(1) default 'Y' not null,
  c014 CHAR(1) default 'S' not null,
  c015 NUMBER(20) not null,
  c016 DATE,
  c017 NUMBER(20) not null,
  c018 CHAR(1) default 'N',
  constraint PK_31_001_0 primary key (C001)
);
comment on table T_31_001_00000
  is '���ֱ�';
comment on column T_31_001_00000.c001
  is '��������ID,���ֱ�ID';
comment on column T_31_001_00000.c002
  is '���ֱ�����';
comment on column T_31_001_00000.c003
  is '���ֱ���ʽT����״���ֱ�C������� ';
comment on column T_31_001_00000.c004
  is '���ֱ��ܷ֣���Ϊ���Ƿ���ʱ����Ϊ0';
comment on column T_31_001_00000.c005
  is '���ֱ�����ID,��ӦT_11_005_00000.c001';
comment on column T_31_001_00000.c006
  is '���ֱ���ʱ��';
comment on column T_31_001_00000.c007
  is '����޸�ʱ��';
comment on column T_31_001_00000.c008
  is '����޸��˵�ID,��ӦT_11_005_00000.c001';
comment on column T_31_001_00000.c009
  is '���ֱ����ÿ�ʼʱ��';
comment on column T_31_001_00000.c010
  is '���ֱ�Ĺ���ʱ��';
comment on column T_31_001_00000.c011
  is '�ϸ��ߣ�Ϊ���Ƿ���ʱ��Ϊ���ٸ������Ǻϸ������';
comment on column T_31_001_00000.c012
  is '���ֱ���������';
comment on column T_31_001_00000.c013
  is '���ֱ�����/����  Y/N';
comment on column T_31_001_00000.c014
  is 'P���ٷֱ� F:���Ƿ� S:��ֵ';
comment on column T_31_001_00000.c015
  is '��������ID,��ӦT_11_006_00000.c001';
comment on column T_31_001_00000.c016
  is '���һ��ʹ��ʱ��';
comment on column T_31_001_00000.c017
  is '0��ʾδʹ�õģ���һ��+1';
comment on column T_31_001_00000.c018
  is '���ֱ��Ƿ����� Y���� N������';
