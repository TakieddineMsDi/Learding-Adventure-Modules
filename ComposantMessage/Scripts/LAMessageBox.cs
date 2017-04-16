using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LAMessageBox : MonoBehaviour {


	public float delay=20f;
	public bool hidden = true;


	private float posf=800f;
	private float Nposf=-800f;
	private Text title;
	private Text message;


	private GameObject OKButton;
	private GameObject background;

	private GameObject titleGO;
	private GameObject messageGO;
	void Start () {

		titleGO = GameObject.Find ("MessageBox/title");
		messageGO = GameObject.Find ("MessageBox/message");

		title = titleGO.GetComponent<Text> ();
		message=messageGO.GetComponent<Text> ();

		OKButton=GameObject.Find ("MessageBox/OKButton");
		background=GameObject.Find ("MessageBox/Background");


	//	hide ();

		background.transform.position = new Vector3 (background.transform.position.x, background.transform.position.y - posf, background.transform.position.z);
		OKButton.transform.position = new Vector3 (OKButton.transform.position.x, OKButton.transform.position.y - posf, OKButton.transform.position.z );
		messageGO.transform.position = new Vector3 (messageGO.transform.position.x, messageGO.transform.position.y - posf, messageGO.transform.position.z);
		titleGO.transform.position = new Vector3 (titleGO.transform.position.x, titleGO.transform.position.y - posf, titleGO.transform.position.z);
		
	}
	


	public void testShow()
	{
		show ("Bienvenue", "Bienvenue dans LA5",1f);
	}
	public void show(string title, string message,float delay)
	{
		this.delay = delay;
		this.title.text = title;
		this.message.text = message;
		if (hidden) {
			iTween.MoveAdd (background, iTween.Hash ("y", posf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			iTween.MoveAdd (OKButton, iTween.Hash ("y", posf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			iTween.MoveAdd (messageGO, iTween.Hash ("y", posf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			iTween.MoveAdd (titleGO, iTween.Hash ("y", posf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			/*	titleGO.SetActive (true);
		messageGO.SetActive (true);
		OKButton.SetActive (true);
		background.SetActive (true);
*/hidden = false;
			StartCoroutine (hideC ());

		}
	}

	public void hide()
	{
		if (!hidden) {
			Debug.Log ("this should Hide MB");
			iTween.MoveAdd (background, iTween.Hash ("y", Nposf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			iTween.MoveAdd (OKButton, iTween.Hash ("y", Nposf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			iTween.MoveAdd (messageGO, iTween.Hash ("y", Nposf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			iTween.MoveAdd (titleGO, iTween.Hash ("y", Nposf, "easeType", iTween.EaseType.easeInOutBack, "delay", .1));
			/*titleGO.SetActive (false);
		messageGO.SetActive (false);
		OKButton.SetActive (false);
		background.SetActive (false); */
			hidden = true;
		}
	}

	private IEnumerator hideC()
	{
		yield return new WaitForSeconds (delay);
		hide ();
	}

}
