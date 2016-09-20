create table T_11_008_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) default 0 not null,
  C003 NVARCHAR2(256) not null,
  C004 NVARCHAR2(1024) not null,
  C005 CHAR(1) not null,
  C006 CHAR(1) not null,
  C007 VARCHAR2(32) not null,
  C008 NUMBER(20) not null,
  C009 NUMBER(20) not null,
  C010 DATE not null,
  C011 NVARCHAR2(1024),
constraint PK_11_008_00000 primary key (C001)
);
comment on table T_11_008_00000
  is '�������б�';
comment on column T_11_008_00000.C001
  is '��������';
comment on column T_11_008_00000.C002
  is '�����������š�Ŀǰ��ʹ�á�Ĭ��ֵ 0';
comment on column T_11_008_00000.C003
  is '��������롣';
comment on column T_11_008_00000.C004
  is '���������ơ���������';
comment on column T_11_008_00000.C005
  is '�Ƿ��ǻ�ġ�1�����0�����õ�';
comment on column T_11_008_00000.C006
  is '�Ѿ�ɾ����1��ɾ����0��δɾ��';
comment on column T_11_008_00000.C007
  is '����״̬����ʼ32 �� 1';
comment on column T_11_008_00000.C008
  is '���������������ȷ������ = 0��һ������²�ʹ��';
comment on column T_11_008_00000.C009
  is '������';
comment on column T_11_008_00000.C010
  is '����ʱ��UTC';
comment on column T_11_008_00000.C011
  is '��ע������';
create unique index IDX_11_008_C003 on T_11_008_00000 (C003);


