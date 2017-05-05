package com.ump.submeter.service.impl;

import com.ump.submeter.dao.SubmeterDao;
import com.ump.submeter.dao.impl.SubmeterDaoImpl;
import com.ump.submeter.service.SubmeterService;


public class SubmeterServiceImpl implements SubmeterService {

	SubmeterDao dao = new SubmeterDaoImpl(); 
	//确定是否分表
	public int showc004(String c000, String c003) {
		
		return dao.getc004(c000, c003);
	}
	
}
