using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlsImage : MonoBehaviour {

	public string url;

	public GameObject screen;

	private Texture texture;

	private IEnumerator showImage;

	public void ShowImage ()
	{
		showImage = LoadImage (url);
		StartCoroutine (showImage);
	}

	//charger l'image
	private IEnumerator LoadImage (string url)
	{
		//start download from the given url
		WWW www = new WWW (url);

		//wait for download to complete
		yield return www;
		//Assign texture
		int width = www.texture.width,height = www.texture.height;
		if (www.texture.width > Screen.width || www.texture.height > Screen.height) {
			www.texture.Resize (Screen.width-50, Screen.height-50);
			width = Screen.width - 80;
			height = Screen.height - 50;
		}
		screen.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		screen.GetComponent<RawImage> ().texture = www.texture;
	}

	public void CloseImage(){
		StopCoroutine (showImage);
		screen.GetComponent<RawImage> ().texture = new Texture ();
		gameObject.SetActive (false);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
