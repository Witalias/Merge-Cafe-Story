using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCursor : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private Image _image;
    [SerializeField] private float defaultScale;
    [SerializeField] private float clickScale;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _moveDuration;

    private Sequence _tween;
    
    public void ClickAnimation(Transform point, bool loop = true)
    {
        _tween.Kill();
        cursor.SetActive(true);
        cursor.transform.position = point.position;
        var target = cursor.transform;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        _tween = DOTween.Sequence()
            .Append(target.DOScale(defaultScale, 0f))
            .Append(target.DOScale(clickScale, _scaleDuration))
            .Append(target.DOScale(defaultScale, _scaleDuration));

        if (loop)
            _tween.SetLoops(-1);

        _tween.OnComplete(Kill);
    }

    public void DragAnimation(Transform pickPoint, Transform endPoint)
    {
        _tween.Kill();
        cursor.SetActive(true);
        cursor.transform.position = pickPoint.position;
        var target = cursor.transform;
        _tween = DOTween.Sequence()
            //.Append(target.DOMove(pickPoint.position, 0f))
            .Append(_image.DOColor(new Color(_image.color.r, _image.color.g, _image.color.b, 0f), 0f))
            .Append(target.DOScale(defaultScale, 0f))
            .Append(target.DOScale(clickScale, _scaleDuration))
            .Append(target.DOMove(endPoint.position, _moveDuration))
            .Append(target.DOScale(defaultScale, _scaleDuration))
            .Insert(0f, _image.DOColor(new Color(_image.color.r, _image.color.g, _image.color.b, 1f), _scaleDuration))
            .Insert(_scaleDuration + _moveDuration, _image.DOColor(new Color(_image.color.r, _image.color.g, _image.color.b, 0f), _scaleDuration))
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

    public void SetRotation(float value) => cursor.transform.eulerAngles = new Vector3(0f, 0f, value);

    private void Awake()
    {
        DOTween.useSmoothDeltaTime = true;
    }
}