using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Fact
{
	//meme structure que le fichier json
	public string label;
	public float value;

	public actions action;

	public enum actions
	{
		Equal,
		Below,
		Above
	};

	public static List<Fact> getFactsFromNode (JSONNode node)
	{
		if (node != null) {
			List<Fact> facts = new List<Fact> ();
			foreach (JSONNode x in node["Facts"].AsArray) {
				Fact f = new Fact ();
				f.label = x ["Label"];
				if (x ["Value"].ToString ().Contains (">")) {
					f.action = actions.Above;
					f.value = float.Parse (x ["Value"].ToString ().Replace (">", "").Replace ("\"", "")) / 100;
				} else if (x ["Value"].ToString ().Contains ("<")) {
					f.action = actions.Below;
					f.value = float.Parse (x ["Value"].ToString ().Replace ("<", "").Replace ("\"", "")) / 100;
				} else {
					f.action = actions.Equal;
					f.value = float.Parse(x ["Value"].ToString ().Replace("\"",""))/100;
				}
				facts.Add (f);
			}
			return facts;
		} else {
			return null;
		}
	}
}
