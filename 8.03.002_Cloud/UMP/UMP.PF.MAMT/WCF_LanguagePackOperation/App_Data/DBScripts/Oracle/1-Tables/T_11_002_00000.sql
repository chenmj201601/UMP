create table T_11_002_00000
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(512) not null,
  C003 VARCHAR2(32) not null,
  C004 VARCHAR2(512) not null,
  C005 VARCHAR2(512) not null,
  C006 NUMBER(20) not null,
  C008 CHAR(1)  default '0' not null,
  C009 VARCHAR2(512),
  C010 CHAR(1),
  C011 VARCHAR2(512),
  C012 NUMBER(5),
  constraint PK_11_002_00000 primary key (C001, C002)
);
comment on table T_11_002_00000
  is '�û���¼ϵͳ��ˮ��';
-- Add comments to the columns 
comment on column T_11_002_00000.C001
  is '�û����루11_005.C001��';
comment on column T_11_002_00000.C002
  is '��¼ʱ��';
comment on column T_11_002_00000.C003
  is '��¼����״̬��Ĭ��32��''0''';
comment on column T_11_002_00000.C004
  is '��¼������';
comment on column T_11_002_00000.C005
  is '��¼����IP';
comment on column T_11_002_00000.C006
  is '��¼������SessionID';
comment on column T_11_002_00000.C008
  is '���˳�ϵͳ��Ĭ��''0''';
comment on column T_11_002_00000.C009
  is '�˳�ϵͳʱ�䡣';
comment on column T_11_002_00000.C010
  is '�˳�ϵͳ�ķ�ʽ��';
comment on column T_11_002_00000.C011
  is '�����ϵʱ��';
comment on column T_11_002_00000.C012
  is '��¼ģ��';
create unique index IDX_11_002_001006 on T_11_002_00000 (C001, C006);