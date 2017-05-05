-- Create table
create table T_44_103_00000
(
  c001 NUMBER(20) default 0 not null,
  c002 NUMBER(5) default 0 not null,
  c003 VARCHAR2(1024) not null,
  c004 NVARCHAR2(1024),
  c005 CHAR(1) default '1' not null,
  c006 VARCHAR2(1024) not null,
  c007 VARCHAR2(1024),
  c008 CHAR(1) default '1',
  c009 NUMBER(5) default '0' not null,
  c010 NUMBER(20) default 0 not null,
  c011 NUMBER(20),
  c012 NUMBER(20),
  c013 NUMBER(20),
  c014 NUMBER(20),
  constraint PK_44_103_00000 primary key (C001)
);