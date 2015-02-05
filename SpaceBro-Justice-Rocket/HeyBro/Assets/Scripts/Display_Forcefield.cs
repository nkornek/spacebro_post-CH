using UnityEngine;
using System.Collections;

public class Display_Forcefield : MonoBehaviour {

	public bool showField;
	public SpriteRenderer forcefield;
	public SphereCollider forceSphere;

	// Use this for initialization
	void Start () {
		forcefield.GetComponent<Renderer>().enabled = false;
		showField = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (showField == true)
		{
			forcefield.GetComponent<Renderer>().enabled = true;
			forceSphere.enabled = true;
		}
		else if (showField == false)
		{
			forcefield.GetComponent<Renderer>().enabled = false;
			forceSphere.enabled = false;
		}

	
	}
}
