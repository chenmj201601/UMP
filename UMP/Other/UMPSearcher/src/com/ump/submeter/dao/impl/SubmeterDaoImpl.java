package com.ump.submeter.dao.impl;

import java.sql.ResultSet;
import java.sql.SQLException;

import com.ump.database.BaseDao;
import com.ump.submeter.dao.SubmeterDao;

public class SubmeterDaoImpl extends BaseDao implements SubmeterDao {
	/**
	 * 确定是否分表
	 */
	public int getc004(String c000, String c003) {
		int number = 0 ;
		try {
			String sql = "select count(*) from T_00_000 where c000 = ? and c003=? ";
			Object[] prames = { c000, c003 };
			ResultSet rs = this.getResultSet(sql, prames);
			while (rs != null && rs.next()) {
				number = rs.getInt(1);
				System.out.println(number);
			}
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return number;
	}

}
