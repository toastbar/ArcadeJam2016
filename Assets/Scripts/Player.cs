using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public int playerNum;
    public float speed = 5.0f;
    public float digDistance = 1.0f;
    public float digStrength = 100.0f;
    public float controllerRotation = 0.0f;
    public float[] gemCount = new float[5];
    public Color color;

    private Vector3 lastDirection;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private Vector3 secretTreasure;

    private string xAxisName;
    private string yAxisName;
    private string digButtonName;
    private string[][] matrixButtonNames;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        lastDirection = new Vector3(1.0f, 0.0f, 0.0f);

        xAxisName = string.Format("player{0}_x", playerNum);
        yAxisName = string.Format("player{0}_y", playerNum);
        digButtonName = string.Format("player{0}_dig", playerNum);

        matrixButtonNames = new string[4][];
        for (int matrix_row = 0; matrix_row < 4; ++matrix_row)
        {
            matrixButtonNames[matrix_row] = new string[4];
            for (int matrix_col = 0; matrix_col < 4; ++matrix_col)
            {
                matrixButtonNames[matrix_row][matrix_col] = string.Format("player{0}_matrix{1}{2}", playerNum, matrix_row + 1, matrix_col + 1);
            }
        }


        secretTreasure = new Vector3(Random.Range(-30, 30), Random.Range(-18, 18), 0);

        UpdateGemLeds();
        UpdateButtonColors();
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

        bool gemsUpdated = false;
        for (int matrix_row = 0; matrix_row < 4; ++matrix_row)
        {
            for (int matrix_col = 0; matrix_col < 4; ++matrix_col)
            {
                if (Input.GetButtonDown(matrixButtonNames[matrix_row][matrix_col]))
                {   
                    if (matrix_row == 3)
                    {
                        // Wild gems!
                        gemCount[4] = matrix_col + 1;
                        gemsUpdated = true;
                    }
                    else if (matrix_col == 0)
                    {
                        // Blue gems
                        gemCount[0] = matrix_row + 1;
                        gemsUpdated = true;
                    }
                    else if (matrix_col == 1)
                    {
                        // Red gems
                        gemCount[1] = matrix_row + 1;
                        gemsUpdated = true;
                    }
                    else if (matrix_col == 2)
                    {
                        // Yellow gems
                        gemCount[2] = matrix_row + 1;
                        gemsUpdated = true;
                    }
                    else if (matrix_col == 3)
                    {
                        // Green gems
                        gemCount[3] = matrix_row + 1;
                        gemsUpdated = true;
                    }
                }
            }
        }

        if (gemsUpdated)
            UpdateGemLeds();

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

        UpdateSecretLocator();
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

        UpdateGemLeds();
        return true;
    }

    private void UpdateGemLeds()
    {
        KeypadController pad = GetComponent<KeypadController>();
        if (!pad)
            return;

        bool[] leds = new bool[16];

        leds[0] = gemCount[0] > 0;
        leds[1] = gemCount[1] > 0;
        leds[2] = gemCount[2] > 0;
        leds[3] = gemCount[3] > 0;
        leds[4] = gemCount[0] > 1;
        leds[5] = gemCount[1] > 1;
        leds[6] = gemCount[2] > 1;
        leds[7] = gemCount[3] > 1;
        leds[8] = gemCount[0] > 2;
        leds[9] = gemCount[1] > 2;
        leds[10] = gemCount[2] > 2;
        leds[11] = gemCount[3] > 2;
        leds[12] = gemCount[4] > 0;
        leds[13] = gemCount[4] > 1;
        leds[14] = gemCount[4] > 2;
        leds[15] = gemCount[4] > 3;

        pad.SetLedState(leds);
    }

    private void UpdateSecretLocator()
    {
        Vector3 diff = secretTreasure - transform.position;
        diff = Quaternion.Euler(0, 0, -controllerRotation) * diff;

        float left = 0.0f;
        float right = 0.0f;
        float horiz = 0.0f;
        if (Mathf.Abs(diff.x) < 10.0f)
        {
            float intensity = diff.x / 10.0f;
            if (intensity < 0)
            {
                left = 1.0f  + intensity;

                if (left > 0.9)
                    right = left;
                else
                    horiz = left;
            }
            else
            {
                right = 1.0f - intensity;

                if (right > 0.9)
                    left = right;
                else
                    horiz = right;
            }
        }

        float up = 0.0f;
        float down = 0.0f;
        float vert = 0.0f;
        if (Mathf.Abs(diff.y) < 10.0f)
        {
            float intensity = diff.y / 10.0f;
            if (intensity < 0)
            {
                down = 1.0f + intensity;

                if (down > 0.9)
                    up = down;
                else
                    vert = down;
            }
            else
            {
                up = 1.0f - intensity;

                if (up > 0.9)
                    down = up;
                else
                    vert = up;
            }
        }

        KeypadController pad = GetComponent<KeypadController>();
        if (!pad)
            return;

        pad.SetArrowState(left, horiz, right, up, vert, down);
    }

    private void UpdateButtonColors()
    {
        JoypadController joy = GetComponent<JoypadController>();

        joy.SetColorState(new Color[4] { color, Color.white, Color.red, Color.blue });
    }
}
