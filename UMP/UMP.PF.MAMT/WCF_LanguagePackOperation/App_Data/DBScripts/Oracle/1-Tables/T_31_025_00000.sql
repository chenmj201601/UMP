
create table T_31_025_00000
(
  c001 NUMBER(20) not null,
  c002 NUMBER(20) not null,
  c003 NVARCHAR2(1024),
  c004 NUMBER(20) not null,
  c005 DATE not null,
  c006 DATE,
  c007 NUMBER(20),
  c008 NUMBER(5),
  c009 NVARCHAR2(1024),
  c010 CHAR(1)
);
comment on table T_31_025_00000
  is '�Ƽ�¼���б�';
comment on column T_31_025_00000.c001
  is '��������,�Ƽ���¼ID';
comment on column T_31_025_00000.c002
  is '¼����ˮ��,��Ӧ ��ӦT_21_000.RecoredReference';
comment on column T_31_025_00000.c003
  is '�Ƽ�����';
comment on column T_31_025_00000.c004
  is '�Ƽ���ID,��Ӧ T_11_034_BU.UserID';
comment on column T_31_025_00000.c005
  is '�Ƽ�ʱ��';
comment on column T_31_025_00000.c006
  is '����ʱ��';
comment on column T_31_025_00000.c007
  is '������ID,��Ӧ T_11_034_BU.UserID';
comment on column T_31_025_00000.c008
  is '������ID';
comment on column T_31_025_00000.c009
  is '����ע';
comment on column T_31_025_00000.c010
  is 'Y��ʾ�Ѿ����� N��ʾδ����';
alter table T_31_025_00000
  add constraint PK_31_025_0 primary key (C001);
