using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MapMaker : MonoBehaviour {

    public char[] tileSymbols;
    public GameObject[] tileObjects;
    public TextAsset mapFile;
    public int tileWidth;
    public int tileHeight;
    public int mapWidth;
    public int mapHeight;

    private GameObject[][] _map;

	// Use this for initialization
	void Start () {
        var tileDict = new Dictionary<char, GameObject>();

        for (int i = 0; i < tileSymbols.Length; ++i)
        {
            tileDict[tileSymbols[i]] = tileObjects[i];
        }

        string[] lines = mapFile.text.Split('\n');

        Debug.Log(string.Format("{0} lines read", lines.Length));

        float originY = (float)mapHeight / 2.0f * (float)tileHeight - (float)tileHeight * 0.5f;
        float originX = (float)mapWidth / 2.0f * -(float)tileWidth + (float)tileWidth * 0.5f;
  
        _map = new GameObject[mapHeight][];
        for (int ty = 0; ty < mapHeight; ++ty)
        {
            _map[ty] = new GameObject[mapWidth];

            string line = lines[ty];

            for (int tx = 0; tx < mapWidth; ++tx)
            {
                GameObject tileProto = tileDict[line[tx]];

                if (tileProto)
                {
                    float x = originX + tx * (float)tileWidth;
                    float y = originY - ty * (float)tileHeight;

                    GameObject newTile = Object.Instantiate(tileProto, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    newTile.GetComponent<SpriteRenderer>().sortingOrder = 100 - ty;
                    _map[ty][tx] = newTile;
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
