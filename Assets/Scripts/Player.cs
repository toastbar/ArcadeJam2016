using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public int playerNum;
    public float speed = 5.0f;
    public float digDistance = 1.0f;
    public float digStrength = 100.0f;
    public float controllerRotation = 0.0f;
    public float[] gemCount = new float[5];
    public GameObject buttonVis;
    public Color color;
    public GameObject[] redPowers;
    public GameObject[] bluePowers;
    public GameObject[] yellowPowers;
    public GameObject[] greenPowers;
    public GameObject[] wildPowers;

    private Vector3 lastDirection;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private Vector3 secretTreasure;

    private string xAxisName;
    private string yAxisName;
    private string digButtonName;
    private string power0ButtonName;
    private string power1ButtonName;
    private string[][] matrixButtonNames;
    private PlayerPower[] activePowers = new PlayerPower[2];

    private SpriteRenderer controllerJoySprite;
    private SpriteRenderer controllerButton1Sprite;
    private SpriteRenderer controllerButton2Sprite;
    private SpriteRenderer controllerButton3Sprite;
    private SpriteRenderer[][] matrixButtonSprites;

    private enum PowerColor
    {
        Blue,
        Red,
        Yellow,
        Green,
        Wild
    };

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        lastDirection = new Vector3(1.0f, 0.0f, 0.0f);

        xAxisName = string.Format("player{0}_x", playerNum);
        yAxisName = string.Format("player{0}_y", playerNum);
        digButtonName = string.Format("player{0}_dig", playerNum);
        power0ButtonName = string.Format("player{0}_power0", playerNum);
        power1ButtonName = string.Format("player{0}_power1", playerNum);

        matrixButtonNames = new string[4][];
        for (int matrix_row = 0; matrix_row < 4; ++matrix_row)
        {
            matrixButtonNames[matrix_row] = new string[4];
            for (int matrix_col = 0; matrix_col < 4; ++matrix_col)
            {
                matrixButtonNames[matrix_row][matrix_col] = string.Format("player{0}_matrix{1}{2}", playerNum, matrix_row + 1, matrix_col + 1);
            }
        }

        SetupButtonSprites();

        secretTreasure = new Vector3(Random.Range(-30, 30), Random.Range(-18, 18), 0);

        UpdateGemLeds();
        UpdateButtonColors();

        ActivatePower(PowerColor.Red, 0);
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

        if (Input.GetButtonDown(power0ButtonName))
        {
            if (activePowers[0] != null)
            {
                activePowers[0].OnButtonDown();
            }
        }
        if (Input.GetButtonUp(power0ButtonName))
        {
            if (activePowers[0] != null)
            {
                activePowers[0].OnButtonUp();
            }
        }

        if (Input.GetButtonDown(power1ButtonName))
        {
            if (activePowers[1] != null)
            {
                activePowers[1].OnButtonDown();
            }
        }
        if (Input.GetButtonUp(power1ButtonName))
        {
            if (activePowers[1] != null)
            {
                activePowers[1].OnButtonUp();
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
                        if (gemCount[4] > matrix_col)
                        {
                            if (ActivatePower(PowerColor.Wild, matrix_col))
                            {
                                gemCount[4]--;
                                gemsUpdated = true;
                            }
                        }
                    }
                    else if (matrix_col == 0)
                    {
                        // Blue gems
                        if (gemCount[0] > matrix_row)
                        {
                            if (ActivatePower(PowerColor.Blue, matrix_row))
                            {
                                gemCount[0]--;
                                gemsUpdated = true;
                            }
                        }
                    }
                    else if (matrix_col == 1)
                    {
                        // Red gems
                        if (gemCount[1] > matrix_row)
                        {
                            if (ActivatePower(PowerColor.Red, matrix_row))
                            {
                                gemCount[1]--;
                                gemsUpdated = true;
                            }
                        }
                    }
                    else if (matrix_col == 2)
                    {
                        // Yellow gems
                        if (gemCount[2] > matrix_row)
                        {
                            if (ActivatePower(PowerColor.Yellow, matrix_row))
                            {
                                gemCount[2]--;
                                gemsUpdated = true;
                            }
                        }
                    }
                    else if (matrix_col == 3)
                    {
                        // Green gems
                        if (gemCount[3] > matrix_row)
                        {
                            if (ActivatePower(PowerColor.Green, matrix_row))
                            {
                                gemCount[3]--;
                                gemsUpdated = true;
                            }
                        }
                    }
                }
            }
        }

        if (gemsUpdated)
            UpdateGemLeds();

        UpdateButtonColors();
    }

    void FixedUpdate()
    {
        Animator anim = GetComponent<Animator>();
 
        float moveX = Input.GetAxis(xAxisName);
        float moveY = -Input.GetAxis(yAxisName);

        Vector3 move = Quaternion.Euler(0, 0, controllerRotation) * new Vector3(moveX, moveY, 0);

        if (move.sqrMagnitude >= 0.25)
        {
            body.velocity = new Vector2(0.0f, 0.0f);
            move.Normalize();
            lastDirection = move;

            float moveAngle = Mathf.Atan2(move.y, move.x);
            Debug.Log(moveAngle);
            if (moveAngle > 0)
                if (moveAngle < Mathf.PI / 4)
                    anim.SetInteger("moveDir", 3);
                else if (moveAngle < Mathf.PI * 0.75f)
                    anim.SetInteger("moveDir", 2);
                else
                    anim.SetInteger("moveDir", 1);
            else if (moveAngle > -Mathf.PI / 4)
                anim.SetInteger("moveDir", 3);
            else if (moveAngle > -Mathf.PI * 0.75f)
                anim.SetInteger("moveDir", 4);
            else
                anim.SetInteger("moveDir", 1);
            anim.SetFloat("speed", 1.0f);

            body.MovePosition(transform.position + move * speed * Time.deltaTime);
            sprite.sortingOrder = 100 - (int)transform.position.y;
        }
        else
        {
            anim.SetInteger("moveDir", 0);
            anim.SetFloat("speed", 1.0f);
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
        if (matrixButtonSprites != null)
        {
            for (int row = 0; row < 4; ++row)
            {
                for (int col = 0; col < 4; ++col)
                {
                    Color c = Color.black;
                    if (gemCount[col] > row)
                    {
                        if (col == 0) c = Color.blue;
                        if (col == 1) c = Color.red;
                        if (col == 2) c = Color.yellow;
                        if (col == 3) c = Color.green;
                    }

                    if (row == 3 && gemCount[4] > col)
                        c = Color.white;

                    var sprite = matrixButtonSprites[row][col];
                    if (sprite)
                        sprite.color = c;
                }
            }
        }
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

        Color power0Color = Color.black;
        if (activePowers[0] != null)
        {
            power0Color = activePowers[0].GetControllerColor();
        }

        Color power1Color = Color.black;
        if (activePowers[1] != null)
        {
            power1Color = activePowers[1].GetControllerColor();
        }

        joy.SetColorState(new Color[4] { color, Color.white, power0Color, power1Color });
        if (controllerJoySprite)
            controllerJoySprite.color = color;
        if (controllerButton1Sprite)
            controllerButton1Sprite.color = Color.white;
        if (controllerButton2Sprite)
            controllerButton2Sprite.color = power0Color;
        if (controllerButton3Sprite)
            controllerButton3Sprite.color = power1Color;
    }

    private bool ActivatePower(PowerColor color, int index)
    {
        int powerIndex = -1;

        for (int i = 0; i < activePowers.Length; ++i)
        {
            if (activePowers[i] == null)
            {
                powerIndex = i;
                break;
            }
        }

        if (powerIndex < 0)
            return false;

        switch (color)
        {
            case PowerColor.Red:
                if (index < redPowers.Length)
                {
                    var powerObj = Object.Instantiate(redPowers[index]);
                    var power = powerObj.GetComponent<PlayerPower>();
                    power.Activate(this);
                    activePowers[powerIndex] = power;
                    return true;
                }
                break;

            case PowerColor.Blue:
                if (index < bluePowers.Length)
                {
                    var powerObj = Object.Instantiate(bluePowers[index]);
                    var power = powerObj.GetComponent<PlayerPower>();
                    power.Activate(this);
                    activePowers[powerIndex] = power;
                    return true;
                }
                break;

            case PowerColor.Yellow:
                if (index < yellowPowers.Length)
                {
                    var powerObj = Object.Instantiate(yellowPowers[index]);
                    var power = powerObj.GetComponent<PlayerPower>();
                    power.Activate(this);
                    activePowers[powerIndex] = power;
                    return true;
                }
                break;

            case PowerColor.Green:
                if (index < greenPowers.Length)
                {
                    var powerObj = Object.Instantiate(greenPowers[index]);
                    var power = powerObj.GetComponent<PlayerPower>();
                    power.Activate(this);
                    activePowers[powerIndex] = power;
                    return true;
                }
                break;

            case PowerColor.Wild:
                if (index < wildPowers.Length)
                {
                    var powerObj = Object.Instantiate(wildPowers[index]);
                    var power = powerObj.GetComponent<PlayerPower>();
                    power.Activate(this);
                    activePowers[powerIndex] = power;
                    return true;
                }
                break;
        }

        return false;
    }

    public void DeactivatePower(PlayerPower power)
    {
        for (int i = 0; i < activePowers.Length; ++i)
        {
            if (activePowers[i] == power)
            {
                activePowers[i] = null;
                break;
            }
        }
    }

    public Vector3 GetLastDireciton()
    {
        return lastDirection;
    }

    private void SetupButtonSprites()
    {
        matrixButtonSprites = new SpriteRenderer[4][];
        matrixButtonSprites[0] = new SpriteRenderer[4];
        matrixButtonSprites[1] = new SpriteRenderer[4];
        matrixButtonSprites[2] = new SpriteRenderer[4];
        matrixButtonSprites[3] = new SpriteRenderer[4];

        if (buttonVis)
        {
            buttonVis.transform.position = new Vector3(-29, -18, -20);
                var visObj = Instantiate(buttonVis);
            foreach (var child in visObj.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                if (child.name == "Joy")
                    controllerJoySprite = child;
                if (child.name == "Button1")
                    controllerButton1Sprite = child;
                if (child.name == "Button2")
                    controllerButton2Sprite = child;
                if (child.name == "Button3")
                    controllerButton3Sprite = child;
                if (child.name.StartsWith("m"))
                {
                    int col = int.Parse(child.name[1].ToString());
                    int row = int.Parse(child.name[2].ToString());
                    matrixButtonSprites[row][col] = child;
                }
            }
        }
    }
}
