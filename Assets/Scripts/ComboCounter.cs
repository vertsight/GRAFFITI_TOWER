using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    public List<ComboColor> ComboColors;
    public Player player;
    public TMP_Text txtCounter;
    public int counter;
    public float timeToResetCounter;
    public float time;

    private Tweener _scaleTweener;

    void OnEnable()
    {
        player.OnBreak += UpdateCount;
    }
    private void Start()
    {
        NotificationsManager.Instance.OnEnemyDead += UpdateCount;
    }
    private void OnDestroy()
    {
        player.OnBreak -= UpdateCount;
        NotificationsManager.Instance.OnEnemyDead -= UpdateCount;
    }
    private void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            Reset();
        }
    }
    public void UpdateCount()
    {
        counter++;
        time = timeToResetCounter;
        UpdateText();
        DOTween.Kill(this);
        txtCounter.rectTransform.DOScale(txtCounter.rectTransform.localScale.x + counter/10, 0.2f);
        txtCounter.rectTransform.DOScale(1, 2f);

        if(counter >= 25)
        {
            NotificationsManager.Instance.SpawnNotification(player.transform.position, 0);
        }
    }
    private void Reset()
    {
        time = timeToResetCounter;
        counter = 0;
        DOTween.Kill(this);
        txtCounter.rectTransform.DOScale(1, 0.6f);
        UpdateText();
    }
    private void UpdateText()
    {
        txtCounter.text = $"x{counter}";
    }
}

[Serializable]
public class ComboColor
{
    public int count;
    public Color color;
}