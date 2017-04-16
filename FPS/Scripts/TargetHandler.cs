using UnityEngine;
using System.Collections;

public class TargetHandler : MonoBehaviour {

	//public float vanish = 1f;
	public static int bullets = 0;
	public static int lifetimebullets = 0;
	public bool check = false;
	private Animator anime;
	private Animator bkanime;
	private GameObject goBacterie;
	private bool isAnime = true;
	// Use this for initialization
	void Start () {
		bullets++;
		//Debug.Log ("bullets : "+bullets);
	}

	// Update is called once per frame
	void Update () {
		//vanish -= Time.deltaTime;
		//if (vanish <= 0f) {
		if (check == false) {
			if (gameObject.transform.parent.gameObject.tag != "FPS") {
				check = true;
				//Debug.Log ("out of range");
				Destroy (gameObject);
			} else {
				lifetimebullets++;
				if(lifetimebullets == 1){
					anime = gameObject.transform.parent.gameObject.GetComponent<Animator> ();
					anime.enabled = false;
					goBacterie = gameObject.transform.parent.gameObject;
				    //Destroy (anime);
					isAnime = false;
			    }
			}
		}


		if (!isAnime) {
			//Debug.Log (" current health = "+goBacterie.GetComponent<vp_DamageHandler>().CurrentHealth);
			//Debug.Log (" trying to restore animator");
			if (goBacterie.GetComponent<vp_DamageHandler> ().CurrentHealth <= 0) {
				//Debug.Log ("ready to restore");
				anime.enabled = true;
				isAnime = true;
				lifetimebullets = 0;
			}
		}
		//}
	}


}
