
create table T_31_024_00000
(
  c001 NUMBER(20) not null,
  c002 NVARCHAR2(128),
  c003 NVARCHAR2(1024),
  c004 NUMBER(5),
  c005 CHAR(1) default 'Y' not null,
  c006 CHAR(1),
  c007 NUMBER(20),
  c008 NUMBER(10,4),
  c009 VARCHAR2(2000),
  c010 VARCHAR2(2000),
  c011 VARCHAR2(2000),
  c012 VARCHAR2(2000),
  c013 VARCHAR2(2000),
  c014 VARCHAR2(2000),
  c015 VARCHAR2(2000),
  c016 VARCHAR2(2000),
  c017 VARCHAR2(2000),
  c018 VARCHAR2(2000),
  c019 VARCHAR2(2000),
  c020 VARCHAR2(2000),
  c021 CHAR(1),
  c022 CHAR(1),
  c023 NUMBER(38),
  c024 DATE,
  c025 DATE,
  c026 CHAR(1),
  c027 VARCHAR2(1024),
  c028 CHAR(1),
  c029 VARCHAR2(2000),
  c030 CHAR(1),
  c031 VARCHAR2(2000),
  c032 CHAR(1),
  c033 VARCHAR2(2000),
  c034 CHAR(1),
  c035 CHAR(1),
  c036 CHAR(1),
  c037 CHAR(1),
  c038 CHAR(1),
  c039 NVARCHAR2(1),
  c040 NVARCHAR2(16),
  c041 NVARCHAR2(32),
  c042 NVARCHAR2(64),
  c043 VARCHAR2(2000),
  c044 CHAR(1),
  c045 VARCHAR2(2000),
  c046 CHAR(1),
  c047 CHAR(1),
  c048 CHAR(1),
  c049 CHAR(1),
  c050 CHAR(1),
  c051 CHAR(1),
  c052 CHAR(1),
  c053 CHAR(1),
  c054 CHAR(1),
  c055 CHAR(1),
  c056 CHAR(1),
  c057 CHAR(1),
  c058 CHAR(1),
  c059 CHAR(1),
  c060 CHAR(1),
  c061 VARCHAR2(2000),
  c062 CHAR(1),
  c063 CHAR(1),
  c064 CHAR(1)
);
comment on table T_31_024_00000
  is '��ѯ����';
comment on column T_31_024_00000.c001
  is '��������';
comment on column T_31_024_00000.c002
  is '�����ѯ��������';
comment on column T_31_024_00000.c003
  is '����';
comment on column T_31_024_00000.c004
  is '1��ѯ�������� 2�����Զ������������ 3�����Զ������������ 4���������������';
comment on column T_31_024_00000.c005
  is '����/����  Y/N';
comment on column T_31_024_00000.c006
  is 'A��ÿ����ϯÿ�켸�� B��ÿ����ϯÿ��ٷֱ�  C:ÿ����ϯ���ʱ��ȡ������ D:ÿ����ϯ���ʱ��ٷֱ�  E:���ʱ���ܵĶ����� F:���ʱ��¼���ܵİٷֱ�';
comment on column T_31_024_00000.c007
  is '¼��������';
comment on column T_31_024_00000.c008
  is 'ȡ¼���İٷֱ�';
comment on column T_31_024_00000.c009
  is 'Ҫ��ѯ���û�ID���Զ��Ÿ���';
comment on column T_31_024_00000.c010
  is 'Ҫ��ѯ���û�ID���Զ��Ÿ���';
comment on column T_31_024_00000.c011
  is 'Ҫ��ѯ���û�ID���Զ��Ÿ���';
comment on column T_31_024_00000.c012
  is 'Ҫ��ѯ���û�ID���Զ��Ÿ���';
comment on column T_31_024_00000.c013
  is 'Ҫ��ѯ���û�ID���Զ��Ÿ���';
comment on column T_31_024_00000.c014
  is 'Ҫ��ѯ�Ļ���ID���Զ��Ÿ���';
comment on column T_31_024_00000.c015
  is 'Ҫ��ѯ�Ļ���ID���Զ��Ÿ���';
comment on column T_31_024_00000.c016
  is 'Ҫ��ѯ�Ļ���ID���Զ��Ÿ���';
comment on column T_31_024_00000.c017
  is 'Ҫ��ѯ�Ļ���ID���Զ��Ÿ���';
comment on column T_31_024_00000.c018
  is 'Ҫ��ѯ�Ļ���ID���Զ��Ÿ���';
comment on column T_31_024_00000.c019
  is 'Ҫ��ѯ�ļ�����ID���Զ��Ÿ���';
