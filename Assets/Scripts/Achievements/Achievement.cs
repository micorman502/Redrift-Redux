using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Achievement : ScriptableObject {

	public int id;
	public int achievementID;

	public string achievementName;
	public string achievementDesc;
	public Sprite achievementIcon;
}
