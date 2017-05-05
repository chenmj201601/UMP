
create table T_31_026_00000
(
  c001 NUMBER(20) not null,
  c002 CHAR(1) not null,
  c003 DATE not null,
  c004 NUMBER(5),
  c005 NUMBER(5),
  c006 CHAR(1),
  c007 NUMBER(5),
  c008 NUMBER(5),
  c009 NUMBER(5),
  c010 NUMBER(5),
  c011 CHAR(1),
  c012 NUMBER(5),
  c013 NUMBER(5),
  c014 NUMBER(5),
  c015 NUMBER(5),
  c016 NUMBER(5),
  c017 NUMBER(5)
);
comment on table T_31_026_00000
  is '���������Լ�����������ʱ�估����';
comment on column T_31_026_00000.c001
  is '������������������ID';
comment on column T_31_026_00000.c002
  is '�������� D:��,W:��,P:Ѯ,M:��,S:�� Y:��  O:ֻ����һ��';
comment on column T_31_026_00000.c003
  is '����ʱ��';
comment on column T_31_026_00000.c004
  is 'ÿ�ܵ����ڼ����� 1~7';
comment on column T_31_026_00000.c005
  is 'ÿ�µĵڼ�������1~31,�Ǹ���û��29��30��31���Ǹ��µ����һ������';
comment on column T_31_026_00000.c006
  is '�Ƿ�ֱ�����Ѯ���� YѮ�ֿ����� NͳһѮ����';
comment on column T_31_026_00000.c007
  is 'ͳһ���õ�ʱ�� 1~11, ����Ǹ�Ѯû��9��10��11��Ѯ�����һ������';
comment on column T_31_026_00000.c008
  is '��Ѯ�ĵڼ�������1~10';
comment on column T_31_026_00000.c009
  is '��Ѯ�ĵڼ�������1~10';
comment on column T_31_026_00000.c010
  is '��Ѯ�ĵڼ�������1~11����Ѯû��9��10��11�ģ������һ������';
comment on column T_31_026_00000.c011
  is '�Ƿ�ֱ�����Ѯ���� Y���ֿ����� Nͳһ������';
comment on column T_31_026_00000.c012
  is 'ͳһ���õ�ʱ�� 0~92, ����Ǹ���û��90��91��92����ȡ�Ǹ��������һ������';
comment on column T_31_026_00000.c013
  is '��һ���ĵڼ���';
comment on column T_31_026_00000.c014
  is '�ڶ���
�ĵڼ���';
comment on column T_31_026_00000.c015
  is '�������ĵڼ���';
comment on column T_31_026_00000.c016
  is '���ļ��ĵڼ���';
comment on column T_31_026_00000.c017
  is '��ĵڼ�������(0~366)';
alter table T_31_026_00000
  add constraint PK_T_31_026_0 primary key (C001)
