package com.ump.database;

import java.io.*;
import java.sql.*;
import java.util.*;
import java.sql.Connection;
import oracle.sql.*;

import com.microsoft.sqlserver.jdbc.SQLServerDriver;

public class DBOperator {
	static public final int NUMBER = 2;
	static public final int LONGNUMBER = 3;
	static public final int DOUBLE = 5;
	static public final int VARCHAR2 = 12;
	static public final int DATE = 93;
	static public final int BLOB = 2004;
	static public final int CLOB = 2005;
	private String dataBaseTypeString;
	private String StrDBConnectProfile;
	private String userName;
	private String password;
	private boolean autoCommit = true;

	Connection connection = null;
	ResultSet rs = null;

	public DBOperator(String DatabaseTypeString, String StrDBConnectProfile,
			String StrUserName, String StrPassword) {
		this.dataBaseTypeString = DatabaseTypeString;
		this.StrDBConnectProfile = StrDBConnectProfile;
		this.userName = StrUserName;
		this.password = StrPassword;

		try {
			if (dataBaseTypeString.equals("2")) {
				// MS SQL Server
				DriverManager.registerDriver(new SQLServerDriver());
				connection = DriverManager.getConnection(StrDBConnectProfile,
						this.userName, this.password);
			} else if (dataBaseTypeString.equals("3")) {
				// Oracle
				DriverManager
						.registerDriver(new oracle.jdbc.driver.OracleDriver());
				connection = DriverManager.getConnection(StrDBConnectProfile,
						this.userName, this.password);
			}

		} catch (Exception ex) {
			connection = null;
			System.out.println(connection);
		} finally {
		}
	}

	public int executePreparedStatement(PreparedStatementOperator pso) {

		if (connection == null) {
			return -1;
		}

		PreparedStatement preparedStatement = null;
		try {
			preparedStatement = connection.prepareStatement(pso.getSql());

			for (int i = 0; i < pso.size(); i++) {
				switch (pso.parameterTypeAt(i)) {
				case NUMBER:
					preparedStatement.setInt(i + 1, pso.intParameterAt(i));
					break;
				case LONGNUMBER:
					preparedStatement.setLong(i + 1, pso.longParameterAt(i));
					break;
				case DOUBLE:
					preparedStatement
							.setDouble(i + 1, pso.doubleParameterAt(i));
					break;
				case VARCHAR2:
					preparedStatement
							.setString(i + 1, pso.stringParameterAt(i));
					break;
				case DATE:
					preparedStatement.setDate(i + 1, pso.dateParameterAt(i));
					break;
				}
			}
			preparedStatement.execute();
			preparedStatement.close();
		} catch (Exception ex) {
			try {
				preparedStatement.close();
			} catch (Exception ex1) {
			}
			returnConnection();
			return -1;
		}

		return 1;
	}

