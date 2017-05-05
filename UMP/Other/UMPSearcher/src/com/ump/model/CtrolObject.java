package com.ump.model;

public class CtrolObject {
	private String id;
	private String text;
	private String mFullName;
	private String mOrgID;
	
	public CtrolObject() {
		super();
	}
	
	public CtrolObject(String id,String text,String mFullName,String mOrgID) {
		super();
		this.id = id;
		this.text = text;
		this.mFullName = mFullName;
		this.mOrgID = mOrgID;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public String getText() {
		return text;
	}

	public void setText(String text) {
		this.text = text;
	}

	public String getmFullName() {
		return mFullName;
	}

	public void setmFullName(String mFullName) {
		this.mFullName = mFullName;
	}

	public String getmOrgID() {
		return mOrgID;
	}

	public void setmOrgID(String mOrgID) {
		this.mOrgID = mOrgID;
	}

	
}
