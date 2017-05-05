
create table T_11_201_00000
(
  C001 NUMBER(20) not null,
  C002 NUMBER(20) not null,
  C003 NUMBER(20) not null,
  C004 NUMBER(20) not null,
  C005 DATE not null,
  C006 DATE not null,
  constraint PK_11_201_000000 primary key (C001, C003, C004)
);
comment on table T_11_201_00000
  is '用户、角色管理或控制的资源列表';
comment on column T_11_201_00000.C001
  is '编号';
comment on column T_11_201_00000.C002
  is '父级编号，对应本表 C001，默认值 0';
comment on column T_11_201_00000.C003
  is '用户、角色、技能组编号';
comment on column T_11_201_00000.C004
  is '被控制、管理对象的编号';
comment on column T_11_201_00000.C005
  is '开始时间';
comment on column T_11_201_00000.C006
  is '结束时间';
create index IDX_11_201_C003_000000 on T_11_201_00000 (C003);

