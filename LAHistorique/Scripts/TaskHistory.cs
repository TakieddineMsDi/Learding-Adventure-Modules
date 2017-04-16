using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

//cette classe est le modele pour enregistrer l'historique d'une tache
public class TaskHistory {

	public int id;
	public string name = null;
	public string label = null;
	public string scene = null;
	public string status = null;

	//phase d'initialisation 
	//initialiser MissionHistory a partir du fichier json
	public static List<TaskHistory> getTaskHistoryFromJSON(JSONNode x){
		List<TaskHistory> lth = new List<TaskHistory>();
		foreach (JSONNode y in x["tache"].AsArray) {
			TaskHistory th = new TaskHistory ();
			th.id = y["id"].AsInt;
			th.label = y["label"];
			th.name = y["name"];
			th.scene = y["scene"];
			th.status = y["status"];
			lth.Add (th);
		}
		return lth;
	}

	//phase d'initialisation 
	//initialiser MissionHistory pour la premiere foi (pa d'historique) avec les tache charger par le moteur de scénario
	public static TaskHistory getLoadedTaskHistory(int iD,Task task){
		TaskHistory th = new TaskHistory ();
		th.id = iD;
		th.name = task.getName ();
		th.label = task.getLabel ();
		th.scene = task.getScene ();
		th.status = task.getStatut ();
		return th;
	}

	//verifier si la tache est active
	public bool isActive(){
		if (status.Equals (MissionHistory.Active)) {
			return true;
		}
		return false;
	}

	//changer la status du tache
	public void setActive(){
		status = MissionHistory.Active;
	}

	public void setDone(){
		status = MissionHistory.Done;
	}

	public void setNew(){
		status = MissionHistory.New;
	}

	//au format JSON
	public string toString(){
		string json = "                    {\n";
		json +=       "                        \"id\":\""+id+"\",\n";
		json +=       "                        \"name\":\""+name+"\",\n";
		json +=       "                        \"label\":\""+label+"\",\n";
		json +=       "                        \"scene\":\""+scene+"\",\n";
		json +=       "                        \"status\":\""+status+"\"\n";
		json +=       "                    }";
		return json;
	}
}
