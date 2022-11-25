using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrainingCursor : MonoBehaviour
{
    public GameObject cursor;
    private Sequence tween;

    // Start is called before the first frame update
    public void ClickAnimation(Vector3 startPoint, Vector3 endPoint)
    {
        tween.Kill();
        cursor.SetActive(true);
        var target = cursor.transform;
        tween = DOTween.Sequence().Append(target.DOLocalMove(endPoint, 1).From(startPoint))
            .AppendInterval(0.5f).Append(target.DOScale(11f, 0.5f))
            .Append(target.DOScale(15, 0.5f))
            .SetLoops(-1);
    }

    public void DragAnimation(Vector3 startPoint, Vector3 pickPoint, Vector3 endPoint)
    {
        tween.Kill();
        cursor.SetActive(true);
        var target = cursor.transform;
        tween = DOTween.Sequence().Append(target.DOLocalMove(pickPoint, 1).From(startPoint))
            .AppendInterval(0.5f).Append(target.DOScale(11f, 0.5f))
            .Append(target.DOLocalMove(endPoint, 2))
            .Append(target.DOScale(15, 0.5f))
            .SetLoops(-1);
    }

    public void Stop()
    {
        cursor.SetActive(false);
        tween.Pause();
    }

    public void Restart()
    {
        tween.Restart();
        cursor.SetActive(true);
    }

    public void Kill()
    {
        cursor.SetActive(false);
        tween.Kill();
    }
}