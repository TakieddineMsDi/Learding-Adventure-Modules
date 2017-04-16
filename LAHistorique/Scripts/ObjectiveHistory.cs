using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ObjectiveHistory{

	public int id;
	public string label = null;

	public string status;

	public List<TaskHistory> tasks;

	//phase d'initialisation 
	//initialiser MissionHistory pour la premiere foi (pa d'historique) avec les tache charger par le moteur de scénario
	public static ObjectiveHistory getLoadedObjectiveHistory(int iD,Objective obj){
		ObjectiveHistory oh = new ObjectiveHistory ();
		oh.id = iD;
		oh.label = obj.getLabel ();
		oh.status = obj.getStatut();
		oh.tasks = new List<TaskHistory>();
		for (int i = 0; i < obj.getTasks().Count; i++) {
			TaskHistory th = TaskHistory.getLoadedTaskHistory(i,obj.getTasks()[i]);
			oh.tasks.Add (th);
		}
		return oh;
	}



	//phase d'initialisation 
	//initialiser MissionHistory a partir du fichier json
	public static List<ObjectiveHistory> getObjectiveHistoryFromJSON(JSONNode x){
		List<ObjectiveHistory> loh = new List<ObjectiveHistory>();
		foreach (JSONNode y in x["objectif"].AsArray) {
			ObjectiveHistory oh = new ObjectiveHistory ();
			oh.id = y["id"].AsInt;
			oh.label = y["label"];
			oh.status = y["status"];
			oh.tasks = TaskHistory.getTaskHistoryFromJSON (y);
			loh.Add (oh);
		}
		return loh;
	}

	public bool isActive(){
		if (status.Equals (MissionHistory.Active)) {
			return true;
		}
		return false;
	}

	//la tache active dans cet objective
	public TaskHistory getActiveTH(){
		if (isActive ()) {
			for (int i = 0; i < tasks.Count; i++) {
				if (tasks [i].isActive ()) {
					return tasks [i];
				}
			}
		}
		return null;
	}

	//la tache active est elle la derniere dans cet objective
	public string isLastTH(){
		if (isActive ()) {
			if (getActiveTH ().id == tasks.Count - 1) {
				return MissionHistory.True;
			} else {
				return MissionHistory.False;
			}
		}
		return MissionHistory.NotActive;
	}

	//la tache active est elle la premiere dans cet objective
	public string isFirstTH(){
		if (isActive ()) {
			if (getActiveTH ().id == 0) {
				return MissionHistory.True;
			} else {
				return MissionHistory.False;
			}
		}
		return MissionHistory.NotActive;
	}

	//activer la tache suivante et enregistrer l'historique
	public string moveToNext(){
		if (isActive ()) {
			if (getActiveTH () == null) {
				getFirstTH().setActive ();
				return MissionHistory.True;
			} else {
				if (isLastTH ().Equals (MissionHistory.True)) {
					tasks [getActiveTH ().id].setDone ();
					return MissionHistory.False;
				} else {
					int iD = getActiveTH ().id;
					tasks [iD].setDone ();
					tasks [iD + 1].setActive ();
					return MissionHistory.True;
				}
			}
		}
		return MissionHistory.NotActive;
	}

	public TaskHistory getFirstTH(){
		return tasks [0];
	}

	public void setActive(){
		status = MissionHistory.Active;
	}

	public void setDone(){
		status = MissionHistory.Done;
	}

	public void setNew(){
		status = MissionHistory.New;
	}

	public string toString(){
		string json = "            {\n";
		json +=       "                \"id\":\""+id+"\",\n";
		json +=       "                \"label\":\""+label+"\",\n";
		json +=       "                \"status\":\""+status+"\",\n";
		json +=       "                \"tache\":[\n";
		for (int i = 0; i < tasks.Count; i++) {
			if (i == tasks.Count - 1) {
				json += tasks[i].toString()+"\n";
			}else{
				json += tasks[i].toString()+",\n";
			}
		}
		json +=       "                ]\n";
		json +=       "            }";
		return json;
	}
}
