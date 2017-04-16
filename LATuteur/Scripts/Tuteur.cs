using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Tuteur{
	//meme structure que le fichier json
	public string scene;
	public string id;

	public List<Etape> etapes;

	public static Dictionary<string, Tuteur> getTuteursFromNode (JSONNode node)
	{
		if (node != null) {
			Dictionary<string, Tuteur> tuteurs = new Dictionary<string, Tuteur> ();
			foreach (JSONNode x in node["tuteurs"].AsArray) {
				Tuteur t = new Tuteur ();
				t.scene = x ["scene"];
				t.id = x ["id"];
				t.etapes = Etape.getEtapesFromNode (x);
				tuteurs.Add (t.id, t);
			}
			return tuteurs;
		} else {
			return null;
		}
	}
}
