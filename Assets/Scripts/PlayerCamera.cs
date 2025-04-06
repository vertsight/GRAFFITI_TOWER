using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Player player;
    void Update()
    {
        Vector2 plr = player.transform.position;
        if(plr.y < transform.position.y)
        {
            transform.position = new Vector3(0,player.transform.position.y,-10);
        }
    }
}
