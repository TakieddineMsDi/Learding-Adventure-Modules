using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Etape {
	//meme structure que le fichier json
	public enum Types {Texte,Video,Image};
	public Types type;
	public string titre;
	public string contenu;
	public Taille taille;
	public Ancrage ancrage;

	public static List<Etape> getEtapesFromNode(JSONNode node)
	{
		if (node != null) {
			List<Etape> etapes = new List<Etape> ();
			foreach (JSONNode x in node["etapes"].AsArray) {
				Etape e = new Etape ();
				foreach (Types t in System.Enum.GetValues(typeof(Types))) {
					if (t.ToString ().Equals (x ["type"])) {
						e.type = t;
						break;
					}
				}
				e.titre = x ["titre"];
				e.contenu = x ["contenu"];
				e.taille = Taille.getTailleFromNode (x["taille"]);
				e.ancrage = Ancrage.getAncrageFromNode(x["ancrage"]);
				etapes.Add (e);
			}
			return etapes;
		} else {
			return null;
		}
	}
}
