using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class MissionHistory
{

	public string name = null;
	public static string Done = "done";
	public static string Active = "active";
	public static string New = "new";
	public static string True = "true";
	public static string NotActive = "notActive";
	public static string False = "false";
	public string status;

	public List<ObjectiveHistory> objectives = null;


	public static MissionHistory getMissionHistoryFromJSON (string selectedMission,string filePath)
	{
		LAJsonReader laJsonReader = new LAJsonReader (filePath);
		laJsonReader.setExternal (true);
		JSONNode node = laJsonReader.parsing ();

		MissionHistory mh = new MissionHistory ();
		//mh.name = selectedMission;

		mh.name = node ["historique"] ["mission"];
		mh.status = node ["historique"] ["status"];

		mh.objectives = ObjectiveHistory.getObjectiveHistoryFromJSON(node ["historique"]);

		return mh;
	}

	public static MissionHistory getLoadedMissionHistory (string selectedMission, Objective[] loadedObjectives)
	{
		MissionHistory mh = new MissionHistory ();
		mh.name = selectedMission;
		mh.status = Active;
		mh.objectives = new List<ObjectiveHistory> ();
		for (int i = 0; i < loadedObjectives.Length; i++) {
			ObjectiveHistory oh = ObjectiveHistory.getLoadedObjectiveHistory (i, loadedObjectives [i]);
			mh.objectives.Add (oh);
		}
		return mh;
	}

	//la tache active est elle la premiere dans cet objective
	public string isFirstOH(){
		if (isActive ()) {
			if (getActiveOH ().id == 0) {
				return MissionHistory.True;
			} else {
				return MissionHistory.False;
			}
		}
		return MissionHistory.NotActive;
	}

	public bool isActive ()
	{
		if (status.Equals (MissionHistory.Active)) {
			return true;
		}
		return false;
	}

	public int countAll (){
		int ca = 0;
		for (int i = 0; i < objectives.Count; i++) {
			ca++;
			for (int j = 0; j < objectives [i].tasks.Count; j++) {
				ca++;
			}
		}
		return ca;
	}

	public ObjectiveHistory getActiveOH ()
	{
		if (isActive ()) {
			for (int i = 0; i < objectives.Count; i++) {
				if (objectives [i].isActive ()) {
					return objectives [i];
				}
			}
		}
		return null;
	}

	public string isLastOH ()
	{
		if (isActive ()) {
			if (getActiveOH ().id == objectives.Count - 1) {
				return MissionHistory.True;
			} else {
				return MissionHistory.False;
			}
		}
		return MissionHistory.NotActive;
	}

	public string moveToNext ()
	{
		if (isActive ()) {
			if (getActiveOH () == null) {
				getFirstOH().setActive ();
				getFirstOH().moveToNext ();
				return MissionHistory.True;
			} else {
				if (isLastOH ().Equals (MissionHistory.True)) {
					if (objectives [getActiveOH ().id].moveToNext ().Equals (MissionHistory.False)) {
						objectives [getActiveOH ().id].setDone ();
						setDone ();
						return MissionHistory.False;
					} else {
						return MissionHistory.True;
					}
				} else {
					if (objectives [getActiveOH ().id].moveToNext ().Equals (MissionHistory.False)) {
						int iD = getActiveOH ().id;
						objectives [iD].setDone ();
						objectives [iD + 1].setActive ();
						objectives [iD + 1].getFirstTH().setActive();
					} else {
						return MissionHistory.True;
					}
				}
			}
		}
		return MissionHistory.NotActive;
	}

	public ObjectiveHistory getFirstOH ()
	{
		return objectives [0];
	}

	public void setActive ()
	{
		status = MissionHistory.Active;
	}

	public void setDone ()
	{
		status = MissionHistory.Done;
	}

	public void setNew ()
	{
		status = MissionHistory.New;
	}

	public string toString ()
	{
		string json = "{\n";
		json += "    \"version\":\"1.0\",\n";
		json += "    \"date\":\"19/05/2016\",\n";
		json += "    \"historique\":{\n";
		json += "        \"mission\":\"" + name + "\",\n";
		json += "        \"status\":\"" + status + "\",\n";
		json += "        \"objectif\":[\n";
		for (int i = 0; i < objectives.Count; i++) {
			if (i == objectives.Count - 1) {
				json += objectives [i].toString () + "\n";
			} else {
				json += objectives [i].toString () + ",\n";
			}
		}
		json += "        ]\n";
		json += "    }\n";
		json += "}\n";
		return json;
	}
}
