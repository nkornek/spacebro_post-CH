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
			transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(1.1f, 1.1f), 0.5f);
		}
		else
		{
			transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0.9f, 0.9f), 0.5f);
		}

		//swap growing
		if (growing && transform.localScale.x > 1.095f)
		{
			growing = false;
		}
		if (!growing && transform.localScale.x < 0.95f)
		{
			growing = true;
		}

	
	}
}
