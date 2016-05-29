using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public int playerNum;
    public float speed;
    public float digDistance;
    public float digStrength;
    public float controllerRotation;

    private Vector3 lastDirection;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private string xAxisName;
    private string yAxisName;
    private string digButtonName;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        lastDirection = new Vector3(1.0f, 0.0f, 0.0f);

        xAxisName = string.Format("player{0}_x", playerNum);
        yAxisName = string.Format("player{0}_y", playerNum);
        digButtonName = string.Format("player{0}_dig", playerNum);
    }

    void Update()
    {
        Debug.DrawRay(transform.position, lastDirection, Color.blue);

        bool digPressed = Input.GetButtonDown(digButtonName);

        if (digPressed)
        {
            float x1 = transform.position.x;
            float y1 = transform.position.y;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(new Vector2(x1, y1), 0.3f, new Vector2(lastDirection.x, lastDirection.y), digDistance);

            Debug.DrawRay(new Vector3(x1, y1, 0), lastDirection, Color.red, 5.0f);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider)
                {
                    Diggable dig = hit.collider.GetComponent<Diggable>();
                    if (dig)
                    {
                        Debug.Log(hit.collider);
                        dig.ApplyHit(digStrength);
                        break;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis(xAxisName);
        float moveY = -Input.GetAxis(yAxisName);

        Vector3 move = Quaternion.Euler(0, 0, controllerRotation) * new Vector3(moveX, moveY, 0);

        Debug.Log(move);

        if (move.sqrMagnitude >= 0.25)
        {
            move.Normalize();
            lastDirection = move;

            body.MovePosition(transform.position + move * speed * Time.deltaTime);
            sprite.sortingOrder = 100 - (int)transform.position.y;
        }
    }
}
