using UnityEngine;
using System.Collections;

public abstract class PlayerPower : MonoBehaviour
{
    public abstract void Activate(Player player);

    public abstract Color GetControllerColor();

    public abstract void OnButtonDown();

    public abstract void OnButtonUp();
}
