using DG.Tweening;
using UnityEngine;

public class TrainingCursor : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private float defaultScale;
    [SerializeField] private float clickScale;
    [SerializeField] private float speed;
    private Sequence _tween;
    
    public void ClickAnimation(Vector3 startPoint, Vector3 endPoint)
    {
        _tween.Kill();
        cursor.SetActive(true);
        var target = cursor.transform;
        _tween = DOTween.Sequence().Append(target.DOLocalMove(endPoint, 1).From(startPoint))
            .AppendInterval(speed).Append(target.DOScale(clickScale, speed))
            .Append(target.DOScale(defaultScale, speed))
            .SetLoops(-1);
    }

    public void DragAnimation(Vector3 startPoint, Vector3 pickPoint, Vector3 endPoint)
    {
        _tween.Kill();
        cursor.SetActive(true);
        var target = cursor.transform;
        _tween = DOTween.Sequence().Append(target.DOLocalMove(pickPoint, 1).From(startPoint))
            .AppendInterval(speed).Append(target.DOScale(clickScale, speed))
            .Append(target.DOLocalMove(endPoint, speed))
            .Append(target.DOScale(defaultScale, speed))
            .SetLoops(-1);
    }

    public void Stop()
    {
        cursor.SetActive(false);
        _tween.Pause();
    }

    public void Restart()
    {
        _tween.Restart();
        cursor.SetActive(true);
    }

    public void Kill()
    {
        cursor.SetActive(false);
        _tween.Kill();
    }
}