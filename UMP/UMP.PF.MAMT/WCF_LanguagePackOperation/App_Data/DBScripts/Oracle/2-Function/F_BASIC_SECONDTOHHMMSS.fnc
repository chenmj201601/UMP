create or replace function F_BASIC_SECONDTOHHMMSS (li_sec in int)
   return varchar2
as
   ls_return   varchar2 (8); --秒转成时分秒格式00:00:00
begin
  
   if li_sec>=86400 then  --超过24小时
        ls_return:='-';
        
   else
     select    trim (to_char (floor (li_sec / 3600), '00'))
          || ':'
          || trim (to_char (floor (mod (li_sec, 3600) / 60), '00'))
          || ':'
          || trim (to_char (mod (li_sec, 60), '00'))
       into ls_return
       from dual;
            
   end if;
   
   return ls_return;
   
end;
/
