using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class LAHistorique : MonoBehaviour
{

	// tous les message de test s'affiche seulement si debug est True
	private static bool debug = true;

	// initialized indique si on bien récupérer les autres objects (mission, taskmanager, historique JSON)
	public bool initialized = false;

	// indique à l'ouverture du jeu si il existe deja un historique ou en dois charger le jeu normalement
	public bool isHistory = false;

	//indique si on a atteint l'etat enregistrer dans l'historique ou non
	public bool settled = true;

	// pour controller la methode handleProgress
	public bool starthandling = false;

	// pour gerer la methode executewhenfinishsceneLoading in TaskManager
	public int blocked = 1;

	public GameObject contentHA = null;
	public GameObject textHA = null;
	public GameObject contentAM = null;
	public GameObject textAM = null;
	public GameObject rectContent = null;
	public GameObject switchButtonGO = null;

	public Color doneColor;
	public Color activeColor;
	public Color newColor;

	// si le fichier JSON de l'historique est récupérer
	private bool historyLoaded = false;

	//en cas d'historique
	private bool first = true;
	private bool historySaved = false;

	private Configuration configuration;

	private bool switchAMHA = true;

	private GameObject mission = null;
	private GameObject taskManager = null;

	private Mission currentMission = null;
	private Objective currentObjective = null;
	private Task currentTask = null;
	private TaskManager currentTaskManager = null;

	private int indexCurrentTask = 0;

	private static string filePath = null;
	private string selectedMission = null;

	private IEnumerator initializationCoRoutine = null;

	public MissionHistory missionHistory = null;

	private float itemWidth;
	private float itemHeight;
	private float alpha = 5;

	private ArrayList itemList;

	bool firstAdd;
	bool maximize = true;

	// Use this for initialization
	void Start ()
	{
		
		startInitialisation ();
	}

	//boutton pour basculer entre l'affichage de l'historique d'action et l avancement du mission
	public void switchButton ()
	{
		if (switchAMHA) {
			contentAM.SetActive (true);
			contentHA.SetActive (false);
			textAM.SetActive (true);
			textHA.SetActive (false);
			rectContent.GetComponent<ScrollRect> ().content = contentAM.GetComponent<RectTransform> ();
			switchButtonGO.transform.GetChild (0).GetComponent<Text> ().text = "Historiques Indicateurs";
		} else {
			contentAM.SetActive (false);
			contentHA.SetActive (true);
			textAM.SetActive (false);
			textHA.SetActive (true);
			rectContent.GetComponent<ScrollRect> ().content = contentHA.GetComponent<RectTransform> ();
			switchButtonGO.transform.GetChild (0).GetComponent<Text> ().text = "Avancement Mission";
		}
		switchAMHA = !switchAMHA;
	}

	//initialisation du panel historique
	void initPanelHistory ()
	{
		itemList = new ArrayList ();
		GameObject item = contentAM.transform.FindChild ("item").gameObject;
		itemWidth = item.GetComponent<RectTransform> ().rect.width;
		itemHeight = item.GetComponent<RectTransform> ().rect.height;
		itemList.Add (item);
		firstAdd = true;
	}

	public void addHistoryItem (AMHistoryItem amhi)
	{
		if (itemList == null)
			initPanelHistory ();
		//	Debug.Log ("ItemList logueur = "+itemList.Count);
		GameObject item = (GameObject)itemList [itemList.Count - 1];
		if (!firstAdd) {
			GameObject item2 = (GameObject)Instantiate (item, item.transform.position, transform.rotation);
			item2.transform.parent = item.transform.parent;

			float x = item.GetComponent<RectTransform> ().anchoredPosition.x;
			float y = item.GetComponent<RectTransform> ().anchoredPosition.y;		
			item2.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (x, y - itemHeight - alpha, 0);

			if (amhi.getIsSub ()) {
				item2.transform.FindChild ("title").GetComponent<Text> ().text = "            Task : " + amhi.getTitle ();
				item2.transform.FindChild ("status").GetComponent<Text> ().text = amhi.getStatus ();

			} else {
				item2.transform.FindChild ("title").GetComponent<Text> ().text = "Objective : " + amhi.getTitle ();
				item2.transform.FindChild ("status").GetComponent<Text> ().text = amhi.getStatus ();
			}
			if (amhi.getStatus ().Equals (MissionHistory.Active)) {
				item2.transform.FindChild ("title").GetComponent<Text> ().color = activeColor;
				item2.transform.FindChild ("status").GetComponent<Text> ().color = activeColor;
			} else if (amhi.getStatus ().Equals (MissionHistory.Done)) {
				item2.transform.FindChild ("title").GetComponent<Text> ().color = doneColor;
				item2.transform.FindChild ("status").GetComponent<Text> ().color = doneColor;
			} else if (amhi.getStatus ().Equals (MissionHistory.New)) {
				item2.transform.FindChild ("title").GetComponent<Text> ().color = newColor;
				item2.transform.FindChild ("status").GetComponent<Text> ().color = newColor;
			}
			itemList.Add (item2);

		} else {
			//item.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (x, y - itemHeight - alpha, 0);

			if (amhi.getIsSub ()) {
				item.transform.FindChild ("title").GetComponent<Text> ().text = "            Task : " + amhi.getTitle ();
				item.transform.FindChild ("status").GetComponent<Text> ().text = amhi.getStatus ();
			} else {
				item.transform.FindChild ("title").GetComponent<Text> ().text = "Objective : " + amhi.getTitle ();
				item.transform.FindChild ("status").GetComponent<Text> ().text = amhi.getStatus ();
			}
			if (amhi.getStatus ().Equals (MissionHistory.Active)) {
				item.transform.FindChild ("title").GetComponent<Text> ().color = activeColor;
				item.transform.FindChild ("status").GetComponent<Text> ().color = activeColor;
			} else if (amhi.getStatus ().Equals (MissionHistory.Done)) {
				item.transform.FindChild ("title").GetComponent<Text> ().color = doneColor;
				item.transform.FindChild ("status").GetComponent<Text> ().color = doneColor;
			} else if (amhi.getStatus ().Equals (MissionHistory.New)) {
				item.transform.FindChild ("title").GetComponent<Text> ().color = newColor;
				item.transform.FindChild ("status").GetComponent<Text> ().color = newColor;
			}
			firstAdd = false;
		}
		if (maximize) {
			maximize = false;
			RectTransform go = contentAM.GetComponent<RectTransform> ();
			float delta = (itemHeight * (missionHistory.countAll ()));
			go.offsetMin = new Vector2 (go.offsetMin.x, go.offsetMin.y - 5 - delta); 

		}
	}

	//démarrer le coroutine de l'initialisation
	private void startInitialisation (params bool[] setInitialized)
	{
		if (setInitialized.Length != 0) {
			//showDebug ("Void startInitialization : setting Initialized to : " + setInitialized [0]);
			initialized = setInitialized [0];
		}
		starthandling = false;
		initializationCoRoutine = initialization ();
		StartCoroutine (initializationCoRoutine);
	}

	private IEnumerator initialization ()
	{
		if (!initialized) {

			mission = GameObject.Find ("essentials1/mission1");
			taskManager = GameObject.Find ("essentials1/TaskManager1");
			yield return new WaitForSeconds (1f);

			selectedMission = GameObject.Find ("selectedMission").GetComponent<SelectedScene> ().selectedMission;
			//showDebug ("Void initialization : selected Mission : " + selectedMission);

			currentMission = mission.GetComponent<Mission> ();
			currentTaskManager = taskManager.GetComponent<TaskManager> ();

			configuration = GameObject.Find ("configuration").GetComponent<Configuration> ();

			yield return new WaitForSeconds (1f);
			starthandling = true;
			//si on change la scene
			if (currentObjective != null && currentTask != null) {
				//showDebug ("Void initialization : Handling Progress ");
				handleProgress (/*false*/);
			} else {
				//showDebug ("Void initialization : Updating Currents ");
				updateCurrents (true/*,false*/);
			}

			initialized = true;
			//showDebug ("Void initialization : Loading History ");
			loadHistory ();

			updateHistoryPanel ();


			//showDebug ("Initialization Just Ended, Stopping Coroutine ", true);
			StopCoroutine (initializationCoRoutine);
		}
	}


	public void updateHistoryPanel ()
	{
		if (itemList != null) {
			if (itemList.Count > 1) {
				for (int k = 1; k < itemList.Count; k++) {
					Destroy ((GameObject)itemList [k]);
				}
			}
			itemList = null;
		}
		for (int i = 0; i < missionHistory.objectives.Count; i++) {
			addHistoryItem (new AMHistoryItem (missionHistory.objectives [i].label, missionHistory.objectives [i].status, false));
			for (int j = 0; j < missionHistory.objectives [i].tasks.Count; j++) {
				addHistoryItem (new AMHistoryItem (missionHistory.objectives [i].tasks [j].label, missionHistory.objectives [i].tasks [j].status, true));
			}
		}
	}
	//Enregistrer Tout changment (Progress) dans le jeu
	private void handleProgress (/*params bool[] movetoNext*/)
	{
		if (starthandling) {
			if (currentObjective.getLabel ().Equals (currentMission.getActiveObjective ().getLabel ())) {
				//Same Objective Same Task
				/*
			if (currentTask.getLabel ().Equals (currentTaskManager.getCurrentTask ().getLabel ()) && currentTask.getName ().Equals (currentTaskManager.getCurrentTask ().getName ()) && indexCurrentTask == currentMission.getActiveObjective ().currentTask) {
			*/	//Same Objective Task Changed
				if (currentTask.getLabel ().Equals (currentTaskManager.getCurrentTask ().getLabel ()) && currentTask.getName ().Equals (currentTaskManager.getCurrentTask ().getName ()) && indexCurrentTask == currentMission.getActiveObjective ().currentTask) {
				
				} else {
					updateCurrents (false/*,movetoNext*/);
				}
				//Objective And Task Changed
			} else {
				//showDebug (" - Current Task : " + currentTask.getName () + " - " + currentTask.getLabel () + " - Status : " + currentTask.getStatut () + " - isDone : " + currentTask.isDone () + " - isActive : " + currentTask.isActive () + " - isSuccess : " + currentTask.isSuccess (),true);
				updateCurrents (true/*,movetoNext*/);
			}
		}
	}

	private void updateCurrents (bool updateObjective/*,params bool[] movetoNext*/)
	{
		if (starthandling) {
			if (updateObjective) {
				/*currentObjective = currentMission.getActiveObjective ();*/
				currentObjective = currentMission.getActiveObjective ();
			}
			/*currentTask = currentMission.getActiveObjective ().getCurrentTaskInExecution ();*/
			currentTask = currentTaskManager.getCurrentTask ();
			indexCurrentTask = currentMission.getActiveObjective ().currentTask;
			if (historyLoaded) {
				if (initialized) {
					if (settled) {
						bool canSave = false;
						if (isHistory) {
							if (first) {
								first = false;
								historySaved = false;
							} else {
								canSave = true;
							}
						} else {
							canSave = true;
						}
						if (canSave) {
							//showDebug ("void UpdateCurrents : movetonext");
							//showDebug(currentTask.getLabel()+" = "+currentTaskManager.getCurrentTask ().getLabel());
							missionHistory.moveToNext ();

							updateHistoryPanel ();
							historySaved = false;
							showDebug ("saved here cansave");
						} else {
							if (missionHistory.isFirstOH ().Equals (MissionHistory.True)) {
								if (missionHistory.getActiveOH ().isFirstTH ().Equals (MissionHistory.True)) {
									missionHistory.moveToNext ();
									updateHistoryPanel ();
									historySaved = false;
									showDebug ("saved here other one");
								}
							}
						}
					}
				}
			}
		}
	}

	private void loadHistory ()
	{
		if (initialized) {
			prepareHistoryFile ();
			if (!historyLoaded) {
				if (isHistory) {
					//showDebug ("Void loadHistory : History Loaded From JSON", true);
					missionHistory = MissionHistory.getMissionHistoryFromJSON (selectedMission, filePath);
					if (missionHistory.isFirstOH ().Equals (MissionHistory.False)) {
						blocked = 2;
					}
					settled = false;
				} else {
					//showDebug ("Void loadHistory : History Created From Loaded Mission", true);
					missionHistory = MissionHistory.getLoadedMissionHistory (selectedMission, currentMission.getObjectives ());
					settled = true;
				}
				historyLoaded = true;
			}
		}
	}

	private void prepareHistoryFile ()
	{
		filePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "LA5/Configuration") + "/LAHistorique/" + configuration.login + "/" + selectedMission + ".json";
		string folderPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "LA5/Configuration") + "/LAHistorique/" + configuration.login + "/";
		if (!System.IO.File.Exists (filePath)) {
			isHistory = false;
			if (!Directory.Exists (folderPath)) {
				//showDebug ("Void PrepareHistoryFile : Creating LAHIstorique Directory");
				Directory.CreateDirectory (folderPath);
			}
			FileStream Historique = new FileStream (filePath, FileMode.Create);
			Historique.Close ();
			//showDebug ("Void PrepareHistoryFile : History file just created", true);
		} else {
			isHistory = true;
			//showDebug ("Void PrepareHistoryFile : History file exists already ..", true);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (initialized) {
			try {
				if (isHistory) {
					if (!settled) {
						if (!missionHistory.getActiveOH ().getActiveTH ().scene.Equals (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name)) {
							StartCoroutine (restoreSavedState());
						}else{
							if(!Application.isLoadingLevel){
								showDebug("restoring saved state -----------------");
								StartCoroutine (restoreSavedState());
							}
						}
					}
				}
				handleProgress ();
				saveHistory ();
			} catch (Exception e) {
				showDebug ("update exception " + e.StackTrace);
				startInitialisation (false);
			}
		}
	}

	//remetre le jeu dans l'etat ou on a quitter la derniere fois
	public IEnumerator restoreSavedState ()
	{
		yield return new WaitForSeconds (0.2f);
		showDebug ("restore saved state called");
		if (initialized) {
			if (historyLoaded) {
				if (isHistory) {
					if (!settled) {
						showDebug (missionHistory.isFirstOH () + " " + missionHistory.getActiveOH ().isFirstTH ());
						if (missionHistory.getActiveOH ().getActiveTH ().scene.Equals (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name)) {
							bool managed = false;
							for (int i = 0; i < currentMission.getObjectives ().Length; i++) {
								for (int j = 0; j < ((Objective)currentMission.getObjectives ().GetValue (i)).getTasks ().Count; j++) {
									if (((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getLabel ().Equals (missionHistory.getActiveOH ().getActiveTH ().label)
									    && ((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getName ().Equals (missionHistory.getActiveOH ().getActiveTH ().name)
										/*&& missionHistory.getActiveOH ().getActiveTH ().id == (((Objective)currentMission.getObjectives ().GetValue (i)).currentTask-1)*/ && missionHistory.getActiveOH ().getActiveTH ().scene.Equals (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name)) {
										//currentTaskManager.manageTasks ();
										if (!managed) {
											showDebug (" id of mission : " + missionHistory.getActiveOH ().id);
											/*if (missionHistory.getActiveOH ().id > 1) {
											if (missionHistory.getActiveOH ().isFirstTH ().Equals (MissionHistory.True)) {
												currentTaskManager.manageTasks ();
											}


										}*/
											if (missionHistory.isFirstOH ().Equals (MissionHistory.False)) {
												if (missionHistory.getActiveOH ().isFirstTH ().Equals (MissionHistory.True)) {
											        currentTaskManager.manageTasks ();
												}
											}
											settled = true;
											break;
										}
									} else {
										//showDebug ("void restoreSavedState : setting saved state forwarding finished tasks");
										if (missionHistory.getActiveOH ().isFirstTH ().Equals (MissionHistory.True)) {
											if (GameObject.Find (((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getNJ ().getName())!=null) {
												((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getNJ ().disableTarget ();
											}
												//((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getNJ ().disableTarget ();
												((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].setDoneWMT ("success");
											managed = false;
										} else {
											if (GameObject.Find (((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getNJ ().getName())!=null) {
												((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getNJ ().disableTarget ();
											}
												//((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].getNJ ().disableTarget ();
												((Objective)currentMission.getObjectives ().GetValue (i)).getTasks () [j].setDone ("success");
											managed = true;

										}
									}
								}
								if (!((Objective)currentMission.getObjectives ().GetValue (i)).getLabel ().Equals (missionHistory.getActiveOH ().label)) {
									((Objective)currentMission.getObjectives ().GetValue (i)).setDone ();
								} else {
									break;
								}
							}
						} else {
							blocked = 3;
							Application.LoadLevel (missionHistory.getActiveOH ().getActiveTH ().scene);
							yield return new WaitForSeconds (0.2f);
						}
					}
				}
			}
		}
	}

	public void saveTenue ()
	{

	}

	public void saveHistory ()
	{
		if (initialized) {
			if (historyLoaded) {
				//if (!settling) {
				if (!historySaved) {
					//showDebug ("Void saveHistory : History Saved :)");
					System.IO.File.WriteAllText (filePath, missionHistory.toString ());
					//showDebug ("void saveHistory - current task : " + currentTask.getLabel ());
					historySaved = true;
				}
				//}
			}
		}
	}

	public static void showDebug (string sd, params bool[] error)
	{
		if (error.Length != 0) {
			if (debug) {
				if (error [0]) {
					Debug.LogError (sd);
				} else {
					Debug.Log (sd);
				}
			}
		} else {
			Debug.LogError (sd);
		}
	}
}