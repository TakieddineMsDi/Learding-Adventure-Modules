using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Diagnostics;

public class PlayVideo : MonoBehaviour
{

	//Format du lien pour les videos local
	//URL = "file:///" + "C:/Users/takieddine/Desktop/PlayVideoCodeConversion/Assets/PlayVideo/Videos/small.ogg";
	//Format de lien pour les videos streaming
	//URL = "http://www.site.com/videos/video.ogg";

	//URL du video à lire
	public string url;

	//variable bool pour déterminer l'état du video (en lecture, pause, ..)
	private bool isPlaying = false;
	private bool notPresent = true;

	//la durée restante si en met la video en pause
	private float leftDuration = 0f;
	//timer pour calculer le temp de pause et le temp restant
	private Stopwatch timer = new Stopwatch ();

	//le routine pour terminer la video
	private IEnumerator coroutineToStopVideo;   // pour faire appel à la méthode EndVideo!

	public GameObject pausePlayButton;

	//l'ecran qui contient le composant RAW IMAGE
	public GameObject screen;
	//l'AudioSource du video à lire
	public GameObject audioSource;

	private MovieTexture movieTexture;


	private bool activateSound;
	// Use this for initialization
	void Start ()
	{
		/*
		pausePlayButton = GameObject.Find ("ControlsTV/Buttons/PausePlayButton");
		screen = GameObject.Find ("ControlsTV/Screen");
		audioSource = GameObject.Find ("ControlsTV/Audio");
        */
	}

	//calculer le temp restant de la video à l'aide du composant StopWatch
	private float GetPausedDurationFromStopWatch ()
	{
		return (float)(timer.Elapsed.TotalSeconds + (((int)timer.Elapsed.TotalMinutes) * 60) + (((int)timer.Elapsed.TotalHours) * 3600));
	}

	//swap entre les textures pause et play
	private Sprite GetPausePlaySpriteFromState ()
	{
		if (isPlaying) {
			return Resources.Load<Sprite> ("Textures/Pause");
		} else {
			return Resources.Load<Sprite> ("Textures/Play");
		}
	}


	public void PausePlayVideo ()
	{

		UnityEngine.Debug.Log ("this is pausePlayVideo ->" +url);
		if (isPlaying) {
		
			UnityEngine.Debug.Log ("video is playing !!");
			if (movieTexture != null) {
				UnityEngine.Debug.Log ("movie texture !=null");

				timer.Stop ();
				movieTexture.Pause ();
				audioSource.GetComponent<AudioSource> ().Pause ();

				isPlaying = !isPlaying;
				pausePlayButton.GetComponent<Image> ().sprite = GetPausePlaySpriteFromState (); // c'est évident de mêttre un btn play !

				StopCoroutine (coroutineToStopVideo); // ?????? // EndVideo(leftDuration)

				leftDuration -= GetPausedDurationFromStopWatch (); //
				timer.Reset ();
			}

		} else {
			UnityEngine.Debug.Log ("video is not  playing !!");
			isPlaying = !isPlaying;
			pausePlayButton.GetComponent<Image> ().sprite = GetPausePlaySpriteFromState (); // mêttre un btn pause !

			if (notPresent) {
				StartCoroutine(StartVideo (url));
			} else {
				timer.Start ();
				movieTexture.Play ();
				audioSource.GetComponent<AudioSource> ().Play ();

				coroutineToStopVideo = EndVideo (leftDuration);   // première instantiation de coroutine to stop video EndVideo
				StartCoroutine (coroutineToStopVideo);            // EndVideo(0);
			}
		}
	}

	public void StopVideo ()
	{
		UnityEngine.Debug.Log ("this is stop video");
		if (movieTexture != null) {
			if(isPlaying || movieTexture.duration != leftDuration){
				UnityEngine.Debug.Log ("this is stop video-> condition satisfied !!!");
			    isPlaying = false;
				pausePlayButton.GetComponent<Image> ().sprite = GetPausePlaySpriteFromState ();
				timer.Stop ();
				timer.Reset ();
				leftDuration = movieTexture.duration;

				StopCoroutine (coroutineToStopVideo);

				movieTexture.Stop ();
				audioSource.GetComponent<AudioSource> ().Stop ();
				movieTexture = new MovieTexture();

				notPresent = true;
				}
		}
	}

	private IEnumerator hideHUD()
	{ UnityEngine.Debug.Log ("PLAYVIDEO -> hideHUD");
		yield return new WaitForSeconds (0.2f);
		UnityEngine.Debug.Log ("PLAYVIDEO -> hideHUD called !");
		GameObject.Find ("HUDManager1").GetComponent<HUDManager> ().hideHUD ();
	}
	//démarrer la video
	public IEnumerator StartVideo (string url)
	{if(GameObject.Find ("button-sound")!=null)

		GameObject.Find ("button-sound").GetComponent<LASound> ().desactivateSound();
		StartCoroutine (hideHUD());

		WWW www = new WWW (url);

		movieTexture = www.movie;

		while (!movieTexture.isReadyToPlay)
			yield return 0;

		screen.GetComponent<RawImage> ().texture = movieTexture;
		audioSource.GetComponent<AudioSource> ().clip = movieTexture.audioClip;
		movieTexture.Play ();

		audioSource.GetComponent<AudioSource> ().Play ();
		timer.Start ();
		coroutineToStopVideo = EndVideo (movieTexture.duration);

		notPresent = false;
		isPlaying = true;

		leftDuration = movieTexture.duration;
		StartCoroutine (coroutineToStopVideo);
	}

	// Terminer la vider (boutton X)
	public void CloseVideo(){
		if (!notPresent) { // texture introuvable !
			UnityEngine.Debug.Log("this is close video texture present !!");
			//StopCoroutine (coroutineToStopVideo);
			coroutineToStopVideo = EndVideo (0);
			StartCoroutine (coroutineToStopVideo);
		
		} else {
			UnityEngine.Debug.Log("this is close video texture not present !!");
			coroutineToStopVideo = EndVideo (0);
			StartCoroutine (coroutineToStopVideo);
		}
		// il faut mettre une seconde pour réinitialiser les textures ;
		// avant de désactiver l'objet PlayVideo
		StartCoroutine(desactivatePlayVideo());
		GameObject.Find ("HUDManager1").GetComponent<HUDManager> ().showHUD ();
		//GameObject.Find ("ControlMenu").GetComponent<ControlMenu> ().animateButton(ControlMenu.MenuButtons.BAG);

		Sequencer.freeSquencer ();
	}

	private IEnumerator desactivatePlayVideo()
	{
		yield return new WaitForSeconds (0.2f);
		gameObject.SetActive (false);
	}

	//supprimer la video quand elle se termine
	private IEnumerator EndVideo (float duration)
	{
		UnityEngine.Debug.Log ("this is End video before taking effect duration="+duration);

		yield return new WaitForSeconds (duration);
		UnityEngine.Debug.Log ("this is End video taking effect !");
		timer.Stop ();
		timer.Reset ();
		isPlaying = false;
		pausePlayButton.GetComponent<Image> ().sprite = GetPausePlaySpriteFromState ();
		screen.GetComponent<RawImage> ().texture = new Texture ();
		audioSource.GetComponent<AudioSource> ().clip = new AudioClip ();
		movieTexture = new MovieTexture ();
		notPresent = true;

		leftDuration = 0f;
	}
	// Update is called once per frame
	void Update ()
	{

	}
}