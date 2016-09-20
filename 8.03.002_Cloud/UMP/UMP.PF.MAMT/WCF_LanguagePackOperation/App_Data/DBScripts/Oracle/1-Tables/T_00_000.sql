-- Create table
create table T_00_000
(
  C000 VARCHAR2(5) not null,
  C001 VARCHAR2(32) not null,
  C002 VARCHAR2(16) not null,
  C003 VARCHAR2(8) not null,
  C004 CHAR(1) not null,
  C005 DATE not null,
  C006 DATE,
  C011 VARCHAR2(16),
  C012 VARCHAR2(512),
  C013 VARCHAR2(16),
  C014 VARCHAR2(512),
  C015 VARCHAR2(16),
  C016 VARCHAR2(512),
  C017 VARCHAR2(16),
  C018 VARCHAR2(512),
  C019 VARCHAR2(16),
  C020 VARCHAR2(512),
  C101 VARCHAR2(8),
  C102 VARCHAR2(8),
  C103 VARCHAR2(16),
  C104 VARCHAR2(16),
  C105 VARCHAR2(16),
  C106 VARCHAR2(32),
  C107 VARCHAR2(32),
  C108 VARCHAR2(32),
  C109 VARCHAR2(1024),
  C110 VARCHAR2(1024),
  Constraint PK_00_000 primary key (C000, C001)
);
-- Add Comments to the table 
Comment on table T_00_000
  is '���ݿ���������Ϣ��';
-- Add Comments to the Columns 
Comment on Column T_00_000.C000
  is '�⻧���. Ĭ��ֵ 00000';
Comment on Column T_00_000.C001
  is '��������';
Comment on Column T_00_000.C002
  is '��ǰ�汾';
Comment on Column T_00_000.C003
  is '�������͡��硣FN��TB��';
Comment on Column T_00_000.C004
  is '״̬';
Comment on Column T_00_000.C005
  is '����ʱ��UTC';
Comment on Column T_00_000.C006
  is '����޸�ʱ��UTC';
Comment on Column T_00_000.C011
  is '����1';
Comment on Column T_00_000.C012
  is '����1���';
Comment on Column T_00_000.C013
  is '����2';
Comment on Column T_00_000.C014
  is '����2���';
Comment on Column T_00_000.C015
  is '����3';
Comment on Column T_00_000.C016
  is '����3���';
Comment on Column T_00_000.C017
  is '����4';
Comment on Column T_00_000.C018
  is '����4���';
Comment on Column T_00_000.C019
  is '����5';
Comment on Column T_00_000.C020
  is '����5���';
Comment on Column T_00_000.C101
  is '�����ֶ�1';
Comment on Column T_00_000.C102
  is '�����ֶ�2';
Comment on Column T_00_000.C103
  is '�����ֶ�3';
Comment on Column T_00_000.C104
  is '�����ֶ�4';
Comment on Column T_00_000.C105
  is '�����ֶ�5';
Comment on Column T_00_000.C106
  is '�����ֶ�6';
Comment on Column T_00_000.C107
  is '�����ֶ�7';
Comment on Column T_00_000.C108
  is '�����ֶ�8';
Comment on Column T_00_000.C109
  is '�����ֶ�9';
Comment on Column T_00_000.C110
  is '�����ֶ�10';
