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

   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);   
   v_ainparam09   VARCHAR2 (20);
   v_ainparam31   VARCHAR2 (20);
   v_ainparam05   VARCHAR2 (20);   
  
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
   

   if  ainparam05 is not null then
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
   else
     UPDATE T_24_001 SET
       C002=ainparam02,   
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
   end if;    

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
