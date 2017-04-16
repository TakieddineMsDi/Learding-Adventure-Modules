using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
using System;
using UnityEngine.SceneManagement;


public class LATuteur : MonoBehaviour {

	//le guide demarre caché
	public bool hidden;

	//nombre des page du Tuteur (deductible à partir du data(LATuteur.JSON))
	private int nbPages;
	private int currentPage = 0;
	//id du tuteur en cours
	private string currentID = null;
	//tuteur en cours
	private Tuteur currentTuteur = null;

    //position pour cacher ou afficher le tuteur 
	private float posf=600f;

	private GameObject background;

	private GameObject titleGO;
	private Text title;
	private GameObject stepsGO;
	private Text steps;

	private GameObject buttons_Ok;
	private GameObject buttons_Close;

	private GameObject content_TextGO;
	private Text content_Text;

	private GameObject content_Video;
	private GameObject content_Image;

	private GameObject content_ControlsTV;

	private GameObject content_ControlsImage;

	private RectTransform rTLATuteur;

	//variable utilisé pour caché la transtion du box d'une position à une autre
	//et pour reveler le guide aprés 1 seconde quand on appui sur suivant
	private float timeToRestoreOpacity = 1f;

	//la restoration d'opacité s'effectue à Update alors on l'initialise par mettre cette var à true
	private bool restoreOpacity = false;

	private string filePath = null;

	private LAJsonReader laJsonReader;
	private JSONNode node = null;

	//Dictionaire qui sére à stocker les tuteur avec leur id unique (ID) pour faciliter la recherche 
	public Dictionary<string, Tuteur> tuteurs;

	//check si les fichier json est charger et/ou parser
	private bool fileLoaded = false;
	//check si les actions sont parsée
	private bool tuteursLoaded = false;

	Scene scene;