comment on column T_31_024_00000.c020
  is 'Ҫ��ѯ�ļ�����ID���Զ��Ÿ���';
comment on column T_31_024_00000.c021
  is '�Ƿ��������ʱ�� Y������ N�����ÿ�ʼʱ��~����ʱ��';
comment on column T_31_024_00000.c022
  is 'Y:�� M:��  D:��  H:Сʱ';
comment on column T_31_024_00000.c023
  is '���꣬���£����죬��Сʱ';
comment on column T_31_024_00000.c024
  is '¼����ʼʱ��';
comment on column T_31_024_00000.c025
  is '¼������ʱ��';
comment on column T_31_024_00000.c026
  is '�Ƿ�ֻ���ģ����ѯ';
comment on column T_31_024_00000.c027
  is '�ֻ���ƴ�����Զ��Ÿ���';
comment on column T_31_024_00000.c028
  is '�Ƿ�¼����ˮ��ģ����ѯ';
comment on column T_31_024_00000.c029
  is '¼����ˮ���Զ��Ÿ���';
comment on column T_31_024_00000.c030
  is '�Ƿ����к�ģ����ѯ';
comment on column T_31_024_00000.c031
  is '���к��Զ��Ÿ���';
comment on column T_31_024_00000.c032
  is '�Ƿ񱻽к�ģ����ѯ';
comment on column T_31_024_00000.c033
  is '���к��Զ��Ÿ���';
comment on column T_31_024_00000.c034
  is '���з���  A ����ͺ���  O���� I����';
comment on column T_31_024_00000.c035
  is '�Ƿ���¼��  A ȫ���� Y��¼�� N��¼��';
comment on column T_31_024_00000.c036
  is '�Ƿ�������������־';
comment on column T_31_024_00000.c037
  is '�Ƿ���ؼ��ֱ�־';
comment on column T_31_024_00000.c038
  is '�Ƿ��¼����ǩ';
comment on column T_31_024_00000.c039
  is '¼�������ֶ�1';
comment on column T_31_024_00000.c040
  is '¼�������ֶ�2';
comment on column T_31_024_00000.c041
  is '¼�������ֶ�3';
comment on column T_31_024_00000.c042
  is '¼�������ֶ�4';
comment on column T_31_024_00000.c043
  is '���ֱ�Id���Զ��ŷָ�';
comment on column T_31_024_00000.c044
  is 'A ȫ���� Y������� Nδ��������';
comment on column T_31_024_00000.c045
  is '������ID';
comment on column T_31_024_00000.c046
  is '����ˮƽ  Y���ã�N����';
comment on column T_31_024_00000.c047
  is 'Aȫ�� Y����̬�Ⱥ�  N����̬�Ȳ�';
comment on column T_31_024_00000.c048
  is 'רҵˮƽ  Y���ã�N����';
comment on column T_31_024_00000.c049
  is 'Aȫ�� Yרҵˮƽ��  Nרҵˮƽ ��';
comment on column T_31_024_00000.c050
  is '���������� Y���ã�N����';
comment on column T_31_024_00000.c051
  is 'Aȫ�� Y���������Ⱥ�  N���������Ȳ�';
comment on column T_31_024_00000.c052
  is '�º���Ч��Y���ã�N����';
comment on column T_31_024_00000.c053
  is 'Aȫ�� Y �º���Ч�ʸ�  N �º���Ч�ʵ�';
comment on column T_31_024_00000.c054
  is '¼���߷��� Y���ã�N����';
comment on column T_31_024_00000.c055
  is 'Aȫ�� H:¼���߷��� C:¼��ƽ�� L:¼���͹�';
comment on column T_31_024_00000.c056
  is '�ظ����� Y���ã�N����';
comment on column T_31_024_00000.c057
  is 'Aȫ�� Y���ظ�����  N�����ظ�����';
comment on column T_31_024_00000.c058
  is 'ʱ������  Y���ã�N����';
comment on column T_31_024_00000.c059
  is 'Aȫ��  Yʱ�������ʺ�,Nʱ�����Ʋ��ʺ�';
comment on column T_31_024_00000.c060
  is '��ϯ�ȼ� Y���ã�N����';
comment on column T_31_024_00000.c061
  is '��ϯ�ȼ�ƴ�����Զ��Ÿ���';
comment on column T_31_024_00000.c062
  is '�쳣ʱ�� Y���ã�N����';
comment on column T_31_024_00000.c063
  is 'Aȫ��  Y���쳣ʱ����,N���쳣ʱ����';
comment on column T_31_024_00000.c064
  is 'Aȫ�� Y����ϯ N������ϯ';
alter table T_31_024_00000
  add constraint PK_T_31_024_0 primary key (C001);
