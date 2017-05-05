package com.ump.database;

import java.util.*;

public class SelectResultSet {

	int	columnCount;
	int	rowCount;

	String[]	columnNames;
	int[]		columnTypes;
	String[]	columnTypeNames;
	Vector		values;

	public SelectResultSet(int columnCount) {
		this.columnCount = columnCount;
		rowCount = 0;

		columnNames = new String[columnCount];
		columnTypes = new int[columnCount];
		columnTypeNames = new String[columnCount];
		values = new Vector();
	}

	public int getColumnCount() {
		return	columnCount;
	}

	public int getRowCount() {
		return	rowCount;
	}

	public String getXMLResultSegment() {
		String	xmlResult = "<?xml version=\"1.0\" encoding=\"GB2312\" standalone=\"yes\"?>\n";

		xmlResult = xmlResult + "<SelectResultSet>\n";
		if(rowCount > 0) {

			for(int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				xmlResult = xmlResult + "<SelectRow id = \"" + rowIndex + "\">\n";
				xmlResult = xmlResult + getXMLResultSegment(rowIndex);
				xmlResult = xmlResult + "</SelectRow>\n";
			}

		}
		xmlResult = xmlResult + "</SelectResultSet>\n";
		return	xmlResult;
	}

	public String getXMLResultSegment(int rowIndex) {

		String	xmlResult = new String();

		if(rowCount <= 0 || rowIndex >= rowCount)
			return	xmlResult;

		String	str;
		for(int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
			if(getObjectValue(columnIndex, rowIndex) == null) {
				str = "";
			}
			else {
				str = getObjectValue(columnIndex, rowIndex).toString();
			}
			xmlResult = xmlResult + XMLUtil.buildXMLString(getColumnName(columnIndex), str);
		}

		return	xmlResult;
	}

	public Object getObjectValue(int columnIndex, int rowIndex) {
		Vector	rowValues = (Vector)values.elementAt(rowIndex);
		return	(Object)rowValues.elementAt(columnIndex);
	}

	/*
    public int getIntValue(int columnIndex, int rowIndex) {
		return	((Integer)getObjectValue(columnIndex, rowIndex)).intValue() ;
	}
    */

	public int getIntValue(int columnIndex, int rowIndex) {
            int tempValue = 0;
            try{
                tempValue = ((java.math.BigDecimal)getObjectValue(columnIndex, rowIndex)).intValue();
                //return new Integer(rs.getObjectValue(0,i).toString()).intValue();
            }catch(Exception e){
		//e.printStackTrace();
                //System.out.println("取值为零");
                tempValue = 0;
            }

	    return tempValue;
        }

	public long getLongValue(int columnIndex, int rowIndex) {
            long tempValue = 0;
            try{
                tempValue = ((java.math.BigDecimal)getObjectValue(columnIndex, rowIndex)).longValue();
                //return new Integer(rs.getObjectValue(0,i).toString()).intValue();
            }catch(Exception e){
                //System.out.println("取值为零");
                tempValue = 0;
            }

	    return tempValue;
        }

	public double getDoubleValue(int columnIndex, int rowIndex) {
            double tempValue = 0;
            try{
                tempValue = ((java.math.BigDecimal)getObjectValue(columnIndex, rowIndex)).doubleValue();
                //return new Integer(rs.getObjectValue(0,i).toString()).intValue();
            }catch(Exception e){
		//e.printStackTrace();
                tempValue = 0;
            }

	    return tempValue;
        }

	public String getStringValue(int columnIndex, int rowIndex) {
	    return  (String)getObjectValue(columnIndex, rowIndex);
	}
	public java.sql.Date getDateValue(int columnIndex, int rowIndex) {
            java.util.Date tempDate = null;
            try{
            tempDate =  (java.util.Date)(java.sql.Timestamp)getObjectValue(columnIndex, rowIndex);
            }catch(Exception e){ tempDate = null;}
            if (tempDate == null)
                return null;
            else
                return  new java.sql.Date(tempDate.getTime());
        }

        public java.util.Date getUtilDateValue(int columnIndex, int rowIndex) {
            java.util.Date tempDate = null;
            try{
            tempDate =  (java.util.Date)(java.sql.Timestamp)getObjectValue(columnIndex, rowIndex);
            }catch(Exception e){ tempDate = null;}
            if (tempDate == null)
                return null;
            else
                return  tempDate;
        }

	public void setColumnName(int columnIndex, String columnName) {
	    columnNames[columnIndex] = columnName;
	}

	public String getColumnName(int columnIndex) {
		return	columnNames[columnIndex];
	}

	public void setColumnType(int columnIndex, int columnType) {
		columnTypes[columnIndex] = columnType;
	}

	public int getColumnType(int columnIndex) {
		return	columnTypes[columnIndex];
	}

	public void setColumnTypeName(int columnIndex, String columnTypeName) {
		columnTypeNames[columnIndex] = columnTypeName;
	}

	public String getColumnTypeName(int columnIndex) {
		return	columnTypeNames[columnIndex];
	}

	public void addRowValues(Vector rowValues) {
		values.addElement(rowValues);
		rowCount++;
	}

}