using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class CountdownSequence : MonoBehaviour
{
    public TextMeshProUGUI[] Numbers;
    public AudioClip[] Clips;
    public Ease _inEaseType;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void StartCountdown(TweenCallback onSequenceComplete)
    {
        int count = 0;
        Sequence seq = DOTween.Sequence();
        foreach (TextMeshProUGUI text in Numbers)
        {
            seq.Append(text.transform.DOScale(2, 1).SetEase(_inEaseType).OnComplete(() => {
                text.DOColor(Color.clear, 0.5f);
                text.transform.DOLocalMoveY(-600, 0.5f);
                text.transform.DOScale(0, 0.5f).OnComplete(() => text.gameObject.SetActive(false));
                }));
            int tempCount = count++;
            seq.InsertCallback(tempCount + 1 * (1 - 0.25f), () => _audioSource.PlayOneShot(Clips[tempCount]));
        }
        seq.OnComplete(onSequenceComplete);
    }
}
