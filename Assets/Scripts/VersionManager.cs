using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VersionManager : MonoBehaviour {

	[SerializeField] TMP_Text versionText;

	void Start() {
		versionText.text = "v" + Application.version;
	}
}
