using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CountdownSequence : MonoBehaviour
{
    public TextMeshProUGUI[] Numbers;
    public Ease _inEaseType;

    public void StartCountdown(TweenCallback onSequenceComplete)
    {
        Sequence seq = DOTween.Sequence();
        foreach (TextMeshProUGUI text in Numbers)
        {
            seq.Append(text.transform.DOScale(2, 1).SetEase(_inEaseType).OnComplete(() => {
                text.DOColor(Color.clear, 0.5f);
                text.transform.DOLocalMoveY(-600, 0.5f);
                text.transform.DOScale(0, 0.5f).OnComplete(() => text.gameObject.SetActive(false));
                }));
        }
        seq.OnComplete(onSequenceComplete);
    }
}