	// public int executePreparedStatement(PreparedStatementOperator pso,
	// InputStream inputStream) {
	// ResultSet resultSet = null;
	// int errorCode = 0;
	// int writeAllLength = 0;
	// if (connection == null) {
	// return -1;
	// }
	//
	// PreparedStatement preparedStatement = null;
	// try {
	// preparedStatement = connection.prepareStatement(pso.getSql());
	//
	// for (int i = 0; i < pso.size(); i++) {
	// switch (pso.parameterTypeAt(i)) {
	// case NUMBER:
	// preparedStatement.setInt(i + 1, pso.intParameterAt(i));
	// break;
	// case LONGNUMBER:
	// preparedStatement.setLong(i + 1, pso.longParameterAt(i));
	// break;
	// case VARCHAR2:
	// preparedStatement.setString(i + 1, pso.stringParameterAt(i));
	// break;
	// case DATE:
	// preparedStatement.setDate(i + 1, pso.dateParameterAt(i));
	// break;
	// }
	// }
	// resultSet = preparedStatement.executeQuery();
	// resultSet.next();
	// BLOB blob = ( (OracleResultSet) resultSet).getBLOB(1);
	// errorCode = 4;
	// if (blob == null) {
	// System.out.println("blob is none");
	// }
	// System.out.println("*** for zhang ***");
	// writeAllLength = writeBlob(inputStream, blob);
	//
	// //preparedStatement.execute();
	// resultSet.close();
	// preparedStatement.close();
	//
	// }
	// catch (Exception ex) {
	// try {
	// preparedStatement.close();
	// }
	// catch (Exception ex1) {}
	// returnConnection();
	// return -1;
	// }
	//
	// return 1;
	// }
	//
	// public int executePreparedStatement(PreparedStatementOperator pso,
	// OutputStream outputStream) {
	// ResultSet resultSet = null;
	// int errorCode = 0;
	// int writeAllLength = 0;
	// if (connection == null) {
	// return -1;
	// }
	//
	// PreparedStatement preparedStatement = null;
	// try {
	// preparedStatement = connection.prepareStatement(pso.getSql());
	//
	// for (int i = 0; i < pso.size(); i++) {
	// switch (pso.parameterTypeAt(i)) {
	// case NUMBER:
	// preparedStatement.setInt(i + 1, pso.intParameterAt(i));
	// break;
	// case LONGNUMBER:
	// preparedStatement.setLong(i + 1, pso.longParameterAt(i));
	// break;
	// case VARCHAR2:
	// preparedStatement.setString(i + 1, pso.stringParameterAt(i));
	// break;
	// case DATE:
	// preparedStatement.setDate(i + 1, pso.dateParameterAt(i));
	// break;
	// }
	// }
	// resultSet = preparedStatement.executeQuery();
	// resultSet.next();
	// BLOB blob = ( (OracleResultSet) resultSet).getBLOB(1);
	// errorCode = 4;
	// if (blob == null) {
	// System.out.println("blob is none");
	// }
	// writeAllLength = readBlob(outputStream, blob);
	//
	// //preparedStatement.execute();
	// resultSet.close();
	// preparedStatement.close();
	//
	// }
	// catch (Exception ex) {
	// try {
	// preparedStatement.close();
	// }
	// catch (Exception ex1) {}
	// returnConnection();
	// return -1;
	// }
	//
	// return 1;
	// }

	@SuppressWarnings("unchecked")
	public SelectResultSet executeSelect(PreparedStatementOperator pso) {

		SelectResultSet selectResultSet = null;
		ResultSet resultSet = null;
		PreparedStatement preparedStatement = null;

		if (connection == null) {
			return null;
		}

		try {
			preparedStatement = connection.prepareStatement(pso.getSql());

			for (int i = 0; i < pso.size(); i++) {
				switch (pso.parameterTypeAt(i)) {
				case NUMBER:
					preparedStatement.setInt(i + 1, pso.intParameterAt(i));
					break;
				case LONGNUMBER:
					preparedStatement.setLong(i + 1, pso.longParameterAt(i));
					break;
				case VARCHAR2:
					preparedStatement
							.setString(i + 1, pso.stringParameterAt(i));
					break;
				case DATE:
					preparedStatement.setDate(i + 1, pso.dateParameterAt(i));
					break;
				}
			}
			resultSet = preparedStatement.executeQuery();

		} catch (Exception ex) {
			try {
				preparedStatement.close();
			} catch (Exception ex1) {
			}
			returnConnection();
			return null;
		}
		try {
			ResultSetMetaData resultSetMetaData = resultSet.getMetaData();
			int columnCount = resultSetMetaData.getColumnCount();
			selectResultSet = new SelectResultSet(columnCount);

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
				selectResultSet.setColumnName(columnIndex,
						resultSetMetaData.getColumnName(columnIndex + 1));
				selectResultSet.setColumnType(columnIndex,
						resultSetMetaData.getColumnType(columnIndex + 1));
				selectResultSet.setColumnTypeName(columnIndex,
						resultSetMetaData.getColumnTypeName(columnIndex + 1));
			}

			int rowIndex = 0;
			Vector rowValues;
			while (resultSet.next()) {
				rowValues = new Vector();
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
					rowValues.addElement(resultSet.getObject(columnIndex + 1));
				}
				selectResultSet.addRowValues(rowValues);
				rowIndex++;
			}

