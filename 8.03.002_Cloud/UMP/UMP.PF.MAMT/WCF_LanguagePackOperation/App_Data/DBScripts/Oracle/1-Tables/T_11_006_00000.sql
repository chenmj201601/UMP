
create table T_11_006_00000
(
  C001 NUMBER(20) not null,
  C002 NVARCHAR2(1024) not null,
  C003 NUMBER(5) not null,
  C004 NUMBER(20) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 VARCHAR2(32) not null,
  C008 VARCHAR2(512) not null,
  C009 VARCHAR2(512) not null,
  C010 NUMBER(20) not null,
  C011 DATE not null,
  C012 NVARCHAR2(1024),
  constraint PK_11_006_00000 primary key (C001)
);
comment on table T_11_006_00000
  is '��������';
comment on column T_11_006_00000.C001
  is '������š�С��100Ϊϵͳ��ʼ������';
comment on column T_11_006_00000.C002
  is '�������ơ���ʹ�ü������ݱ���';
comment on column T_11_006_00000.C003
  is '�������͡���ȫ�ֲ��������ú��ѡ��';
comment on column T_11_006_00000.C004
  is '����������ţ���Ӧ����C001';
comment on column T_11_006_00000.C005
  is '�Ƿ��ǻ�ġ�1�����0�����õ�';
comment on column T_11_006_00000.C006
  is '�Ѿ�ɾ����1��ɾ����0��δɾ��';
comment on column T_11_006_00000.C007
  is '����״̬��Ĭ��ֵ32 ��0';
comment on column T_11_006_00000.C008
  is '��Чʱ�俪ʼ��������';
comment on column T_11_006_00000.C009
  is '����ʱ��(��� = UNLIMITED�� ��ʾ������) ��������';
comment on column T_11_006_00000.C010
  is '�����ˡ�';
comment on column T_11_006_00000.C011
  is '����ʱ�䡣UTC';
comment on column T_11_006_00000.C012
  is '��ע������';

