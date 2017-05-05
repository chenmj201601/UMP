package com.ump.model;

public class Permission {
	private String query;
	private String play;
	private String download;
	
	public Permission(String query,String play,String download) {
		super();
		this.query = query;
		this.play = play;
		this.download = download;
	}

	public String getQuery() {
		return query;
	}

	public void setQuery(String query) {
		this.query = query;
	}

	public String getPlay() {
		return play;
	}

	public void setPlay(String play) {
		this.play = play;
	}

	public String getDownload() {
		return download;
	}

	public void setDownload(String download) {
		this.download = download;
	}
}
