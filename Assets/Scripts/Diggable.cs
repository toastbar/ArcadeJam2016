using UnityEngine;
using System.Collections;

public class Diggable : MonoBehaviour {

    public float hitPoints;
    public GameObject onDestroyObject;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyHit(float damage)
    {
        hitPoints -= damage;
        //Debug.Log(hitPoints);
        if (hitPoints < 0)
        {
            if (onDestroyObject)
            {
                Object.Instantiate(onDestroyObject, transform.position, transform.rotation);
            }
			//this.GetComponent<SpriteRenderer> ().sprite = null;
			//gameObject.GetComponent<SpriteRenderer> ().sprite = null;
			Object.Destroy(gameObject);

        }
    }
}
