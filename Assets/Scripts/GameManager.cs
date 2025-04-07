using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] Player player;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] Level firstLevel;
    [SerializeField] LevelGenerator levelGenerator;

    [Header("UI")]
    [SerializeField] GameObject pnlStart;
    [SerializeField] Image jumpH;
    [SerializeField] Image hitH;
    [SerializeField] Image txtLogo;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        firstLevel.OnFloorBreaked += StartGame;
    }
    private void OnDisable()
    {
        firstLevel.OnFloorBreaked -= StartGame;
    }

    public void StartGame()
    {
        LevelManager.Instance.RoomsUpSpeed = 1f;
        SoundManager.Instance.Music.Play();
        SoundManager.Instance.Music.DOFade(0.8f, 0.4f);
        SoundManager.Instance.Noise.Stop();

        txtLogo.rectTransform.DOAnchorPosY(-495, 0.7f);
        jumpH.rectTransform.DOAnchorPos(new Vector2(-229f, 127f), 0.7f);
        hitH.rectTransform.DOAnchorPosX(-229f, 0.7f).onComplete = () => 
        {
            pnlStart.gameObject.SetActive(false);
        };
    }
    public void RestartGame()
    {
        LevelManager.Instance.RoomsUpSpeed = 0f;
        pnlStart.gameObject.SetActive(true);
        txtLogo.rectTransform.DOAnchorPosY(26, 0.7f);
        jumpH.rectTransform.DOAnchorPos(new Vector2(215f, -120f), 0.7f);
        hitH.rectTransform.DOAnchorPosX(215f, 0.7f);


        levelGenerator.CompletedLevels = 0;
        firstLevel.transform.position = levelGenerator.usedLevels[0].transform.position + new Vector3(0, 8);
        player.transform.position = firstLevel.transform.position + Vector3.up;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        foreach(var i in LevelManager.Instance.FlyEnemiesSpawned) i.SetUsed();
        playerCamera.transform.DOMoveY(player.transform.position.y, 1.2f).onComplete = () => 
        {
            List<Level> levels = FindObjectsByType<Level>(FindObjectsSortMode.None).ToList();
            levels.Remove(firstLevel);
            levelGenerator.usedLevels.Clear();
            for(int i = 0; i < levels.Count; i++) 
            {
                Destroy(levels[i].gameObject);
            }
            levelGenerator.StartGenerate();
        };
        SoundManager.Instance.Music.DOFade(0, 0.7f).onComplete = () => SoundManager.Instance.Music.Stop();
        SoundManager.Instance.Noise.Play();
    }
}