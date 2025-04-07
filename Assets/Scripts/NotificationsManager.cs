using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance;

    public event Action OnEnemyDead;

    public Sprite[] pointsNotifications;
    public Sprite[] enemiesNotifications;

    public List<Notification> freeNotifications;
    public GameObject notification;

    private void Awake()
    {
        Instance = this;
    }
    public void SpawnNotification(Vector3 spawnPos, int type)
    {
        Sprite sprite = type == 0 ? pointsNotifications[UnityEngine.Random.Range(0, pointsNotifications.Length)] :
            enemiesNotifications[UnityEngine.Random.Range(0, enemiesNotifications.Length)];

        if(type != 0) OnEnemyDead?.Invoke();
        if(freeNotifications.Count == 0)
        {
            var i = Instantiate(notification);
            freeNotifications.Add(i.GetComponent<Notification>());
        }
        freeNotifications[0].Initialize(sprite, spawnPos);
        freeNotifications.RemoveAt(0);
    }
}