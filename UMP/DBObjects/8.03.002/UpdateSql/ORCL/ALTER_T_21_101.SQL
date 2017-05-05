--T_21_10   2016/11/30   CLOUD	

alter table T_21_101 modify c007 not null;

alter table T_21_101
  drop constraint PK_21_101 cascade;

alter table T_21_101
  add constraint PK_21_101 primary key (C001, C002, C003, C004, C007);