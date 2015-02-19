using UnityEngine;
using System.Collections;

public class EnergyballPulsate : MonoBehaviour {
	public bool growing;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
		if (growing)
		{
			transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(1.05f, 1.05f), 0.4f);
		}
		else
		{
			transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0.95f, 0.95f), 0.4f);
		}

		//swap growing
		if (growing && transform.localScale.x > 1.045f)
		{
			growing = false;
		}
		if (!growing && transform.localScale.x < 0.955f)
		{
			growing = true;
		}

	
	}
}
