
create table T_11_004_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(5) not null,
  C004 NVARCHAR2(1024) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 VARCHAR2(32) not null,
  C008 VARCHAR2(512) not null,
  C009 VARCHAR2(512) not null,
  C010 NUMBER(20) not null,
  C011 DATE not null,
  constraint PK_T_11_004_00000 primary key (C001)
);
comment on table T_11_004_00000
  is 'ϵͳ��ɫ�б���Ĭ�Ͻ�ɫ���û��Զ����ɫ';
comment on column T_11_004_00000.C001
  is '��ɫ��š�С�� 100 Ϊϵͳ��ʼ����ɫ�����ɽ����κα༭';
comment on column T_11_004_00000.C002
  is '������ɫ��š���Ӧ����C001�������ֵΪ 0 �����ʾû�и�����ɫ��';
comment on column T_11_004_00000.C003
  is 'Ӧ�����ĸ�ģ�顣�����ֵΪ0����ʾ��ģ��ʹ��';
comment on column T_11_004_00000.C004
  is '��ɫ���ƣ����ܱ��棩';
comment on column T_11_004_00000.C005
  is '�Ƿ��ǻ�ġ�1�����0�����õ�';
comment on column T_11_004_00000.C006
  is '�Ѿ�ɾ����1��ɾ����0��δɾ��';
comment on column T_11_004_00000.C007
  is '��ɫ����״̬��Ĭ�� 32 ��0';
comment on column T_11_004_00000.C008
  is '����ʱ��,�������ݱ��档YYYY-MM-DD HH24:MI:SS';
comment on column T_11_004_00000.C009
  is '����ʱ�䣬�������ݱ��档YYYY-MM-DD HH24:MI:SS��2199-12-31 23��59��59��ʾ��������';
comment on column T_11_004_00000.C010
  is '�����ˡ�0��ʾ��ʼ������';
comment on column T_11_004_00000.C011
  is '����ʱ�䣨UTC��';
