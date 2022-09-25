using UnityEngine;
using System.Collections;

public class QuickClickTracking : MonoBehaviour
{
    private const float _checkingTime = 0.1f;

    private bool _quickClicked = false;

    public bool QuickClicked
    {
        get
        {
            var current = _quickClicked;
            _quickClicked = false;
            return current;
        }
        set => _quickClicked = value;
    }

    private void OnMouseDown()
    {
        StartCoroutine(CheckQuickClick());
    }

    private IEnumerator CheckQuickClick()
    {
        yield return new WaitForSeconds(_checkingTime);
        if (Input.GetMouseButton(0))
            yield break;
        QuickClicked = true;
    }
}
