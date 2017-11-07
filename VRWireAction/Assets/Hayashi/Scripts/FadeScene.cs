using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScene : MonoBehaviour {
    private Scene_Manager_Fade sceneFade;
    public string changeSceneName;

	// Use this for initialization
	void Start () {
        sceneFade = Camera.main.GetComponent<Scene_Manager_Fade>();	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sceneFade.LoadSceenWithFade(changeSceneName);
        }
	}
}
