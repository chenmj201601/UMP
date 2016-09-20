CREATE PROCEDURE P_00_001(AInParam01      VARCHAR2,
                                     AInParam02      VARCHAR2,
                                     AInParam03      VARCHAR2:='00000',
                                     AInParam04      VARCHAR2:='20140101000000',
                                     AOutParam01     OUT VARCHAR2,
                                     AOutErrorNumber OUT NUMBER,
                                     AOutErrorString OUT NVARCHAR2)

  IS


-- type:           系统流水号生成过程，记录目标表为T_00_001
-- create date:    <2014-08-6>
-- description:    <获取系统自动编码>
-- =========================================================================================
-- parameters description:
-- @ainparam01:   模块编码
-- @ainparam02:   模块内编码 100 ～ 920
-- @ainparam03:   租户编号,默认为00000
-- @ainparam04:   时间变量 固定时间或开始录音时间
-- =========================================================================================
-- return description:
-- @AOutParam01:成功后返回的系统自动编号bigint型
--          -10：模块编码错误，
--          -11：模块不存在；
--          -20：内部编码错误；
--          -21：内部编码范围错误
--          100yymmddhh00000000:成功后返回的系统自动编号varchar型 计 3+8+8 =19位
--操作码记录表
--SELECT TOP 1000 [CType]
--      ,[CValue]
--      ,[CDescriber]
--      ,[CTypeName]
--  FROM [UMPDevInfo].[dbo].[T_Type_List]
-- ==========================================================================================
    v_ainparam03     VARCHAR2(5);
    v_ainparam04     VARCHAR2(14);
    v_err_num        NUMBER;
    v_err_msg        NVARCHAR2(200);
    vcurrenthour     VARCHAR2(8);
    intserialnumber  NUMBER(11);
    intdatacount     NUMBER(11);
    intcurrentnumber NUMBER(20);
    intmoduleid      NUMBER(6);
    intserialtype    NUMBER(6);
    v_sql            VARCHAR2(500);

BEGIN
   IF ainparam03 IS NULL THEN
     v_ainparam03:='00000';
   ELSE
     v_ainparam03:=ainparam03;
   END IF;

   IF Ainparam04 IS NULl THEN
     v_Ainparam04:='20140101000000';
   ELSE
     v_Ainparam04:=Ainparam04;
   END IF;

   IF F_00_003(ainparam01) = 0 THEN
        AOutErrorNumber := -10;
        AOutErrorString:='failed';
        RETURN;
   END IF;

   IF F_00_003(ainparam02) = 0 THEN
        AOutErrorNumber := -20;
        AOutErrorString:='failed';        
        RETURN;
   END IF;

    intmoduleid   := TO_NUMBER(ainparam01);
    intserialtype := TO_NUMBER(ainparam02);

    IF (intmoduleid < 11 OR intmoduleid > 91) THEN
        AOutErrorNumber := -10;
        AOutErrorString:='failed';        
        RETURN;
    END IF;

    IF (intserialtype < 100 OR intserialtype > 999) THEN
        AOutErrorNumber := -21;
        AOutErrorString:='failed';              
        RETURN;
    END IF;

    v_sql:='SELECT COUNT(1) FROM T_00_008 WHERE C001 = '||intmoduleid;

    EXECUTE IMMEDIATE v_sql INTO intdatacount;

    IF intdatacount = 0 THEN
        AOutErrorNumber := -11; 
        AOutErrorString:='failed';             
        RETURN;
    END IF;   

    vcurrenthour := SUBSTR(v_Ainparam04,3,8);

    intserialnumber := TO_NUMBER(vcurrenthour);

    SELECT COUNT(1)
      INTO intdatacount
      FROM T_00_001
     WHERE c000 =v_Ainparam03
       AND c001 = intmoduleid
       AND c002 = intserialtype
       AND c003 = intserialnumber;

    IF intdatacount = 0 THEN
        INSERT INTO T_00_001
        VALUES(v_ainparam03,intmoduleid,intserialtype, intserialnumber,0);
    END IF;

    SELECT nvl(c004,0) + 1
      INTO intcurrentnumber
      FROM T_00_001
     WHERE c000 =v_Ainparam03
       AND c001 = intmoduleid
       AND c002 = intserialtype
       AND c003 = intserialnumber;

    UPDATE T_00_001
       SET c004 = intcurrentnumber
     WHERE c000 =v_Ainparam03
       AND c001 = intmoduleid
       AND c002 = intserialtype
       AND c003 = intserialnumber;

    COMMIT;  
      
    IF  ainparam01='21' AND ainparam02='201' AND ainparam04='20140101000000'  THEN
      AOutParam01 := intcurrentnumber;--录音记录流水号 c001  1
      
    ELSIF ainparam01='21' AND ainparam02='201' and ainparam04<>'20140101000000'  THEN
      AOutParam01 := to_char(intserialnumber*100000000000+intcurrentnumber);--录音记录流水号 c002 8+11 19 yymmddhh00000000001
        
    ELSE
    --格式为  100:操作编号唯一 yymmddhh:小时时间 00000000：8位流水号   3+8+8 19位
      AOutParam01 := ainparam02||TO_CHAR(intserialnumber*100000000+intcurrentnumber);
    END IF;
            
    AOutErrorNumber :=0;
    AOutErrorString :='successful';

EXCEPTION
  WHEN OTHERS THEN
    ROLLBACK;
    v_err_num := SQLCODE;
    v_err_msg  := substr(SQLERRM,1,200);
    AOutErrorNumber := v_err_num;
    AOutErrorString := v_err_msg;
END;