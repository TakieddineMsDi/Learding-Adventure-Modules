using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
using System;

public class AssessementEngine : MonoBehaviour
{

	// Test de changement on appelant callAssessement sans collider
	//public float timetowait = 10f;
	//public bool done = false;

	//cheming du fichier json qui contient la base des actions
	private string filePath;
	//l'objet indicateur present dans la scene
	public GameObject Indicators;

	private LAJsonReader laJsonReader;
	private JSONNode node = null;

	//Dictionaire qui sére à stocker les action avec leur id unique (Address) pour faciliter la recherche 
	public Dictionary<string, AssessementAction> actions;

	//check si les fichier json est charger et/ou parser
	private bool fileLoaded = false;
	//check si les actions sont parsée
	private bool actionsLoaded = false;

	//charger le fichier json
	private void loadJSON ()
	{
		laJsonReader = new LAJsonReader (filePath);
		laJsonReader.setExternal (true);
		node = laJsonReader.parsing ();
		fileLoaded = true;
	}

	//get version du fichier de configuration
	public string getVersion ()
	{
		if (fileLoaded) {
			return node ["version"].Value;
		} else {
			loadJSON ();
			return node ["version"].Value;
		}
	}

	//get date du fichier de configuration
	public string getDate ()
	{
		if (fileLoaded) {
			return node ["date"].Value;
		} else {
			loadJSON ();
			return node ["date"].Value;
		}
	}

	//debut du parsing du json node
	public void getActions ()
	{
		//si le fichier est chargé
		if (fileLoaded) {
			//si les action ne sont pas chargées
			if(!actionsLoaded){
			    actions = AssessementAction.getActionsFromNode (node);
				actionsLoaded = true;
			}
		} else {
			loadJSON ();
			if (!actionsLoaded) {
				actions = AssessementAction.getActionsFromNode (node);
				actionsLoaded = true;
			}
		}
	}

	//call Assessement utilisé par les colliders pour calculer les nouveau valeurs des indicateur à partir du base des actions
	//string address : id unique d'une action
	//string cause : cause de changement du valeur de l'indicateur
	public void callAssessement(string address,string cause){
		//si les action ne sont pas chargées on les charge au debut
		if (!actionsLoaded) {
			getActions ();
		}
		AssessementAction aa = new AssessementAction ();
		//recupération de l'action à partir du dictionnaire avec le param Address passer par le collider
		actions.TryGetValue (address, out aa);
		//Debug.Log ("getting action "+address+" label : "+aa.label);

		//parcour des indicateur pour trouver la situation -> consequence convenant
		foreach(Indicator idctr in aa.indicators){
			//matches sere à identifier si les valeur en cours sont présent à une des situation d'une action
			bool matches = true;
			//Debug.Log ("checking facts");

			//parcours des fact (situation initial)
			foreach(Fact fct in idctr.facts){
				//Debug.Log ("Fact Value : "+fct.value+" ::::: Current Value : "+Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (fct.label));
				/* format d'un fact "Label":"un label"
				 *                  "Value":"100" c'est une egalité : le valeur en cours doit etre egale au fact pour satisfaire la situation
				 *                          ">100" le valeur en cours doit etre Supérieur au fact
				 *                          "<100" le valeur en cours doit etre Inférieur au fact
				 *         si on trouve pas un "Match" dans les situation "Fact" avec les valeur en cours, on passe au situation suivant "Fact", 
				 * 
				*/
				if (fct.action == Fact.actions.Above) {
					if (fct.value < Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (fct.label)) {
						matches = false;
						//Debug.Log ("******************** Indicators : No Matche Case Found in DataBase");
						break;
					}
				}
				if(fct.action == Fact.actions.Below){
					if (fct.value > Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (fct.label)) {
						matches = false;
						//Debug.Log ("******************** Indicators : No Matche Case Found in DataBase");
						break;
					}
				}
				if(fct.action == Fact.actions.Equal){
					if (fct.value != Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (fct.label)) {
						matches = false;
						//Debug.Log ("******************** Indicators : No Matche Case Found in DataBase");
						break;
					}
				}
			}
			/* si "Maches", alors on a trouvez un situation egal aux valeur en cours
			 *      on applique les consequence
			 *            Value:100 : remplacé 
			 *            Value:+100 Incrementation d'indicateur
			 *            Value:-100 Decrementation d'indicateur
			 * 
			 * 
			 * 
			*/
			if (matches) {
				//Debug.Log ("checking consequences");
				foreach(Consequence csqce in idctr.consequences){
					if (csqce.action == Consequence.actions.Replace) {
						float value = csqce.value;
						Indicators.GetComponent<IndicateursMainHCL> ().setIndicator (csqce.label, value, cause);
						//Debug.Log ("Setting "+csqce.label+" To "+value+" (Was : "+Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (csqce.label)+")");
					}
					if (csqce.action == Consequence.actions.Increment) {
						float value = Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (csqce.label) + csqce.value;
						if(value > 1){
							value = 1;
						}
						Indicators.GetComponent<IndicateursMainHCL> ().setIndicator (csqce.label, value, cause);
						//Debug.Log ("Setting "+csqce.label+" To "+value+" (Was : "+Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (csqce.label)+")");
					}
					if (csqce.action == Consequence.actions.Decrement) {
						float value = Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (csqce.label) - csqce.value;
						if(value < 0){
							value = 0;
						}
						Indicators.GetComponent<IndicateursMainHCL> ().setIndicator (csqce.label, value, cause);
						//Debug.Log ("Setting "+csqce.label+" To "+value+" (Was : "+Indicators.GetComponent<IndicateursMainHCL> ().getIndicatorValue (csqce.label)+")");
					}
				}
				break;
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		/*
		 * getActions ();
		Debug.Log (getVersion());
		Debug.Log (getDate());
		foreach(string x in actions.Keys){
			Debug.Log ("    - Address : " + x);
			AssessementAction aa = new AssessementAction ();
			actions.TryGetValue (x, out aa);
			Debug.Log ("        - Label : "+aa.label);
			Debug.Log ("        - Indicators : ");
			foreach(Indicator y in aa.indicators){
				Debug.Log ("            **** Facts ****");
				foreach(Fact z in y.facts){
					Debug.Log("                "+z.label+" : "+z.value);
				}
				Debug.Log ("            **** Consequences ****");
				foreach(Consequence w in y.consequences){
					Debug.Log("                "+w.label+" : "+w.value+" ::: "+w.action);
				}
			}
			Debug.Log ("        - Bloquant : "+aa.bloquant);

		}
		*/

		filePath= Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "LA5/Configuration") +"/Assessement_Rules.json";
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*
		timetowait -= Time.deltaTime;
		if (timetowait <= 0f) {
			if (!done) {
				done = !done;
				callAssessement ("@28", "cest un cause");
			}
		}
		*/
	}
}