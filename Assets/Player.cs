using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed;
    public float digDistance;
    public float digStrength;

    private Rigidbody2D body;
    private SpriteRenderer sprite;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            move += new Vector3(0.0f, 1.0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            move += new Vector3(0.0f, -1.0f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move += new Vector3(-1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            move += new Vector3(1.0f, 0.0f);
        }

        if (move.sqrMagnitude > Mathf.Epsilon)
        {
            move.Normalize();
            body.MovePosition(transform.position + move * speed * Time.deltaTime);
            sprite.sortingOrder = 100 - (int)transform.position.y;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float x1 = transform.position.x;
                float y1 = transform.position.y;

                RaycastHit2D[] hits = Physics2D.CircleCastAll(new Vector2(x1, y1), 0.3f, new Vector2(move.x, move.y), digDistance);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider)
                    {

                        Diggable dig = hit.collider.GetComponent<Diggable>();
                        if (dig)
                        {
                            Debug.Log(hit.collider);
                            dig.ApplyHit(digStrength);
                        }
                    }
                }
            }
        }
    }
}
