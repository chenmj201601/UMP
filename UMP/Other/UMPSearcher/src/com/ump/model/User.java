package com.ump.model;

public class User {
	private String ReturnCode;
	private String StrRent;
	private String UserID;
	private String LoginSessionID;
	private String userFullNameString;
	private String OrgID;
	private String orgNameString;
	private String userNameString;
	
	public String getUserNameString() {
		return userNameString;
	}

	public void setUserNameString(String userNameString) {
		this.userNameString = userNameString;
	}

	public User() {
		super();
		
	}

	public User(String ReturnCode, String StrRent,
			String UserID, String LoginSessionID,
			String userFullNameString, String OrgID,
			String orgNameString,String usernameString) {
		super();
		this.ReturnCode = ReturnCode;
		this.StrRent = StrRent;
		this.UserID = UserID;
		this.LoginSessionID = LoginSessionID;
		this.userFullNameString = userFullNameString;
		this.OrgID = OrgID;
		this.orgNameString = orgNameString;
		this.userNameString = usernameString;
		
	}

	public String getReturnCode() {
		return ReturnCode;
	}

	public void setReturnCode(String returnCode) {
		ReturnCode = returnCode;
	}

	public String getStrRent() {
		return StrRent;
	}

	public void setStrRent(String strRent) {
		StrRent = strRent;
	}

	public String getUserID() {
		return UserID;
	}

	public void setUserID(String userID) {
		UserID = userID;
	}

	public String getLoginSessionID() {
		return LoginSessionID;
	}

	public void setLoginSessionID(String loginSessionID) {
		LoginSessionID = loginSessionID;
	}

	public String getUserFullNameString() {
		return userFullNameString;
	}

	public void setUserFullNameString(String userFullNameString) {
		this.userFullNameString = userFullNameString;
	}

	public String getOrgID() {
		return OrgID;
	}

	public void setOrgID(String orgID) {
		OrgID = orgID;
	}

	public String getOrgNameString() {
		return orgNameString;
	}

	public void setOrgNameString(String orgNameString) {
		this.orgNameString = orgNameString;
	}
	
	
}
