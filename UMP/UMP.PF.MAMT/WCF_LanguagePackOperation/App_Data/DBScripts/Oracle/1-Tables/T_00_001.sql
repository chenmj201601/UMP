create table T_00_001
(
  C000 VARCHAR2(5) not null,
  C001 NUMBER(5) not null,
  C002 NUMBER(5) not null,
  C003 NUMBER(11) not null,
  C004 NUMBER(11) not null,
  constraint PK_00_001 primary key (C000, C001, C002, C003)
);
comment on table T_00_001
  is 'ϵͳ�Զ���ˮ�����ɱ�';
comment on column T_00_001.C000
  is '�⻧���. Ĭ��ֵ 00000';
comment on column T_00_001.C001
  is 'ģ���ţ�11-91��';
comment on column T_00_001.C002
  is 'ģ���ڲ���ţ���ΧΪ 100 - 999';
comment on column T_00_001.C003
  is 'ʹ�õ���ʱ�䣬��ȷ��Сʱ��ֵ�� yyyymmddhh�������ֵ����ΪNoTime������ʱ���޹أ�ֱ�ӷ��� C004 + 1';
comment on column T_00_001.C004
  is '��ǰ�Ѿ�ʹ�õ�ֵ 1 - 9,999,999';
