
    ALTER table T_11_012_00000 alter  column c002 smallint NOT NULL;

    ALTER table T_11_012_00000
      drop constraint PK_11_102_00000; 
       
    ALTER table T_11_012_00000
      add constraint PK_11_102_00000 primary key (C001, C002, C003, C004);
