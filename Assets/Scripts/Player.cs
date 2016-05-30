using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public int playerNum;
    public float speed = 5.0f;
    public float digDistance = 1.0f;
    public float digStrength = 100.0f;
    public float controllerRotation = 0.0f;
    public float[] gemCount = new float[5];

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

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider)
                {
                    Diggable dig = hit.collider.GetComponent<Diggable>();
                    if (dig)
                    {
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

        if (move.sqrMagnitude >= 0.25)
        {
            body.velocity = new Vector2(0.0f, 0.0f);
            move.Normalize();
            lastDirection = move;

            body.MovePosition(transform.position + move * speed * Time.deltaTime);
            sprite.sortingOrder = 100 - (int)transform.position.y;
        }
    }

    public bool AddGem(int gemId)
    {
        if (gemId < 0 || gemId >= gemCount.Length)
            return false;

        if (gemId < 4 && gemCount[gemId] >= 3)
            return false;
        else if (gemCount[gemId] >= 4)
            return false;

        gemCount[gemId]++;
        return true;
    }
}
