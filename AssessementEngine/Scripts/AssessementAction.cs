using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class AssessementAction
{
	//meme structure que le fichier json
	public string label;
	public string address;

	public List<Indicator> indicators;

	public string bloquant;
	// Use this for initialization

	public static Dictionary<string, AssessementAction> getActionsFromNode (JSONNode node)
	{
		if (node != null) {
			Dictionary<string, AssessementAction> AsAction = new Dictionary<string, AssessementAction> ();
			foreach (JSONNode x in node["Actions"].AsArray) {
				AssessementAction aa = new AssessementAction ();
				aa.label = x ["Label"];
				aa.address = x ["Address"];
				aa.indicators = Indicator.getIndicatorsFromNode (x);
				aa.bloquant = x ["Bloquant"];
				AsAction.Add (aa.address, aa);
			}
			return AsAction;
		} else {
			return null;
		}
	}
}
