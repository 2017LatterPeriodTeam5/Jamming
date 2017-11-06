using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultDrawPos : MonoBehaviour {

    private GameObject player;
    public Canvas resultCanvas;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        resultCanvas.transform.position = player.transform.forward;
        resultCanvas.worldCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
