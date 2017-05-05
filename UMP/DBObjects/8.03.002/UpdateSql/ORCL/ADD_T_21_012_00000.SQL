create table T_21_012_00000
(
  c001 NUMBER(20) default 0 not null,
  c002 NUMBER(11) default 0 not null,
  c003 NUMBER(11) default 0,
  c004 NUMBER(11) default 0,
  c005 VARCHAR2(1024),
  c006 VARCHAR2(1024),
  c007 NUMBER(20) default 0,
  c008 NUMBER(20) default 0,
  constraint PK_21_012 primary key (C001, C002)
);
