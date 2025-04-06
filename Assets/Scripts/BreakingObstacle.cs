using System;
using DG.Tweening;
using UnityEngine;

public class BreakingObstacle : MonoBehaviour
{
    public event Action OnBreak;
    private bool breaked;
    private Level level;
    private bool IsCountBreaking;
    public void Initialize(Level level, bool IsCountBreaking=false)
    {
        gameObject.SetActive(true);
        this.level = level;
        this.IsCountBreaking = IsCountBreaking;
        breaked = false;
    }
    public void Break()
    {
        if(IsCountBreaking)
        {
            OnBreak?.Invoke();
            BreakAnimate();
        }
        else
        {
            if (breaked) return;
            
            breaked = true;
            level.Generator.GenerateLevel();
            Breaking();
        }
    }
    public void Breaking()
    {
        BreakAnimate();
        gameObject.SetActive(false);
    }
    private void BreakAnimate()
    {

    }
}