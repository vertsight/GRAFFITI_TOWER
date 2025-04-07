using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public event Action OnFloorBreaked;

    public LevelGenerator Generator;
    public int CountToBreak;
    [SerializeField] Enemy[] enemies;
    [SerializeField] Graffiti[] graffitis;
    [SerializeField] BreakingObstacle breakingObstacle;
    [SerializeField] Transform breakingObstacleBG;
    [SerializeField] Transform flat_1;
    [SerializeField] Transform flat_2;

    public float totalWidth = 18f;
    public float minHoleWidth = 2f;
    public float maxHoleWidth = 5f;
    public float floorPositionY;
    public void Initialize(LevelGenerator generator, int countToBreak = 0)
    {
        Generator = generator;
        CountToBreak = countToBreak;
        if(CountToBreak <= 0)
        {
            breakingObstacle.Initialize(this);
            GenerateFloor();
        }
        else
        {
            breakingObstacle.Initialize(this, true);
            breakingObstacle.OnBreak += BreakFloor;
            
            breakingObstacle.transform.localPosition = new Vector3(0, floorPositionY, 0);
            breakingObstacle.transform.localScale = new Vector3(18, 1, 1);
            flat_1.transform.localScale = new Vector3(0, 1, 1);
            flat_2.transform.localScale = new Vector3(0, 1, 1);
        }
        Spot[] spots = GetComponentsInChildren<Spot>(true);
        foreach(Spot spot in spots)
        {
            spot.Remove();
        }
        SpawnEnemies();
        SpawnGraffiti();
    }
    private void Update()
    {
        transform.position += new Vector3(0,LevelManager.Instance.RoomsUpSpeed * Time.deltaTime, 0);
    }
    private void BreakFloor()
    {
        CountToBreak--;
        if(CountToBreak <= 0)
        {
            breakingObstacle.Breaking();
            OnFloorBreaked?.Invoke();
        }
    }
    private void SpawnGraffiti()
    {
        for(int i = 0; i < graffitis.Length; i++)
        {
            if(UnityEngine.Random.Range(0, 2) == 0)
            {
                graffitis[i].gameObject.SetActive(true);
                graffitis[i].transform.position = new Vector3(UnityEngine.Random.Range(-4f, 4f), transform.position.y + UnityEngine.Random.Range(0f, 3f));
            }
            else
            {
                graffitis[i].gameObject.SetActive(false);
            }
        }
    }
    private void SpawnEnemies()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            if(UnityEngine.Random.Range(0, i) == 0)
            {
                enemies[i].transform.position = new Vector3(UnityEngine.Random.Range(-8, 8), transform.position.y);
            }
            else
            {
                enemies[i].gameObject.SetActive(false);
            }
        }

        if(Generator.CompletedLevels > 20)
        {
            for(int i = 0; i < LevelManager.Instance.FlyEnemiesSpawned.Count; i++)
            {
                if(!LevelManager.Instance.FlyEnemiesSpawned[i].IsUsing && UnityEngine.Random.Range(0, 8) == 0)
                {
                    LevelManager.Instance.FlyEnemiesSpawned[i].Initialize();
                }
            }
        }
    }
    private void GenerateFloor()
    {
        float holeWidth = UnityEngine.Random.Range(minHoleWidth, maxHoleWidth);
        float holeXPosition = UnityEngine.Random.Range(
            -totalWidth/2 + holeWidth/2, 
            totalWidth/2 - holeWidth/2);
        
        breakingObstacle.transform.localPosition = new Vector3(holeXPosition, floorPositionY, 0);
        // breakingObstacleBG.transform.localPosition = new Vector3(holeXPosition, floorPositionY, 0);
        breakingObstacle.transform.localScale = new Vector3(holeWidth, 1, 1);
        
        float leftFloorWidth = (totalWidth - holeWidth) / 2 + holeXPosition;
        float leftFloorX = (-totalWidth/2) + leftFloorWidth/2;
        
        flat_1.transform.localPosition = new Vector3(leftFloorX, floorPositionY, 0);
        flat_1.transform.localScale = new Vector3(leftFloorWidth, 1, 1);
        
        float rightFloorWidth = totalWidth - holeWidth - leftFloorWidth;
        float rightFloorX = holeXPosition + holeWidth/2 + rightFloorWidth/2;
        
        flat_2.transform.localPosition = new Vector3(rightFloorX, floorPositionY, 0);
        flat_2.transform.localScale = new Vector3(rightFloorWidth, 1, 1);
    }
}
