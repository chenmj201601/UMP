create table T_11_204_00000
(
  C001 NUMBER(20) not null,
  C002 VARCHAR2(32) not null,
  C003 VARCHAR2(1024) not null,
  C004 NUMBER(5) not null,
  C005 NVARCHAR2(64) not null,
  C006 CHAR(1) not null,
  constraint PK_11_204_00000 primary key (C001, C002)
);
comment on table T_11_204_00000
  is '�û�����ɫ���������Ӧ�ķ��顢�����б�';
comment on column T_11_204_00000.C001
  is '�û�����ɫ����������';
comment on column T_11_204_00000.C002
  is 'GroupID����¼��������ķ�����';
comment on column T_11_204_00000.C003
  is '�����Ĺ��ܱ��,���48�����ܱ�ţ��м���char(27)�ֿ�';
comment on column T_11_204_00000.C004
  is '��ʾ˳��';
comment on column T_11_204_00000.C005
  is '��������.����������� = GroupID��������԰��ж�ȥ��Ӧ������';
comment on column T_11_204_00000.C006
  is '�Ƿ���ʾ�������ƣ�1:��ʾ��0:����ʾ';
