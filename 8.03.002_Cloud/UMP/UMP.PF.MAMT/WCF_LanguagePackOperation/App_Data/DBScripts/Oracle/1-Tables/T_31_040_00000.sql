
create table T_31_040_00000
(
  c001 VARCHAR2(128) not null,
  c002 NUMBER(5) default 0,
  c003 NUMBER(5) not null,
  c004 NVARCHAR2(1000) default -1,
  c005 NUMBER(5) default -1 not null,
  c006 CHAR(1) not null,
  c007 NUMBER(20) default -1 not null,
  c008 NUMBER(5) default -1 not null,
  constraint PK_31_040_0 primary key (C001, C007)
);
comment on table T_31_040_00000
  is '�Զ����ѯ����';
comment on column T_31_040_00000.c001
  is '��ѯ�ؼ���';
comment on column T_31_040_00000.c002
  is ' 1Ϊtextbox 2 combox 3Ϊgrid';
comment on column T_31_040_00000.c003
  is '����tabitem������ֵ,��0��ʼ';
comment on column T_31_040_00000.c004
  is '�����TabItem������';
comment on column T_31_040_00000.c005
  is '��һ��tabitem���˳��';
comment on column T_31_040_00000.c006
  is 'YΪϵͳ����Ĭ�ϵ� NΪ�û����˶����';
comment on column T_31_040_00000.c007
  is '�û�ID ,��Ӧ T_11_005_00000.C001';
comment on column T_31_040_00000.c008
  is '�ؼ��߶�';

