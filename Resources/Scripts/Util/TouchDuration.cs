using UnityEngine;

/// <summary>
/// 判定是点击还是拖动，长按还是短按
/// </summary>
public class TouchDuration : MonoSingleton<TouchDuration>   
{
    private float _touchDuration; // 持续时间
    private bool _isTouching;     // 是否正在触摸
    private Vector2 _delta;
    private Vector2 _touchPos;

    private float _touchJudgeTime = 0.5f; // 判定为长按的时间
    private float _touchJudgeDist = 2f;   // 判定为拖动的距离


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else 
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
            _touchDuration = 0f;
            _delta = Vector2.zero;
            _isTouching = true;
            _touchPos = Input.mousePosition;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0) && _isTouching)
#else
        if (Input.touchCount != 1 && _isTouching)
#endif
        {
            _isTouching = false;
        }


        if (_isTouching)
        {
            _touchDuration += Time.deltaTime;
            _delta = _touchPos - (Vector2)Input.mousePosition;
        }
    }

    public bool IsClick()
    {
        return _delta.sqrMagnitude < _touchJudgeDist;
    }

    public bool IsDrag()
    {
        return _delta.sqrMagnitude > _touchJudgeDist;
    }

    public bool IsShotClick()
    {
        return _touchDuration < _touchJudgeTime;
    }

    public bool IsLongClick()
    {
        return _touchDuration > _touchJudgeTime;
    }
}
