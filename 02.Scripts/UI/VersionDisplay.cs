using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private Text text;

    private void Awake() {
        text.text = "v. " + Application.version;
    }
}
