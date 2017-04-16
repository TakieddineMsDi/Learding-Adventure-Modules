using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Indicator
{
	//meme structure que le fichier json
	public List<Fact> facts;
	public List<Consequence> consequences;

	public static List<Indicator> getIndicatorsFromNode (JSONNode node)
	{
		if (node != null) {
			List<Indicator> indicators = new List<Indicator> ();
			foreach (JSONNode x in node["Indicators"].AsArray) {
				Indicator i = new Indicator ();
				i.facts = Fact.getFactsFromNode (x);
				i.consequences = Consequence.getConsequencesFromNode (x);
				indicators.Add (i);
			}
			return indicators;
		} else {
			return null;
		}
	}
}
