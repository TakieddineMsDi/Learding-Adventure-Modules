using UnityEngine;
using System.Collections;

public class AMHistoryItem {

	private string title;
	private string status;
	private bool isSub;

	/// <summary>
	///  
	/// </summary>
	public AMHistoryItem(string title, string status, bool isSub)
	{
		this.title = title;
		this.status = status;
		this.isSub = isSub;
	}


	public string getTitle(){
		return title;
	}
	public string getStatus(){
		return status;
	}

	public bool getIsSub(){
		return isSub;
	}
}
