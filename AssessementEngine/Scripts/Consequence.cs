using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Consequence
{
	//meme structure que le fichier json
	public string label;
	public float value;

	public actions action;

	public enum actions
	{
		Replace,
		Increment,
		Decrement
	};

	public static List<Consequence> getConsequencesFromNode (JSONNode node)
	{
		if (node != null) {
			List<Consequence> consequences = new List<Consequence> ();

			foreach (JSONNode x in node["Consequences"].AsArray) {
				Consequence c = new Consequence ();
				c.label = x ["Label"];
				if (x ["Value"].ToString ().Contains ("+")) {
					c.action = actions.Increment;
					c.value = float.Parse(x ["Value"].ToString ().Replace("+","").Replace("\"",""))/100;
				} else if (x ["Value"].ToString ().Contains ("-")) {
					c.action = actions.Decrement;
					c.value = float.Parse(x ["Value"].ToString ().Replace('-',' ').Replace("\"",""))/100;
				} else {
					c.action = actions.Replace;
					c.value = float.Parse(x ["Value"].ToString ().Replace("\"",""))/100;
				}
				consequences.Add (c);
			}
			return consequences;
		} else {
			return null;
		}
	}
}
