using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	public float damage = 500.0f;
    public float radius = 5f;
    public float lifetime = 2.0f;

	// Use this for initialization
	void Start () {
        float x1 = transform.position.x;
        float y1 = transform.position.y;

        Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(x1, y1), radius);
        foreach (Collider2D hit in hits)
        {
            Diggable dig = hit.GetComponent<Diggable>();
            if (dig)
            {
                dig.ApplyHit(damage);
            }
        }

        Destroy(gameObject, lifetime);
    }
}