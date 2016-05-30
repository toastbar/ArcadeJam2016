using UnityEngine;
using System.Collections.Generic;

public class TimeBomb : MonoBehaviour {
	public float timer;
	float createTime = 0;
	// Use this for initialization
	void Start () {
		createTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
		//gameObject.GetComponent<SpriteRenderer>().sprite=Bomb_3;
		if ((Time.time - createTime) > timer) {
			Destroy (gameObject);
		}

	}
}
