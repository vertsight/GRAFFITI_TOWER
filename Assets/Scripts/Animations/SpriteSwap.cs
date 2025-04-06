using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwap : MonoBehaviour
{
	public AnimationFrame[] AnimationFrames;
	public bool isLoop;
	private int frame;
	private SpriteRenderer spr;
	private Image img;
	private void Start()
	{
		frame = 0;
		spr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
	}
	private void OnEnable()
	{
		StartCoroutine(Animation());
	}
    private void OnDisable()
    {
		StopCoroutine(Animation());   
    }
    private IEnumerator Animation()
	{
		yield return new WaitForSeconds(AnimationFrames[frame].offset);
		if(spr != null) spr.sprite = AnimationFrames[frame].sprite;
		if(img != null) img.sprite = AnimationFrames[frame].sprite;
		if(frame + 1 < AnimationFrames.Length)
		{
			frame++;
			StartCoroutine(Animation());
		}
		else if(isLoop)
		{
			frame = 0;
			StartCoroutine(Animation());
		}
	}
}
[Serializable]
public class AnimationFrame
{
	public Sprite sprite;
	public float offset;
}