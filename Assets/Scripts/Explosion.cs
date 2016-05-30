using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	public float damage=500;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject, 2);
		float x1 = transform.position.x;
		float y1 = transform.position.y;

	Collider2D[] hits = Physics2D.OverlapCircleAll (new Vector2 (x1, y1), 5.0f);
		foreach (Collider2D hit in hits)
		{
			if (hit) {
				Diggable dig = hit.GetComponent<Diggable>();
				if (dig) {
					dig.ApplyHit(damage);
					//break;
				}
			}
	}
	}
}