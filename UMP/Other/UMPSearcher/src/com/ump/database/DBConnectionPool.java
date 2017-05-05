package com.ump.database;

import java.util.*;
import java.sql.*;
import com.microsoft.sqlserver.jdbc.SQLServerDriver;



public class DBConnectionPool {

	private String dataBaseTypeString;
	private String  StrDBConnectProfile;
	@SuppressWarnings("rawtypes")
	private Vector connections;
	private int maxConnectionAmount;
	private int checkoutAmount;
    @SuppressWarnings("unused")
	private static DBConnectionPool instance;
	private String userName;
	private String password;  

//        public synchronized static DBConnectionPool getInstance() {
//        if (instance == null) {
//                instance = new DBConnectionPool();
//        }
//        return instance;
//	}

//        private DBConnectionPool() {
//        	
//        	this.dataBaseTypeString="3";
//            this.ip="WIN-CI0DBLMEPP8";
//            this.port="1521";
//            this.sid="PFOrcl";
//            this.userName="elearning";
//            this.password="1";
//          
//          this.maxConnectionAmount=30;
//          connections = new Vector();
//          checkoutAmount = 0;
//	}
      @SuppressWarnings("rawtypes")
	public  DBConnectionPool(String DatabaseTypeString,String StrDBConnectProfile,String StrUserName,String StrPassword){
    	  this .dataBaseTypeString= DatabaseTypeString;
    	  this.StrDBConnectProfile=StrDBConnectProfile;
    	  this.userName= StrUserName;
    	  this.password= StrPassword;
    	  this.maxConnectionAmount=30;
    	  connections = new Vector();
    	  checkoutAmount = 0;
	}

	@SuppressWarnings("rawtypes")
	public void registerDBConnectionPool(int maxConnectionAmount) {		
		this.maxConnectionAmount = maxConnectionAmount;
                if (connections == null) {
                   connections = new Vector();
		   checkoutAmount = 0;
                }
	}

	public synchronized Connection getConnection() {
		Connection connection = null;
		if(connections.size() > 0) {
			connection = (Connection)connections.remove(0);
			if(connection != null)
				checkoutAmount++;
			try {
				if (connection.isClosed()) {
					connection = getConnection();
				}
				connection.rollback();
				connection.setAutoCommit(false);
			} catch(Exception ex) {
				connection = getConnection();
			}
		}
		else {
			if(checkoutAmount < maxConnectionAmount) {
				newConnection();
				connection = getConnection();
			}
			else {
				connection = null;
				System.out.println("超过最大连接数");
			}
		}
		return	connection;
	}

	public synchronized Connection getConnection(long timeout) {

		long startTime = new java.util.Date().getTime();

		Connection connection = null;
		while ((connection = getConnection()) == null) {
			try {
				wait(timeout);
			} catch (InterruptedException e) {}
			if ((new java.util.Date().getTime() - startTime) >= timeout) {
				return null;
			}
		}
		return connection;
	}

	@SuppressWarnings("unchecked")
	public synchronized void freeConnection(Connection connection) {
		if(connection == null)
			return;

		try {
			connection.rollback();
			connections.addElement(connection);
			connection = null;
			checkoutAmount--;
			notifyAll();
		} catch(Exception ex) {
			try { connection.close(); } catch(Exception ex1) {}
		}
	}

	@SuppressWarnings("unchecked")
	public void newConnection() {
		try {
			Connection connection = null;			
			if (dataBaseTypeString=="2") {
				//MS SQL Server
				DriverManager.registerDriver(new SQLServerDriver());
				connection=DriverManager.getConnection(StrDBConnectProfile,this.userName,this.password);			
				
			}else if (dataBaseTypeString=="3") {
				//Oracle
				 DriverManager.registerDriver (new oracle.jdbc.driver.OracleDriver());
				 connection = DriverManager.getConnection (StrDBConnectProfile,this.userName,this.password);			 
			}			
           connection.setAutoCommit(false);
           connections.add(connection);			

		} catch(Exception ex) {
			return;
		}
	}

	public int getConnectionAmount() {
		return connections.size();
	}

	public void setMaxConnectionAmount(int amount) {
		maxConnectionAmount = amount;
	}
}
