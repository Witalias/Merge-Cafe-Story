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
    
    public void ClickAnimation(Vector2 point)
    {
        _tween.Kill();
        cursor.SetActive(true);
        cursor.transform.position = point;
        var target = cursor.transform;
        _tween = DOTween.Sequence()
            .Append(target.DOScale(defaultScale, 0f))
            .Append(target.DOScale(clickScale, _scaleDuration))
            .Append(target.DOScale(defaultScale, _scaleDuration))
            .SetLoops(-1);
    }

    public void DragAnimation(Vector2 pickPoint, Vector2 endPoint)
    {
        _tween.Kill();
        cursor.SetActive(true);
        cursor.transform.position = pickPoint;
        var target = cursor.transform;
        _tween = DOTween.Sequence()
            .Append(_image.DOColor(new Color(_image.color.r, _image.color.g, _image.color.b, 0f), 0f))
            .Append(target.DOScale(defaultScale, 0f))
            .Append(target.DOScale(clickScale, _scaleDuration))
            .Append(target.DOMove(endPoint, _moveDuration))
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