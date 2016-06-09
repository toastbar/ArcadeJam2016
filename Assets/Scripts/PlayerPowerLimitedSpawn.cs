using UnityEngine;
using System.Collections;
using System;

public class PlayerPowerLimitedSpawn : PlayerPower {

    public GameObject thingToSpawn;
    public int numSpawnsRemaining = 3;
    public Color controllerColor;
    public float spawnDistance = 1.0f;

    private Player player;

    public PlayerPowerLimitedSpawn(GameObject _thingToSpawn, int numSpawns, Color _controllerColor)
    {
        thingToSpawn = _thingToSpawn;
        numSpawnsRemaining = numSpawns;
        controllerColor = _controllerColor;
    }

	// Use this for initialization
	void Start()
    {
	}

    public override void Activate(Player _player)
    {
        player = _player;
    }

    public override void OnButtonDown()
    {
        if (numSpawnsRemaining > 0)
        {
            Vector3 spawnLocation = player.transform.position + player.GetLastDireciton() * spawnDistance;
            Instantiate(thingToSpawn, spawnLocation, Quaternion.identity);
            numSpawnsRemaining--;
        }

        if (numSpawnsRemaining <= 0)
        {
            player.DeactivatePower(this);
            DestroyObject(gameObject);
        }
    }

    public override void OnButtonUp()
    {
    }

    public override Color GetControllerColor()
    {
        return controllerColor;
    }
}
