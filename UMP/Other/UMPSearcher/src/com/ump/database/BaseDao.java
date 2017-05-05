package com.ump.database;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import javax.naming.Context;
import javax.naming.InitialContext;
import javax.naming.NamingException;
import javax.sql.DataSource;

public class BaseDao {
	protected Connection conn;
	private ResultSet rs;

	// 获取连接
	protected Connection getConnection() {
		try {
			Context cxt = new InitialContext();
			DataSource ds = (DataSource) cxt
					.lookup("java:comp/env/jdbc/umpsearcher");
			this.conn = ds.getConnection();
		} catch (NamingException e) {
			e.printStackTrace();
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return conn;
	}

	// 查询
	public ResultSet getResultSet(String sql, Object[] prames) {
		this.getConnection();
		try {
			PreparedStatement smt = conn.prepareStatement(sql);
			if (prames != null) {
				for (int i = 0; i < prames.length; i++) {
					smt.setObject((i + 1), prames[i]);
				}
			}
			this.rs = smt.executeQuery();
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return rs;
	}

	/**
	 * 增删改
	 * 
	 * @param sql
	 * @param prames
	 * @return
	 */
	public boolean getupload(String sql, Object[] prames) {
		int num = 0;
		boolean flag = false;
		this.getConnection();
		try {
			PreparedStatement smt = conn.prepareStatement(sql);
			if (prames != null) {
				for (int i = 0; i < prames.length; i++) {
					smt.setObject((i + 1), prames[i]);
				}
			}
			num = smt.executeUpdate();
			if (num == 1) {
				flag = true;
			}
		} catch (SQLException e) {
			e.printStackTrace();
		} finally {
			this.close();
		}
		return flag;
	}

	// 关闭资源
	public void close() {
		try {
			if (rs != null) {
				rs.close();
			}
			if (conn != null) {
				conn.close();
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	

}