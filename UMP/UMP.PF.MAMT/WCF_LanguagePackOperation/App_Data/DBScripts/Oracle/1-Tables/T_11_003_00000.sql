
create table T_11_003_00000
(
  C001 NUMBER(5) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(20) not null,
  C004 NUMBER(5) not null,
  C005 VARCHAR2(32) not null,
  C006 VARCHAR2(1024),
  C007 VARCHAR2(1024),
  C008 VARCHAR2(1024),
  C009 NVARCHAR2(1024) not null,
  C010 NVARCHAR2(1024),
  C011 CHAR(1) not null,
  C012 CHAR(1),
  C013 NVARCHAR2(1024),
  C014 CHAR(4),
  C015 VARCHAR2(1024),
  constraint PK_T_11_003_0 primary key (C001, C002)
);
comment on table T_11_003_00000
  is '系统功能和操作列表';
comment on column T_11_003_00000.C001
  is '模块编号。对应值 T_11_002.C001';
comment on column T_11_003_00000.C002
  is '功能编号或操作编号。在该字段上建立唯一索引';
comment on column T_11_003_00000.C003
  is '父级编号';
comment on column T_11_003_00000.C004
  is '本级排序序号';
comment on column T_11_003_00000.C005
  is '默认GroupID，登录后主界面的分组';
comment on column T_11_003_00000.C006
  is '许可信息';
comment on column T_11_003_00000.C007
  is '许可信息';
comment on column T_11_003_00000.C008
  is '许可信息';
comment on column T_11_003_00000.C009
  is '打开对象(Url)';
comment on column T_11_003_00000.C010
  is '打开对象需要带的参数';
comment on column T_11_003_00000.C011
  is '打开方式。N：正常打开; 或 操作的类型，M：Menu，B：Button';
comment on column T_11_003_00000.C012
  is 'B:在前面显示分割条；A：在后面显示分割条；N：无';
comment on column T_11_003_00000.C013
  is '显示图片的路径';
comment on column T_11_003_00000.C014
  is '存在小、中、宽、大图标，分别用 0、1表示是否存在';
comment on column T_11_003_00000.C015
  is '显示依赖对象标识';
alter table T_11_003_00000
  add constraint CH_T_11_003_0 unique (C002)
;
