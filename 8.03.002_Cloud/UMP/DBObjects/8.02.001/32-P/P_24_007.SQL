CREATE  PROCEDURE P_24_007 (
   ainparam01              NVARCHAR2,
   ainparam02              VARCHAR2,
   ainparam05              VARCHAR2,
   ainparam06              VARCHAR2,
   ainparam09              VARCHAR2,
   ainparam10              VARCHAR2,
   ainparam11              VARCHAR2,
   ainparam12              VARCHAR,
   ainparam13              VARCHAR2,
   ainparam14              VARCHAR2,
   ainparam15              VARCHAR2,
   ainparam16              VARCHAR2,
   ainparam17              VARCHAR2,
   ainparam18              VARCHAR2,
   ainparam19              VARCHAR2,
   ainparam20              VARCHAR2,
   ainparam23              VARCHAR2,
   ainparam24              VARCHAR2,
   ainparam30              VARCHAR2,
   ainparam31             VARCHAR2,
   ainparam35              NVARCHAR2,
   aouterrornumber   OUT   NUMBER,
   aouterrorstring   OUT   NVARCHAR2
)
IS
-- description:    修改加密密钥策略配置表 T_24_001 
-- create date:    <2015-7-23>
-- =========================================================================================
--AInParam01        PolicyID 策略编码。402
--AInParam02        PolicyName 策略名称。
--AInParam05        类型为 “用户输入”的生成密钥时间，必须晚于当前时间。如果值为 20140101000000 表示立即执行
--AInParam06        TypeUEncryptKey 类型为 “用户输入”时加密密钥。启用该密钥时间为TypeUTime，数据使用Key_Normal1加密处理
--AInParam09        DurationEnd 持续时间（结束 UTC）。UNLIMITED表示无结束日期效
--AInParam10        BeginDayOfCycle 执行周期变量。“TypeCCycle = W”时   0：周日；1：周一。默认为 0；“TypeCCycle = U”时   填写实际天数。如：10、30、60、120。0
--AInParam11        ResetCycle 立即更新密钥后执行周期是否重置。'0'否；'1'是。(只针对 “重复执行周期”类型为 “U：自定义天数”有效)
--AInParam12        ComplexityAbled 密码复杂性校验。'0'、不校验；'1'、校验。
--AInParam13        MustContainCapitals 必须包含大写字母。 A ～ Z
--AInParam14        NumbersCapitals 包含大写字母个数。
--AInParam15        MustContainLower 必须包含小写字母。a ~ z
--AInParam16        包含小写字母个数
--AInParam17        必须包含数字。 0 ～ 9
--AInParam18        包含数字个数。
--AInParam19        必须包含特殊字符。如：! $ # %
--AInParam20        包含特殊字符个数。
--AInParam23        最短长度。
--AInParam24        最长长度
--AInParam30        LastModifiedUser 策略最后修改人
--AInParam31        LastModifiedTime 策略最后修改时间 UTC
--AInParam35        POLICYNOTES策略描述

-- ================================================
--aoutparam01       用户ID
-- ================================================
--流水号 模块号 24 操作码 402
-- ================================================
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);   
   v_ainparam09   VARCHAR2 (20);
   v_ainparam31   VARCHAR2 (20);
   v_ainparam05   VARCHAR2 (20);   
   --v_SQL          VARCHAR2 (4000);   
BEGIN

   v_ainparam05:=replace(ainparam05,'-');
   v_ainparam05:=replace(v_ainparam05,' ');
   v_ainparam05:=replace(v_ainparam05,':');


   v_ainparam09:=replace(ainparam09,'-');
   v_ainparam09:=replace(v_ainparam09,' ');
   v_ainparam09:=replace(v_ainparam09,':');


   v_ainparam31:=replace(ainparam31,'-');
   v_ainparam31:=replace(v_ainparam31,' ');
   v_ainparam31:=replace(v_ainparam31,':');
   
/*   v_SQL:='UPDATE T_24_001 SET C002='||ainparam02
   ||',C005='||ainparam05||',
     C006='||ainparam06||',
     C009='||v_ainparam09||',
     C010='||ainparam10||',
     C011='||ainparam11||',
     C012='||ainparam12||',
     C013='||ainparam13||',
     C014='||ainparam14||',
     C015='||ainparam15||',
     C016='||ainparam16||',
     C017='||ainparam17||',
     C018='||ainparam18||',
     C019='||ainparam19||',
     C020='||ainparam20||',
     C023='||ainparam23||',
     C024='||ainparam24||',
     C030='||ainparam30||',
     C031='||v_ainparam31||',C035='||ainparam35||'
   WHERE C001= '||ainparam01;
   DBMS_OUTPUT.put_line(v_SQL);*/

   UPDATE T_24_001 SET
     C002=ainparam02,   
     C005=V_ainparam05,
     C006=ainparam06,
     C009=v_ainparam09,
     C010=ainparam10,
     C011=ainparam11,
     C012=ainparam12,
     C013=ainparam13,
     C014=ainparam14,
     C015=ainparam15,
     C016=ainparam16,
     C017=ainparam17,
     C018=ainparam18,
     C019=ainparam19,
     C020=ainparam20,
     C023=ainparam23,
     C024=ainparam24,
     C030=ainparam30,
     C031=v_ainparam31,
     C035=ainparam35
   WHERE C001= ainparam01;
     
   COMMIT;

   aouterrornumber := '0';
   aouterrorstring := 'successful';
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END;