			resultSet.close();
			preparedStatement.close();

		} catch (Exception ex) {
			try {
				resultSet.close();
			} catch (Exception ex1) {
			}
			try {
				preparedStatement.close();
			} catch (Exception ex1) {
			}
			return null;
		}

		return selectResultSet;
	}
	
	public int GetDataCount(String strSql)
	{
		int columnCount = 0;
		ResultSet resultSet = null;
		Statement statement = null;
		try {
			if (connection == null) {
				return columnCount;
			}
			statement = connection.createStatement();
			resultSet = statement.executeQuery(strSql);
			while(resultSet.next()){
				 	columnCount = Integer.parseInt(resultSet.getString("CNUM").toString());
				 }
			resultSet.close();
			statement.close();
		} catch (Exception e) {
			try {
				resultSet.close();
			} catch (SQLException e1) {
				e1.printStackTrace();
			}
		}
		return columnCount;
	} 

	@SuppressWarnings({ "rawtypes", "unchecked" })
	public List excuteResultSet(String sqlString) {
		List list = new ArrayList();
		ResultSet resultSet = null;
		Statement statement = null;
		try {
			if (connection == null) {
				return null;
			}
			statement = connection.createStatement();
			resultSet = statement.executeQuery(sqlString);
			ResultSetMetaData md = resultSet.getMetaData();
			int columnCount = md.getColumnCount(); // Map rowData;
			while (resultSet.next()) { // rowData = new HashMap(columnCount);
				Map rowData = new HashMap();
				for (int i = 1; i <= columnCount; i++) {
					rowData.put(md.getColumnName(i), resultSet.getObject(i));
				}
				list.add(rowData);
			}
			resultSet.close();
			statement.close();
		} catch (Exception e) {
			try {
				resultSet.close();
			} catch (SQLException e1) {
				e1.printStackTrace();
			}
		}
		return list;
	}

	public SelectResultSet executeSelect(String sqlString) {

		System.out.println("456zzy");
		System.out.println(sqlString);

		if (sqlString == null)
			return null;

		SelectResultSet selectResultSet = null;
		Statement statement = null;
		ResultSet resultSet = null;

		// connection =DriverManager.getConnection (url, userName, password);

		try {
			if (connection == null) {
				return null;
			}

			statement = connection.createStatement();
			resultSet = statement.executeQuery(sqlString);

			ResultSetMetaData resultSetMetaData = resultSet.getMetaData();

			int columnCount = resultSetMetaData.getColumnCount();

			selectResultSet = new SelectResultSet(columnCount);

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
				selectResultSet.setColumnName(columnIndex,
						resultSetMetaData.getColumnName(columnIndex + 1));
				selectResultSet.setColumnType(columnIndex,
						resultSetMetaData.getColumnType(columnIndex + 1));
				selectResultSet.setColumnTypeName(columnIndex,
						resultSetMetaData.getColumnTypeName(columnIndex + 1));
			}

			int rowIndex = 0;
			Vector rowValues;
			while (resultSet.next()) {
				rowValues = new Vector();
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
					rowValues.addElement(resultSet.getObject(columnIndex + 1));
				}
				selectResultSet.addRowValues(rowValues);
				rowIndex++;
			}

			resultSet.close();
			statement.close();

		} catch (Exception ex) {
			try {
				resultSet.close();
			} catch (Exception ex1) {
			}
			try {
				statement.close();
			} catch (Exception ex1) {
			}
			return null;
		}

		return selectResultSet;
	}

	public int writeBlob(String filename, String sqlstr) {
		int iResult = 0;

		java.sql.PreparedStatement ps = null;
		java.sql.ResultSet rs = null;
		int errorcode = 0;

		if (filename == null || sqlstr == null)
			return -1;

		try {
			errorcode = 1;
			File file = new File(filename); // 建立一个文件对象
			FileInputStream fis = new FileInputStream(file); // 建立一个文件输入流对象
			byte[] fbyte = new byte[(int) file.length()]; // 建立一个字节数组
			int i = fis.read(fbyte); // 将文件读入到字节组中

			connection.setAutoCommit(false);
			String sqlString = sqlstr;
			// blob对象数据从SELECT语句中选出,要修改BLOB,必须添加"FOR UPDATE"语句("否则会出现BLOB未锁定...等错误")
			// 还有其他的方法,比如HOLDLOCK也行,...加锁
			ps = connection.prepareStatement(sqlString);
			errorcode = 2;
			rs = ps.executeQuery();
			if (rs.next()) {
				errorcode = 3;

				/*
				 * weblogic.jdbc.rmi.SerialOracleBlob cast1 =
				 * (weblogic.jdbc.rmi. SerialOracleBlob) rs.getBlob(1);
				 * weblogic.jdbc.rmi.internal.OracleTBlobImpl cast2 =
				 * (weblogic.jdbc.rmi. internal.OracleTBlobImpl)
				 * cast1.getTheRealBlob();
				 * 
				 * oracle.sql.BLOB blob = (oracle.sql.BLOB)
				 * cast2.getTheRealBlob(); // Get BLOB handle
				 * java.io.OutputStream outx = blob.getBinaryOutputStream();
				 * 
				 * errorcode = 5; outx.write(fbyte); outx.flush(); errorcode =
				 * 6;
				 */

				ps.execute("commit");
				System.out.println("insert okey ");
				connection.setAutoCommit(true);
				iResult = 1;
			} else {
				iResult = -1;
			}
			return iResult;
		} catch (Exception ex) {
			iResult = -1;
		} finally {
			try {
				rs.close();
			} catch (Exception e) {
			}
			try {
				ps.close();
			} catch (Exception e) {
			}
			try {
				connection.close();
			} catch (Exception e) {
			}
		}
		return iResult;
	}

	// 此Sql语句必须能确定唯一的BLOB字段
	public int readBLOB(String sqlString, OutputStream outputStream) {

		if (outputStream == null || sqlString == null)
			return -1;

		int errorCode = 0;
		int readAllLength = 0;
		Statement statement = null;
		ResultSet resultSet = null;

		try {
			statement = connection.createStatement();
			errorCode = 1;
			resultSet = statement.executeQuery(sqlString);
			errorCode = 2;

			resultSet.next();
			errorCode = 3;
			// BLOB blob = ( (OracleResultSet) resultSet).getBLOB(1);
			/*
			 * weblogic.jdbc.rmi.SerialOracleBlob cast1 = (weblogic.jdbc.rmi.
			 * SerialOracleBlob) resultSet. getBlob(1);
			 * weblogic.jdbc.rmi.internal.OracleTBlobImpl cast2 =
			 * (weblogic.jdbc.rmi. internal.OracleTBlobImpl)
			 * cast1.getTheRealBlob(); oracle.sql.BLOB blob = (oracle.sql.BLOB)
			 * cast2.getTheRealBlob(); // Get BLOB handle
			 * 
			 * errorCode = 4;
			 * 
			 * readAllLength = readBlob(outputStream, blob);
			 */
			errorCode = 5;
			resultSet.close();
			errorCode = 6;
			statement.close();
			errorCode = 7;

		} catch (Exception ex) {
			try {
				resultSet.close();
			} catch (Exception ex1) {
			}
			try {
				statement.close();
			} catch (Exception ex1) {
			}
			return -2;
		}

		return readAllLength;
	}

	// 实现update Insert
	public int executeSQL(String sql) {

		if (connection == null) {
			return -1;
		}
		int rowCount = 0;
		Statement statement = null;
		try {
			statement = connection.createStatement();
			rowCount = statement.executeUpdate(sql);
			statement.close();
		} catch (Exception ex) {
			ex.printStackTrace();
			try {
				statement.close();
			} catch (Exception ex1) {
			}
			returnConnection();
			return -1;
		}
		// Debug.debugMessage(1,String.valueOf(rowCount));
		return 0;
	}

	// 采用BeginTrans(),commit() rollback() 实现封装事务处理(借鉴delphi)
	public int BeginTrans() {
		if (connection == null) {
			return -1;
		}
		try {
			// 判断是否已经设置AutoComit
			if (!autoCommit) {
				return 1;
			}
		} catch (Exception ex) {
			return -1;
		}
		try {
			autoCommit = false;
			connection.setAutoCommit(autoCommit);

		} catch (Exception ex) {
			returnConnection();
			return -1;
		}
		return 1;

	}

	public int commit() {
		// sandy modify weblogic AutoCommit
		if (connection == null) {
			return -1;
		}

		if (autoCommit) {
			return 1;
		}

		try {
			connection.commit();
			autoCommit = true;
			connection.setAutoCommit(autoCommit);
		} catch (Exception ex) {
			returnConnection();
			return -1;
		}
		return 1;

	}

	public int rollback() {
		if (connection == null) {
			return -1;
		}
		if (autoCommit) {
			return 1;
		}
		try {
			connection.rollback();
			autoCommit = true;
			connection.setAutoCommit(autoCommit);
		} catch (Exception ex) {
			returnConnection();
			return -1;
		}
		return 1;
	}

	public void returnConnection() {
		// 采用weblogic 维护pool
		if (connection != null) {
			// dbConnectionPool.freeConnection( connection);
			connection = null;
		}
	}

	public Connection getDatabaseConnection() {
		// 取得Connection
		if (connection != null) {
		}
		return connection;

	}

	private int writeBlob(InputStream is, BLOB blob) {

		int writeAllLength = 0;
		OutputStream os = null;

		try {
			/*
			 * int iblobChunk = blob.getChunkSize(); byte[] buffer = new
			 * byte[iblobChunk];
			 */
			// Debug.debugMessage(1,"4096");

			os = blob.getBinaryOutputStream();
			// int tempid = blob.getChunkSize();
			int tempid = 1024;
			byte[] buffer = new byte[tempid];
			// Debug.debugMessage(1,"块大小=="+tempid);
			int writeLength;
			while ((writeLength = is.read(buffer)) != -1) {
				os.write(buffer, 0, writeLength);
				writeAllLength += writeLength;
			}
			os.close();
		} catch (Exception ex) {
			try {
				os.close();
			} catch (Exception ex1) {
			}
			writeAllLength = -1;
		}
		return writeAllLength;
	}

	private int readBlob(OutputStream os, BLOB blob) {

		int readAllLength = 0;
		InputStream is = null;

		try {
			/*
			 * int iblobChunk = blob.getChunkSize(); byte[] buffer = new
			 * byte[iblobChunk];
			 */
			byte[] buffer = new byte[1024];

			is = blob.getBinaryStream();

			int readLength;
			while ((readLength = is.read(buffer)) != -1) {
				os.write(buffer, 0, readLength);
				os.flush();
				readAllLength += readLength;
			}
			is.close();

		} catch (Exception ex) {
			// ErrorProcessor.prompt("huadi.database.huadi.database.DBOperator",
			// "readBlob(OutputStream os, BLOB blob) error!", ex);
			try {
				is.close();
			} catch (Exception ex1) {
			}
			readAllLength = -1;
		}

		return readAllLength;
	}

	public void finalize() {
		returnConnection();
	}

	public ResultSet Query(String SQLString) throws SQLException {
		String error;
		ResultSet rs = null;
		try {
			Statement stmt = connection.createStatement();
			rs = stmt.executeQuery(SQLString);
		} catch (SQLException sqle) {
			error = "SQLException: Could not execute the query.";
			throw new SQLException(error);
		}
		return rs;
	}

	public int getMax(String aTable, String AField) {
		int maxid = -1;
		String strSql = null;
		SelectResultSet rs = null;
		String error = null;

		strSql = "SELECT Max(" + AField + ") FROM " + aTable;
		rs = executeSelect(strSql);

		try {
			maxid = rs.getIntValue(0, 0) + 1;
		} catch (Exception e) {

		}
		return maxid;
	}

	// get最大值 where ASubTotolField=ASubTotolFieldVar
	public int getMax(String aTable, String AField, String ASubTotolField,
			int ASubTotolFieldVar) {
		int maxid = -1;
		String strSql = null;
		SelectResultSet rs = null;
		String error = null;
		strSql = "SELECT Max(" + AField + ") FROM " + aTable;
		if (ASubTotolField != null && ASubTotolFieldVar > 0) {
			strSql = strSql + " where " + ASubTotolField + "="
					+ Integer.toString(ASubTotolFieldVar);
		}
		rs = executeSelect(strSql);

		try {
			maxid = rs.getIntValue(0, 0) + 1;
		} catch (Exception e) {

		}
		return maxid;
	}

	// get最大值 where ASubTotolField=ASubTotolFieldVar
	public int getMax(String aTable, String AField, String ASubTotolField,
			int ASubTotolFieldVar, String ASub2TotolField,
			String ASub2TotolFieldVar) {
		int maxid = -1;
		String strSql = null;
		SelectResultSet rs = null;
		String error = null;
		strSql = "SELECT Max(" + AField + ") FROM " + aTable;
		if (ASubTotolField != null && ASubTotolFieldVar > 0) {
			strSql = strSql + " where " + ASubTotolField + "="
					+ Integer.toString(ASubTotolFieldVar) + " and "
					+ ASub2TotolField + "='" + ASub2TotolFieldVar + "'";
		}
		rs = executeSelect(strSql);

		try {
			maxid = rs.getIntValue(0, 0) + 1;

		} catch (Exception e) {
		}
		return maxid;
	}

	// sandy 20030426 add for 返回单位编号
	public int getCompID() {
		int iResult = 0;
		String strSql = null;
		SelectResultSet rs = null;
		strSql = "select SI_VALUE from hasysinfo where SI_NAME='LocalUnitID' and  si_type='dtSysConst' ";
		rs = executeSelect(strSql);
		try {
			// Debug.debugMessage(1, "before add 1 =" +
			// Integer.toString(rs.getIntValue(0, 0)));
			iResult = Integer.parseInt(rs.getStringValue(0, 0));
		} catch (Exception e) {
		}
		return iResult;
	}

	// sandy 20030426 add for 最大的编号(sequence)
	public String getSequenceID(String sequence) {
		String sResult = null;
		String sSEQUENCEID = null;
		String strSql = null;
		SelectResultSet rs = null;
		strSql = "select " + sequence + ".NEXTVAL ID from dual ";
		rs = executeSelect(strSql);
		try {
			// Debug.debugMessage(1, "before add 1 =" +
			// Integer.toString(rs.getIntValue(0, 0)));
			sSEQUENCEID = Integer.toString(rs.getIntValue(0, 0));

			sSEQUENCEID = Integer.toString(getCompID()) + sSEQUENCEID;
			sResult = sSEQUENCEID;
		} catch (Exception e) {
		}
		return sResult;
	}
	/********************************************************************/
	/**
	 * 查询
	 */
	public ResultSet getResultSet(String sql,Object[] prames){
		
		try {
			PreparedStatement smt = connection.prepareStatement(sql);
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
	
	public void close() {
		try {
			if (rs != null) {
				rs.close();
			}
			if (connection != null) {
				connection.close();
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
}