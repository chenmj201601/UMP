package com.ump.database;

import java.util.*;

public class PreparedStatementOperator {

	String	sql;
	Vector	parameterTypes;
	Vector	parameters;
	int	size;

        public PreparedStatementOperator() {
		//this.sql = sql;
		parameterTypes = new Vector();
		parameters = new Vector();
		size = 0;
	}

	public PreparedStatementOperator(String sql) {
		this.sql = sql;
		parameterTypes = new Vector();
		parameters = new Vector();
		size = 0;
	}

	public void setSql(String sql) {
		this.sql = sql;
		parameterTypes = new Vector();
		parameters = new Vector();
		size = 0;
	}
	public void setSqlStr(String sql){
	    this.sql = sql;
	}

	public String getSql() {
		return	sql;
	}

	public int size() {
		return	size;
	}

	public void addParameter(int index, int parameterType, Object parameter) {
		parameterTypes.add(index, new Integer(parameterType));
		parameters.add(index, parameter);
		size++;
	}

	public void addParameter(int parameterType, Object parameter) {
		parameterTypes.add(new Integer(parameterType));
		parameters.add(parameter);
		size++;
	}

	public void addParameter(int index, int parameterType, int parameter) {
		parameterTypes.add(index, new Integer(parameterType));
		parameters.add(index, new Integer(parameter));
		size++;
	}

        public void addParameter(int index, int parameterType, long parameter) {
                parameterTypes.add(index, new Integer(parameterType));
                parameters.add(index, new Long(parameter));
                size++;
	}

	public void addParameter(int index, int parameterType, double parameter) {
		parameterTypes.add(index, new Integer(parameterType));
		parameters.add(index, new Double(parameter));
		size++;
	}

	public void addParameter(int parameterType, int parameter) {
		parameterTypes.add(new Integer(parameterType));
		parameters.add(new Integer(parameter));
		size++;
	}

	public int parameterTypeAt(int index) {
		return	((Integer)parameterTypes.elementAt(index)).intValue();
	}

	public Object parameterAt(int index) {
		return	parameters.elementAt(index);
	}

	public String stringParameterAt(int index) {
		return	(String)parameters.elementAt(index);
	}

	public java.sql.Date dateParameterAt(int index) {
		return	(java.sql.Date)parameters.elementAt(index);
	}

	public int intParameterAt(int index) {
		return	((Integer)parameters.elementAt(index)).intValue();
	}

        public long longParameterAt(int index) {
                return	((Long)parameters.elementAt(index)).longValue();
	}
	public double doubleParameterAt(int index) {
		return	((Double)parameters.elementAt(index)).doubleValue();
	}
}
