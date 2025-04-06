using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public Color[] possibleColors;

    [Header("Scale Settings")]
    public float startScale = 0.5f;
    public float maxScale = 1.5f;
    public float endScale = 0.1f;
    public float scaleUpDuration = 0.3f;
    public float scaleDownDuration = 0.5f;

    [Header("Movement Settings")]
    public float minForce = 5f;
    public float maxForce = 10f;
    public float minAngle = 30f;
    public float maxAngle = 60f;
    public float flightDuration = 1f;

    private SpriteRenderer spriteRenderer;
    public void Initialize(Sprite sprite, Vector3 pos)
    {
        gameObject.SetActive(true);
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = possibleColors[Random.Range(0, possibleColors.Length)];
        transform.position = pos;

        StartCoroutine(AnimationSequence());
    }
    private IEnumerator AnimationSequence()
    {
        transform.localScale = Vector3.one * startScale;
        yield return transform.DOScale(maxScale, scaleUpDuration)
            .SetEase(Ease.OutBack)
            .WaitForCompletion();

        Vector2 randomDirection = GetRandomDirection();
        float force = Random.Range(minForce, maxForce);
        
        yield return transform.DOMove(
            (Vector2)transform.position + randomDirection * force, 
            flightDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        yield return transform.DOScale(endScale, scaleDownDuration)
            .SetEase(Ease.InBack)
            .WaitForCompletion();

        gameObject.SetActive(false);
    }
    private Vector2 GetRandomDirection()
    {
        int direction = Random.Range(0, 2) * 2 - 1; // -1 или 1
        float angle = Random.Range(minAngle, maxAngle);
        
        return new Vector2(
            direction * Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;
    }
    private void OnDisable()
    {
        DOTween.Kill(this);
        NotificationsManager.Instance.freeNotifications.Add(this);
    }
}