
create table T_31_002_00000
(
  c001 NUMBER(5) not null,
  c002 NUMBER(20) not null,
  c003 NUMBER(20) not null,
  c004 NUMBER(20),
  c005 NUMBER(5) not null,
  c006 NVARCHAR2(1024),
  c007 NVARCHAR2(1024),
  c008 NUMBER(10,4),
  c009 CHAR(1) default 'N' not null,
  c010 CHAR(1) default 'N' not null,
  c011 CHAR(1) default 'N' not null,
  c012 CHAR(1) default 'N' not null,
  c013 CHAR(1) default 'N' not null,
  c014 CHAR(1) default 'N',
  c015 NUMBER(5) default 1 not null,
  c016 CHAR(1) default 'N' not null,
  c017 CHAR(1) default 'N' not null,
  c018 NUMBER(10,4),
  c019 CHAR(1) default 'N' not null,
  c020 CHAR(1) default 'S' not null,
  c021 CHAR(1),
  c022 NVARCHAR2(1024),
  c023 NUMBER(10,4),
  c024 NUMBER(10,4),
  c025 CHAR(100),
  c026 NUMBER(5),
  c027 NUMBER(10,4),
  c028 NUMBER(10,4),
  c029 NUMBER(10,4),
  c030 NVARCHAR2(1024),
  constraint PK_31_002_0 primary key (C001, C003)
);
comment on table T_31_002_00000
  is '���ֱ������';
comment on column T_31_002_00000.c001
  is '���ֱ�����ID��1~200�����֣���T_31_001_0.C001һ�������������';
comment on column T_31_002_00000.c002
  is '���ֱ�����ID,ͨ�����ô洢��������';
comment on column T_31_002_00000.c003
  is '���ֱ�ID����ӦT_31_001_0.C001';
comment on column T_31_002_00000.c004
  is '�����ID,��Ӧ�����C001';
comment on column T_31_002_00000.c005
  is '����ͬһ���ʱ������˳��';
comment on column T_31_002_00000.c006
  is '��������';
comment on column T_31_002_00000.c007
  is '��������';
comment on column T_31_002_00000.c008
  is '�����ܷ�';
comment on column T_31_002_00000.c009
  is 'Y��ʾ�����ֱ�׼ N��ʾ�������ֱ�׼';
comment on column T_31_002_00000.c010
  is 'Y��ʾ�������ܷ֣�N��ʾ�����ܷ�';
comment on column T_31_002_00000.c011
  is 'Y��ʾ�ǿ����� N��ʾ����';
comment on column T_31_002_00000.c012
  is 'Y��ʾ�ǹؼ��� N��ʾ�ǹؼ��O��һ����';
comment on column T_31_002_00000.c013
  is 'Y��ʾ������N/A�� N��ʾ����';
comment on column T_31_002_00000.c014
  is 'Y��ʾ����ת��,N������ת��';
comment on column T_31_002_00000.c015
  is '1��ʾ�ı����,2��ʾ�������3��ʾ��ѡ���,4��ʾ��ѡ���5��ʾ���������� 6����ģʽ';
comment on column T_31_002_00000.c016
  is '�Ƿ�����ķ���ƽ������������ֱ�׼�����������ȫΪ���ֱ�׼��Y��ʾƽ�����䣬N��ʾ����';
comment on column T_31_002_00000.c017
  is 'Y��ʾ�Ǹ��ӷ� N��ʾ����';
comment on column T_31_002_00000.c018
  is '����';
comment on column T_31_002_00000.c019
  is 'Y�����÷��� ,N�����÷���';
comment on column T_31_002_00000.c020
  is 'P���ٷֱ� F:���Ƿ� S:��ֵ';
comment on column T_31_002_00000.c021
  is 'S�Ǽ򵥹�ʽ A�߼��湫ʽ';
comment on column T_31_002_00000.c022
  is '����';
comment on column T_31_002_00000.c023
  is '����������ʱ��ʼֵ';
comment on column T_31_002_00000.c024
  is '����������ʱ����ֵ';
comment on column T_31_002_00000.c025
  is '��߿̶ȣ����ֵ�Զ��ŷָ�';
comment on column T_31_002_00000.c026
  is '���ƽ���ֳɼ��ȷ�';
comment on column T_31_002_00000.c027
  is 'Ϊ�ı�����ʱ��������ֵ';
comment on column T_31_002_00000.c028
  is 'Ϊ�ı�����ʱ�������Сֵ';
comment on column T_31_002_00000.c029
  is 'Ϊ�ı�����ʱĬ������ֵ';
comment on column T_31_002_00000.c030
  is '������ʽ��֤�ı��������Ƿ���Ϲ���';

