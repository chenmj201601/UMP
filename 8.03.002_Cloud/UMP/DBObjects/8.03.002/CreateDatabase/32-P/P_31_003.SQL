CREATE PROCEDURE P_31_003 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              NVARCHAR2,
   AInParam03              NVARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2,
   AInParam06              VARCHAR2 ,
   AInParam07              VARCHAR2,
   AInParam08              VARCHAR2,
   AInParam09              VARCHAR2,
   AInParam10              VARCHAR2,
   AInParam11              VARCHAR2,
   AInParam13              VARCHAR2,
   AInParam14              VARCHAR2,
   AInParam15              VARCHAR2,
   AInParam16              NVARCHAR2,
   AInParam17              VARCHAR2,
   AInParam18              VARCHAR2,
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS

   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_execsql      VARCHAR2 (1024);

BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   P_00_001 ('31', AInParam18, v_AInParam01, NULL, v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';
     RETURN;
   END IF;

  v_execsql :=
         'INSERT INTO t_31_020_'
      || v_AInParam01
      || '
            VALUES ('''
      || v_serialid
      || ''','''
      || AInParam02
      || ''', '''
      || AInParam03
      || ''','''
      || AInParam04
      || ''','''
      || AInParam05
      || ''',to_date('''
      || AInParam06
      || ''',''yyyy-mm-dd hh24:mi:ss''), '''
      || AInParam07
      || ''','''
      || AInParam08
      || ''',to_date('''
      || AInParam09
      || ''',''yyyy-mm-dd hh24:mi:ss''),''' 
      || AInParam10                         
      || ''','''','''','''                    
      || AInParam11
      || ''',''' 
      || AInParam13
      || ''','''      
      || AInParam14
      || ''',''N'',to_date('''
      || AInParam15
      || ''',''yyyy-mm-dd hh24:mi:ss''),''' 
      || AInParam16                         
      || ''','''','                         
      || AInParam17                         
      || ',0'                                                                        
      || ')';


   EXECUTE IMMEDIATE v_execsql;
   COMMIT;

   AOutParam01 := v_serialid;
   AOutErrornumber := '0';
   AOutErrorstring := 'successful';

EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END;