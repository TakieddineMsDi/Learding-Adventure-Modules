using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Taille {
	public float h;
	public float w;

	public static Taille getTailleFromNode(JSONNode node){
		if (node != null) {
			Taille taille = new Taille ();
			taille.h = node ["longueur"].AsFloat;
			taille.w = node ["largeur"].AsFloat;
			return taille;
		} else {
			return null;
		}
	}
}
