using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {
    public int gemId = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            if (player.AddGem(gemId))
            {
                Object.Destroy(gameObject);
            }
        }
    }
}
