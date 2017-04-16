using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Ancrage {

	public float x;
	public float y;
	public enum Presets 
	{
		Middle,
		Top, 
		Down, 
		Right, 
		Left, 
		TopLeft, 
		TopRight, 
		DownLeft, 
		DownRight
	}
	private static Dictionary<int, Presets> PresetsConversion = null;
	public Vector2 anchorsMin = new Vector2();
	public Vector2 anchorsMax = new Vector2();
	public Presets preset;
	//les positions possibles pour les fléche(indiicateur de direction)
	public enum Angles 
	{
		None,
		Top, 
		Down, 
		Right, 
		Left, 
		TopLeft, 
		TopRight, 
		DownLeft, 
		DownRight
	}

	private static Dictionary<int, Angles> AnglesConversion = null;
	public Angles angle;



	public static Ancrage getAncrageFromNode(JSONNode node){
		if (node != null) {
			if (AnglesConversion == null) {
				AnglesConversion = new Dictionary<int, Angles> ();
				AnglesConversion.Add (1, Angles.DownLeft);
				AnglesConversion.Add (2, Angles.Down);
				AnglesConversion.Add (3, Angles.DownRight);
				AnglesConversion.Add (4, Angles.Left);
				AnglesConversion.Add (5, Angles.None);
				AnglesConversion.Add (6, Angles.Right);
				AnglesConversion.Add (7, Angles.TopLeft);
				AnglesConversion.Add (8, Angles.Top);
				AnglesConversion.Add (9, Angles.TopRight);
			}
			if (PresetsConversion == null) {
				PresetsConversion = new Dictionary<int, Presets> ();
				PresetsConversion.Add (1, Presets.DownLeft);
				PresetsConversion.Add (2, Presets.Down);
				PresetsConversion.Add (3, Presets.DownRight);
				PresetsConversion.Add (4, Presets.Left);
				PresetsConversion.Add (5, Presets.Middle);
				PresetsConversion.Add (6, Presets.Right);
				PresetsConversion.Add (7, Presets.TopLeft);
				PresetsConversion.Add (8, Presets.Top);
				PresetsConversion.Add (9, Presets.TopRight);
			}
			Ancrage ancrage = new Ancrage ();
			ancrage.x = node ["posx"].AsFloat;
			ancrage.y = node ["posy"].AsFloat;
			if (node ["preset"].AsFloat >= 0 && node ["preset"].AsFloat <= 9) {
				PresetsConversion.TryGetValue (node ["preset"].AsInt, out ancrage.preset);
				if (ancrage.preset == Presets.Top) {
					ancrage.anchorsMin = new Vector2 (0.5f,1f);
					ancrage.anchorsMax = new Vector2 (0.5f,1f);
				} else if (ancrage.preset == Presets.Middle) {
					ancrage.anchorsMin = new Vector2 (0.5f,0.5f);
					ancrage.anchorsMax = new Vector2 (0.5f,0.5f);
				} else if (ancrage.preset == Presets.Down) {
					ancrage.anchorsMin = new Vector2 (0.5f,0f);
					ancrage.anchorsMax = new Vector2 (0.5f,0f);
				} else if (ancrage.preset == Presets.Right) {
					ancrage.anchorsMin = new Vector2 (1f,0.5f);
					ancrage.anchorsMax = new Vector2 (1f,0.5f);
				} else if (ancrage.preset == Presets.Left) {
					ancrage.anchorsMin = new Vector2 (0f,0.5f);
					ancrage.anchorsMax = new Vector2 (0f,0.5f);
				} else if (ancrage.preset == Presets.TopLeft) {
					ancrage.anchorsMin = new Vector2 (0f,1f);
					ancrage.anchorsMax = new Vector2 (0f,1f);
				} else if (ancrage.preset == Presets.TopRight) {
					ancrage.anchorsMin = new Vector2 (1f,1f);
					ancrage.anchorsMax = new Vector2 (1f,1f);
				} else if (ancrage.preset == Presets.DownLeft) {
					ancrage.anchorsMin = new Vector2 (0f,0f);
					ancrage.anchorsMax = new Vector2 (0f,0f);
				} else if (ancrage.preset == Presets.DownRight) {
					ancrage.anchorsMin = new Vector2 (1f,0f);
					ancrage.anchorsMax = new Vector2 (1f,0f);
				}
			} else {
				ancrage.preset = Presets.Middle;
				ancrage.anchorsMin = new Vector2 (0.5f,0.5f);
				ancrage.anchorsMax = new Vector2 (0.5f,0.5f);
				Debug.Log ("Angles Input Format Error : None Selected!");
			}
			if (node ["angle"].AsFloat >= 0 && node ["angle"].AsFloat <= 9) {
				AnglesConversion.TryGetValue (node ["angle"].AsInt, out ancrage.angle);
			} else {
				ancrage.angle = Angles.None;
				Debug.Log ("Angles Input Format Error : None Selected!");
			}
			return ancrage;
		} else {
			return null;
		}
	}
}
