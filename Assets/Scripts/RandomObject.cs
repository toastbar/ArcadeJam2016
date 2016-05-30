using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomObject : MonoBehaviour {
	public GameObject[] rndObjects;
	// Use this for initialization
	void Start () 
	{
		int rnd = Random.Range (0, rndObjects.Length - 1);
		Object.Instantiate(rndObjects[rnd], transform.position, transform.rotation);
		Object.Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
