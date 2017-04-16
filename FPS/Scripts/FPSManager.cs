using UnityEngine;
using System.Collections;


/// <summary>
/// Cette classe permet de gérer le mode FPS ;
/// - Instantier le composant CustomPlayer et le déteruire selon le besoin



/// </summary>

public delegate void FPSModeDone();
public class FPSManager : MonoBehaviour {


	public event FPSModeDone fpsModeDone;
	public float MaxDelayForFPSMode=120f;

	private string customPlayerPath="Prefabs/CustomPlayer";
	private GameObject fpsPlayer;
	private GameObject mainPlayer;
	private GameObject mainPlayerCamera;
	public  enum PlayerMode{
		FPSPlayer,
		AvatarPlayer,
	}


	public PlayerMode playerMode;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (playerMode==PlayerMode.FPSPlayer)
		if (Input.GetKey (KeyCode.Escape))
			returnToAvatarMode ();
	}

	/// <summary>
	/// Instantier un joueur en mode FPS
	/// </summary>
	public void switchToFPSPlayer()
	{
		GameObject fpsPlayer = Instantiate (Resources.Load (customPlayerPath, typeof(GameObject))) as GameObject;
		mainPlayer = GameObject.Find ("Avatar1/Player");
		mainPlayerCamera = GameObject.Find ("Avatar1/Main Camera");

		fpsPlayer.transform.position = GameObject.Find("Avatar1/Player").transform.position;
		fpsPlayer.name="CustomPlayer";
		// Désactiver la caméra 
		mainPlayer.SetActive(false);
		mainPlayerCamera.SetActive (false);


		GameObject.Find ("ControlMenu").GetComponent<ControlMenu> ().panelInventorySAC.SetActive (false);
		GameObject.Find ("HUDManager1").GetComponent<HUDManager> ().hideMiniMap ();
		GameObject.Find ("Canvas-informations").GetComponent<InformationsCanvas> ().setInformations ("Visez et Tuez les bactéries. Pour quitter le mode FPS tapez échap");
		playerMode = PlayerMode.FPSPlayer;

		StartCoroutine (resetView());
	}

	private IEnumerator resetView()
	{
		yield return new WaitForSeconds (MaxDelayForFPSMode);

		returnToAvatarMode ();

	}


	public void returnToAvatarMode()
	{
	//Réactivation de la caméra principale du joueur

		if (fpsPlayer != null)
			Destroy (fpsPlayer);
		else
			Destroy (GameObject.Find ("CustomPlayer"));


		mainPlayer.SetActive(true);
		mainPlayerCamera.SetActive (true);

		HUDManager hudManager = GameObject.Find ("HUDManager1").GetComponent<HUDManager> ();
		hudManager.resetCursor ();

		hudManager.showHUD ();

		GameObject.Find ("Canvas-informations").GetComponent<InformationsCanvas> ().clearInformations ();

		playerMode = PlayerMode.AvatarPlayer;

		fpsModeDone ();
	}
}
