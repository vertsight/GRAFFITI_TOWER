using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public float RoomsUpSpeed;
    public List<FlyEnemy> FlyEnemiesSpawned = new List<FlyEnemy>(3);

    private void Awake()
    {
        Instance = this;
    }
}