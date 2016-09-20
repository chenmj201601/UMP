create table T_44_101_00000
(
  c001 NUMBER(20) default 0 not null,
  c002 VARCHAR2(256) not null,
  c003 NVARCHAR2(1024),
  c004 NUMBER(20) default 0 not null,
  c005 CHAR(1) default '0' not null,
  c006 CHAR(1) default '1' not null,
  c007 NUMBER(5) default 0,
  c008 NUMBER(5) default 0,
  c009 VARCHAR2(32),
  c010 VARCHAR2(256),
  c011 NUMBER(20),
  c012 NUMBER(20),
  c013 NUMBER(20),
  c014 NUMBER(20),
  c015 CHAR(1),
  constraint PK_44_101_00000 primary key (C001)
);