	private void setFilePath(){
		filePath= Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "LA5/Configuration") +"/LATuteur.json";
	}

	//charger le fichier json
	private void loadJSON ()
	{
		setFilePath ();
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
	public void getTuteurs ()
	{
		//si le fichier est chargé
		if (fileLoaded) {
			//si les action ne sont pas chargées
			if(!tuteursLoaded){
				tuteurs = Tuteur.getTuteursFromNode (node);
				tuteursLoaded = true;
			}
		} else {
			loadJSON ();
			if (!tuteursLoaded) {
				tuteurs = Tuteur.getTuteursFromNode (node);
				tuteursLoaded = true;
			}
		}
	}

	//change l'opacité du box au cour du temp par les valeur données
	void setOpacity(float newValue){
		Color OldColor = background.GetComponent<Image> ().color;
		background.GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = titleGO .GetComponent<Text> ().color;
		titleGO .GetComponent<Text> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = stepsGO .GetComponent<Text> ().color;
		stepsGO .GetComponent<Text> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = buttons_Ok.GetComponent<Image> ().color;
		buttons_Ok.GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = buttons_Close.GetComponent<Image> ().color;
		buttons_Close.GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = content_TextGO .GetComponent<Text> ().color;
		content_TextGO .GetComponent<Text> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = content_Video.GetComponent<Image> ().color;
		content_Video.GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);

		OldColor = content_Image.GetComponent<Image> ().color;
		content_Image.GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, newValue);
	}


	//tween a value (float) from [From] to [To] in a given Time [Time]
	void tweenValueTo(float From,float To,float Time,string function,string trigger){
		Hashtable param = new Hashtable ();
		param.Add ("from", From);
		param.Add ("to", To);
		param.Add ("time", Time);
		param.Add (trigger, function);
		iTween.ValueTo (gameObject, param);
	}

	//lecture du video si le contenu est une video
	public void VideoButton(){
		Debug.Log (content_Text.text);
		content_ControlsTV.SetActive (true);
		content_ControlsTV.GetComponent<PlayVideo> ().url = content_Text.text;
		content_ControlsTV.GetComponent<PlayVideo> ().PausePlayVideo ();
	}

	//afficher l'image si le contenu est une image
	public void ImageButton(){
		Debug.Log (content_Text.text);
		content_ControlsImage.SetActive (true);
		content_ControlsImage.GetComponent<ControlsImage> ().url = content_Text.text;
		content_ControlsImage.GetComponent<ControlsImage> ().ShowImage();
	}

	//boutton ok,demarrer,suivant ou terminer
	public void OkButton(){
		tweenValueTo (1f, 0f, 0.2f, "setOpacity", "onupdate");
		if (!buttons_Ok.GetComponent<Image> ().sprite.name.Equals ("Terminer")) {
			restoreOpacity = true;
		} else {
			hide ();
		}
		hideArrows ();
	}

	//fermer le tuteur
	public void CloseButton(){
		hide ();
	}

	//demarrer le tuteur avec l'id du tuteur à afficher
	public void StartTuteur(string ID){




		if (!tuteursLoaded) {
			getTuteurs ();
		}
		scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ();
		Tuteur tuteur = null;
		this.currentID = ID;
		tuteurs.TryGetValue (ID, out tuteur);
		this.currentTuteur = tuteur;

		Debug.Log ("cette scène est "+ scene);
		Debug.Log ("la scène du tuteur est "+ tuteur.scene);
		if (scene.name.Equals (tuteur.scene)) {
			if (tuteur != null) {
				Debug.Log ("it's the scene , starting !");
				nbPages = currentTuteur.etapes.Count;
				Sprite sp= Resources.Load<Sprite> ("Textures/Suivant");
				this.currentPage = 0;
				if (buttons_Ok == null)
					Debug.Log ("buttons ok = null !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				if(sp==null)
					Debug.Log ("SP = null !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				buttons_Ok.GetComponent<Image> ().sprite = sp;
				hidden = !hidden;
				OkButton ();
			}
		} else {
			Debug.Log ("this tuteur is not meant for this scene !");
		}
	}

	public void hide()
	{
		if (!hidden) {
			hidden = !hidden;
			hideArrows ();
			this.currentPage = 0;
			this.nbPages = -1;
			Rect r = new Rect(0,-posf,348,165);
			rTLATuteur.anchorMin = new Vector2 (0.5f, 0.5f);
			rTLATuteur.anchorMax = new Vector2 (0.5f, 0.5f);
			rTLATuteur.anchoredPosition3D = new Vector3 (r.position.x,r.position.y,0);
			rTLATuteur.sizeDelta = new Vector2(r.size.x,r.size.y);
			//StartTuteur ("id5");
		}
	}

	/*
	public void showTuteurs(){
		if (!tuteursLoaded) {
			getTuteurs ();
		}
		foreach (string key in tuteurs.Keys) {
			print ("    *********************** ID : "+key);
			Tuteur t = new Tuteur ();
			tuteurs.TryGetValue (key, out t);
			print ("            "+t.scene);
			print ("                Etapes : ");
			foreach (Etape e in t.etapes) {
				print ("                    "+e.type);
				print ("                    "+e.titre);
				print ("                    "+e.contenu);
				print ("                    "+e.taille.h+" * "+e.taille.w);
				print ("                    "+e.ancrage.x+" | "+e.ancrage.y+" | "+e.ancrage.preset+" | "+e.ancrage.angle);
			}
		}
	}
*/
	// Use this for initialization
	void Start () {
		initialization ();
	//	showTuteurs ();
	}

	//cacher tous les indicateur des tuteurs (angle)
	private void hideArrows(){
		foreach (Ancrage.Angles a in System.Enum.GetValues(typeof(Ancrage.Angles))) {
			if (!a.ToString ().Equals ("None")) {
				Color OldColor = GameObject.Find ("LATuteur/Arrows/" + a.ToString()).GetComponent<Image> ().color;
				GameObject.Find ("LATuteur/Arrows/" + a.ToString()).GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, 0);
			}
		}
	}



	private void initialization()
	{
		background = GameObject.Find("LATuteur/Background");

		titleGO = GameObject.Find("LATuteur/Title");
		title = titleGO.GetComponent<Text>();
		stepsGO = GameObject.Find("LATuteur/Steps");
		steps = stepsGO.GetComponent<Text>();

		buttons_Ok = GameObject.Find("LATuteur/Buttons/Ok");


		if (buttons_Ok == null)
			Debug.Log ("cant retreive buttuns !!! ------------------!!!!!!!!!!!!!----------------------");

		buttons_Close = GameObject.Find("LATuteur/Buttons/Close");

		content_TextGO = GameObject.Find("LATuteur/Content/Text");
		content_Text = content_TextGO.GetComponent<Text>();
		content_TextGO.SetActive (false);
		content_Video = GameObject.Find("LATuteur/Content/Video");
		content_Video.SetActive (false);

		content_ControlsTV = GameObject.Find("LATuteurExternals/ControlsTV");
		content_ControlsTV.SetActive (false);

		content_Image = GameObject.Find("LATuteur/Content/Image");
		content_Image.SetActive (false);
		content_ControlsImage = GameObject.Find("LATuteurExternals/ControlsImage");
		content_ControlsImage.SetActive (false);

		rTLATuteur = gameObject.GetComponent<RectTransform>();

		rTLATuteur.localPosition = new Vector3 (rTLATuteur.localPosition.x, rTLATuteur.localPosition.y - posf, rTLATuteur.localPosition.z);


		scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ();


		//StartTuteur ("id1");

	}
	private void setContentActive(bool t){
		content_TextGO.SetActive (t);
		content_Image.SetActive (t);
		content_Video.SetActive (t);
	}
	
	// Update is called once per frame
	void Update () {
		if (restoreOpacity == true) {
			timeToRestoreOpacity -= Time.deltaTime;
			if (timeToRestoreOpacity <= 0f) {
				//restorer les parametre initial
				restoreOpacity = false;
				timeToRestoreOpacity = 1f;

				setContentActive (false);

				hideArrows ();

				if (currentPage < nbPages) {
					currentPage += 1;
				} else {
					currentPage = 0;
					hide ();
				}

				steps.text = currentPage + "/" + nbPages;

				//on change sprite de OkButton à suivant
				if (currentPage >= 1 && currentPage <= nbPages) {
					if (currentPage > 1 && currentPage != nbPages) {
						buttons_Ok.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Textures/Suivant");
					}
					if (currentPage == nbPages) {
						buttons_Ok.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Textures/Terminer");
					}

					//affichage du titre
					title.text = currentTuteur.etapes[currentPage-1].titre;
					//affichage du contenu quelque soit (texte,image ou video)
					content_Text.text = currentTuteur.etapes [currentPage - 1].contenu;

					Rect r = new Rect(currentTuteur.etapes[currentPage-1].ancrage.x,currentTuteur.etapes[currentPage-1].ancrage.y,currentTuteur.etapes [currentPage - 1].taille.w,currentTuteur.etapes [currentPage - 1].taille.h);

					//changer l'ancrage
					rTLATuteur.anchorMin = currentTuteur.etapes [currentPage - 1].ancrage.anchorsMin;
					rTLATuteur.anchorMax = currentTuteur.etapes [currentPage - 1].ancrage.anchorsMax;

					//changer la taille
					rTLATuteur.sizeDelta = new Vector2(r.size.x,r.size.y);
					//changer la position
					rTLATuteur.anchoredPosition3D = new Vector3 (r.position.x,r.position.y,0);

					//si l'indicateur d'angle est != None on change l'opacité du fléche concerné à 1
					if (!currentTuteur.etapes[currentPage - 1].ancrage.angle.ToString ().Equals ("None")) {
						Color OldColor = GameObject.Find ("LATuteur/Arrows/" + currentTuteur.etapes[currentPage - 1].ancrage.angle.ToString()).GetComponent<Image> ().color;
						GameObject.Find ("LATuteur/Arrows/" + currentTuteur.etapes[currentPage - 1].ancrage.angle.ToString()).GetComponent<Image> ().color = new Color (OldColor.r, OldColor.g, OldColor.b, 1);
					}

					//en active le contenu selon le type de l'etape
					if (currentTuteur.etapes [currentPage - 1].type == Etape.Types.Texte) {
						content_TextGO.SetActive (true);
					}else if(currentTuteur.etapes [currentPage - 1].type == Etape.Types.Image){
						content_Image.SetActive (true);
						ImageButton ();
					}else if(currentTuteur.etapes [currentPage - 1].type == Etape.Types.Video){
						content_Video.SetActive (true);
						VideoButton ();
					}

				}

				//on restore l'opacité
				tweenValueTo (0f, 1f, 0.2f, "setOpacity", "onupdate");
			}
		}
	}
}
