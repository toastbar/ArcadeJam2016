using UnityEngine;
using System.Collections.Generic;

public class TimeBomb : MonoBehaviour {
	public float timer;
	public GameObject explosion;
	float createTime = 0;
	// Use this for initialization
	void Start () {
		createTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
		//gameObject.GetComponent<SpriteRenderer>().sprite=Bomb_3;
		if ((Time.time - createTime) > timer) {
			Object.Instantiate(explosion, transform.position, transform.rotation);
			Destroy (gameObject);
		}

	}
}
