using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MapMakerImage : MonoBehaviour {
    public Texture2D[] tileImages;
    public Color[] tileColors;
    public GameObject[] tileObjects;
    public float tileWidth = 1;
    public float tileHeight = 1;

    // Use this for initialization
    void Start()
    {
        var tileDict = new Dictionary<Color, GameObject>();

        for (int i = 0; i < tileColors.Length; ++i)
        {
            tileDict[tileColors[i]] = tileObjects[i];
        }

        foreach (var image in tileImages)
        {
            int mapWidth = image.width;
            int mapHeight = image.height;

            float originY = (float)mapHeight / 2.0f * -(float)tileHeight + (float)tileHeight * 0.5f;
            float originX = (float)mapWidth / 2.0f * -(float)tileWidth + (float)tileWidth * 0.5f;

            for (int ty = 0; ty < mapHeight; ++ty)
            {
                for (int tx = 0; tx < mapWidth; ++tx)
                {
                    Color c = image.GetPixel(tx, ty);

                    // ignore all fully transparent pixels
                    if (c.a == 0)
                        continue;

                    GameObject tileProto;
                    if (!tileDict.TryGetValue(c, out tileProto))
                    {
                        Debug.Log(string.Format("Color: {0} not found in map maker palatte.", c));
                        continue;
                    }

                    if (tileProto)
                    {
                        float x = originX + tx * (float)tileWidth;
                        float y = originY + ty * (float)tileHeight;

                        GameObject newTile = Object.Instantiate(tileProto, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                        newTile.GetComponent<SpriteRenderer>().sortingOrder = 100 - ty;

                    }
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("main");
        }
    }
}
