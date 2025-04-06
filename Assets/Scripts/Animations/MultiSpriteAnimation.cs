using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpriteAnimation
{
    public string name;
    public List<Sprite> frames;
    public float frameDuration = 0.2f;
    public bool loop = true;
    [HideInInspector] public Coroutine animationCoroutine;
}

public class MultiSpriteAnimation : MonoBehaviour
{
    [Header("Settings")]
    public List<SpriteAnimation> animations;
    public string defaultAnimation;
    
    private SpriteRenderer _renderer;
    private Dictionary<string, SpriteAnimation> _animationDictionary;
    private SpriteAnimation _currentAnimation;
    private Coroutine _currentCoroutine;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animationDictionary = new Dictionary<string, SpriteAnimation>();
        
        foreach (var anim in animations)
        {
            _animationDictionary.Add(anim.name, anim);
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(defaultAnimation))
        {
            PlayAnimation(defaultAnimation);
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (_animationDictionary.TryGetValue(animationName, out SpriteAnimation animation))
        {
            
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }

            
            _currentAnimation = animation;
            _currentCoroutine = StartCoroutine(PlayAnimationCoroutine(animation));
        }
    }

    private IEnumerator PlayAnimationCoroutine(SpriteAnimation animation)
    {
        int currentFrame = 0;
        
        do
        {
            
            if (animation.frames.Count > 0)
            {
                _renderer.sprite = animation.frames[currentFrame];
            }

            
            yield return new WaitForSeconds(animation.frameDuration);

            
            currentFrame = (currentFrame + 1) % animation.frames.Count;

            
            if (!animation.loop && currentFrame == 0)
            {
                break;
            }

        } while (true);

        
        if (!string.IsNullOrEmpty(defaultAnimation) && animation.name != defaultAnimation)
        {
            PlayAnimation(defaultAnimation);
        }
    }

    public void StopAnimation()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    void OnDisable()
    {
        StopAnimation();
    }

    
    public void AddAnimation(string name, List<Sprite> frames, float duration, bool loop)
    {
        if (!_animationDictionary.ContainsKey(name))
        {
            var newAnim = new SpriteAnimation
            {
                name = name,
                frames = frames,
                frameDuration = duration,
                loop = loop
            };
            
            animations.Add(newAnim);
            _animationDictionary.Add(name, newAnim);
        }
    }
}