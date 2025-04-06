using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Level level;
    public Level firstLevel;
    public int CompletedLevels;
    [SerializeField] Transform levelsParent;
    public List<Level> usedLevels = new List<Level>();
    private Level lastLevel;
    private void Start()
    {
        StartGenerate();
    }
    public void StartGenerate()
    {
        lastLevel = firstLevel;
        lastLevel.Initialize(this, 1);

        GenerateLevel();
        GenerateLevel();
        GenerateLevel();
    }
    public void GenerateLevel()
    {
        Level i;
        if(usedLevels.Count <= 15)
        {
            i = Instantiate(level, levelsParent);
        }
        else
        {
            i = usedLevels[0];
            usedLevels.RemoveAt(0);
        }
        usedLevels.Add(i);
        if(lastLevel)
        {
            i.transform.position = lastLevel.transform.position - new Vector3(0, 8);
        }
        CompletedLevels++;
        lastLevel = i;
        if(CompletedLevels % 5 == 0)
        {
            lastLevel.Initialize(this, UnityEngine.Random.Range(2, 5));
            GenerateLevel();
        }
        else
        {
            lastLevel.Initialize(this);
        }
    }
}