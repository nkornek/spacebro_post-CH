﻿using UnityEngine;
using System.Collections;

public class Menu_Music : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.loadedLevel == 2)
		{
			Destroy(gameObject);
		}
	
	}

}
