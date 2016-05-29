using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMaker : MonoBehaviour {
    public List<GameObject> tileObjects;
    public TextAsset mapFile;
    public int tileWidth;
    public int tileHeight;
    public int mapWidth;
    public int mapHeight;

    private GameObject[][] _map;

	// Use this for initialization
	void Start () {
        string[] lines = mapFile.text.Split('\n');

        Debug.Log(string.Format("{0} lines read", lines.Length));

        float originY = (mapHeight - 0.5f) / 2 * tileHeight;
        float originX = (mapWidth - 0.5f) / 2 * -tileWidth;
  
        _map = new GameObject[mapHeight][];
        for (int ty = 0; ty < mapHeight; ++ty) {
            _map[ty] = new GameObject[mapWidth];

            string line = lines[ty];

            for (int tx = 0; tx < mapWidth; ++tx) {
                int tileId = 0;
                switch (line[tx])
                {
                    case '#': tileId = 1;
                        break;
                }

                GameObject tileProto = tileObjects[tileId];

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
	
	}
}
