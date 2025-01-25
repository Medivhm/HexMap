using UnityEngine;

/// <summary>
/// �ж��ǵ�������϶����������Ƕ̰�
/// </summary>
public class TouchDuration : MonoSingleton<TouchDuration>   
{
    private float _touchDuration; // ����ʱ��
    private bool _isTouching;     // �Ƿ����ڴ���
    private Vector2 _delta;
    private Vector2 _touchPos;

    private float _touchJudgeTime = 0.5f; // �ж�Ϊ������ʱ��
    private float _touchJudgeDist = 2f;   // �ж�Ϊ�϶��ľ���


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
