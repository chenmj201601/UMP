
create table T_31_029_00000
(
  c001 NUMBER(5) not null,
  c002 CHAR(1) not null,
  c003 NUMBER(20) default -1 not null,
  c004 NUMBER(20) default -1 not null,
  c005 NVARCHAR2(50) not null,
  c006 NUMBER(5) not null,
  c007 VARCHAR2(200) not null,
  c008 NUMBER(20) not null,
  c009 CHAR(1) not null
);
comment on table T_31_029_00000
  is '��ϯ�ȼ������ã����ڷ���ͳ��';
comment on column T_31_029_00000.c001
  is '��ϯ�ȼ�ID';
comment on column T_31_029_00000.c002
  is 'T:������֯���� S������';
comment on column T_31_029_00000.c003
  is '�÷�������ʱ����ϯ��������T_11_006_0000.C001';
comment on column T_31_029_00000.c004
  is '�������š�T_11_008_0000.C001';
comment on column T_31_029_00000.c005
  is '�ȼ�����';
comment on column T_31_029_00000.c006
  is '1����=,2����>=,3����>,4����<=,5����< 6 between';
comment on column T_31_029_00000.c007
  is 'ֵ';
comment on column T_31_029_00000.c008
  is '��������ID';
comment on column T_31_029_00000.c009
  is 'Y���á�N����';
  
alter table T_31_029_00000
  add constraint PK_31_029_0 primary key (C001)

