using UnityEngine;
using System.Collections;

public class CounterAnimationTriggers : MonoBehaviour {

	public ParticleSystem[] rocketParticles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleRockets()
	{
		if (rocketParticles[0].enableEmission)
		{
			rocketParticles[0].enableEmission = false;
			rocketParticles[1].enableEmission = false;
		}
		else
		{
			rocketParticles[0].enableEmission = true;
			rocketParticles[1].enableEmission = true;
		}
	}
}
