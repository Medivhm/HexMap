using System;
using Tiles;
using UnityEngine;

public enum HandType
{
    None,
    OneFinger,
    TwoFinger,
}

public enum MoveType
{
    FreeMove,            // �������ƶ�
    Follow,              // ���������ƶ�
}

public enum CameraMoveTo
{
    Pos,
    Transfrom,
}

public class CameraController : MonoBehaviour
{
    public float EulerRotateX => this.transform.rotation.eulerAngles.x;

    new Camera camera;
    bool mapTouchSwitch => Main.Instance.Mode == GameMode.MoveMap;
    bool canTouchMove = true;
    readonly float CloseShot = 3f;
    readonly float FarShot = 5f;

    #region �����ƫ����
    Vector3 followHex => new Vector3(0f, distY, -distZ);
    float thetaX => this.transform.rotation.eulerAngles.x;
    float distZ = 5f;
    float distY => Mathf.Tan(thetaX * Mathf.Deg2Rad) * distZ;
    #endregion

    HandType _handType = HandType.None;
    HandType HandType
    {
        get => _handType;
        set
        {
            if(value == _handType) return;

            if(value == HandType.None)
            {
                SetMoveState(false);

                //Debug.Log("���None��");
            }
            else if(value == HandType.OneFinger)
            {
                if(_handType == HandType.TwoFinger)
                {
                    SetZoomState(false);
                }
                SetMoveState(true);
                SetMoveType(MoveType.FreeMove);
                //Debug.Log("���One��");
            }
            else if(value == HandType.TwoFinger)
            {
                if (_handType == HandType.OneFinger)
                {
                    SetMoveState(false);
                }
                SetZoomState(true);
                SetMoveType(MoveType.FreeMove);
                //Debug.Log("���Two��");
            }
            _handType = value;
        }
    }
    MoveType _moveType = MoveType.FreeMove;
    MoveType MoveType
    {
        get => _moveType;
        set
        {
            if(value == _moveType) return;

            _moveType = value;
        }
    }

    #region Up Right Down
    Vector3 Up => this.transform.forward;
    Vector3 Right => this.transform.right;
    Vector3 Down = new Vector3(0f, 0f, -1f);
    #endregion

    private void Update()
    {
        if(Main.Instance.MainPlayer == null) return;

        CheckHandTouch();    // �жϴ���ģʽ
        if(MoveType == MoveType.FreeMove && canTouchMove)
        {
            CameraMove();        // ��ͼ�϶�
            CameraZoom();        // ��ͼ�Ŵ���С
        }
        else if(MoveType == MoveType.Follow)
        {
            CameraMoveToPos(Main.Instance.MainPlayer.Unit.transform.position);
        }
        CameraSmoothMove();  // �������������Ŀ��λ��
    }

    private void CheckHandTouch()
    {
        if (!mapTouchSwitch) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandType = HandType.OneFinger;
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandType = HandType.None;
        }
#else
        if (Input.touchCount == 2)
        {
            HandType = HandType.TwoFinger;
        }
        else if (Input.touchCount == 1)
        {
            HandType = HandType.OneFinger;
        }
        else
        {
            HandType = HandType.None;
        }
#endif
    }

    #region �������ý��ô����ƶ��Ŵ�
    public void DisableTouch()
    {
        canTouchMove = false;
    }

    public void EnableTouch()
    {
        canTouchMove = true;
    }
    #endregion

    #region ����ƽ���ƶ��ӿ�
    /// <summary>
    /// ��Ϸ��ʼ�ƶ������
    /// </summary>
    public void StartGameCamera()
    {
        FollowPlayer();
    }

    public void MoveToPlayer(Action ac = null)
    {
        CameraMoveToPos(Main.Instance.MainPlayer.Unit.transform.position, () =>
        {
            if (ac != null)
            {
                ac.Invoke();
            }
        });
    }

    public void FollowPlayer()
    {
        CameraFollowTransform(Main.Instance.MainPlayer.Unit.transform);
    }

    CameraMoveTo _moveTo;
    Vector3 _targetPos;
    Transform _targetTrans;
    Vector3 targetPos
    {
        get
        {
            if(_moveTo == CameraMoveTo.Pos)
            {
                return CalcuFollowPos(_targetPos);
            }
            else
            {
                return CalcuFollowPos(_targetTrans.position);
            }
        }
    }

    float smoothCoeff = 1.5f;
    bool isMoving = false;
    Action arriveCB;
    public void CameraMoveToHex(HexNode hexNode, Action ac = null, float targetHeight = -1f)
    {
        CameraMoveToPos(hexNode.Coords.WorldPos, ac, targetHeight);
    }

    // MovePos �� FollowTrans ����
    public void CameraMoveToPos(Vector3 pos, Action ac = null, float targetHeight = -1f)
    {
        if (targetHeight > 0f)
        {
            distZ = targetHeight;
        }

        SetTarget(pos);
        isMoving = true;
        arriveCB = ac;
    }

    // ����Ǿ�̬Trans����Ȼ����ص���ʱ��..���߼��Ͼͺ����ж�
    public void CameraMoveToTrans(Transform trans, Action ac = null, float targetHeight = -1f)
    {
        if (targetHeight > 0f)
        {
            distZ = targetHeight;
        }

        SetTarget(trans);
        isMoving = true;
        arriveCB = ac;
    }

    // ���������������ǣ���������Զ�������棬����ʼ�ո���
    public void CameraFollowTransform(Transform trans)
    {
        SetTarget(trans);
        MoveType = MoveType.Follow;
        isMoving = true;
        arriveCB = null;
    }

    /// �ж�ƽ���ƶ�
    public void StopSmoothMove()
    {
        isMoving = false;
    }

    private void SetTarget(Vector3 pos)
    {
        _moveTo = CameraMoveTo.Pos;
        _targetPos = pos;
    }

    private void SetTarget(Transform trans)
    {
        _moveTo = CameraMoveTo.Transfrom;
        _targetTrans = trans;
    }
    #endregion

    #region �ڲ�״̬�л���������
    bool moving = false;
    bool zooming = true;
    private void SetMoveType(MoveType moveType)
    {
        MoveType = moveType;
    }

    private void SetMoveState(bool state)
    {
        moving = state;
        if(state) touch = Input.mousePosition;
    }

    private bool GetMoveState()
    {
        return moving;
    }

    private void SetZoomState(bool state)
    {
        zooming = state;
        if (state)
        {
            touch = Input.GetTouch(1).position - Input.GetTouch(0).position;
            tempFloat = touch.magnitude;
        }
    }

    private bool GetZoomState()
    {
        return zooming;
    }
    #endregion

    #region �ڲ�ƽ���ƶ��������ƶ����Ŵ��߼�

    /// <summary>
    /// ͨ��ƽ���ƶ�
    /// </summary>
    private void CameraSmoothMove()
    {
        if(!isMoving) return;

        float lerp = smoothCoeff * (1 - Mathf.Exp(-Time.deltaTime));
        transform.position = Vector3.Lerp(transform.position, targetPos, lerp > 0.03f ? lerp : 0.03f);
        if (Vector3.Distance(targetPos, this.transform.position) < 0.015f)
        {
            this.transform.position = targetPos;
            isMoving = false;
            if (arriveCB != null)
            {
                arriveCB.Invoke();
            }
        }
    }

    private Vector3 CalcuFollowPos(Vector3 pos)
    {
        Vector3 characterScreenPos = camera.WorldToScreenPoint(pos);
        Vector3 targetScreenPos = new Vector3(characterScreenPos.x + Screen.width * 0.25f, characterScreenPos.y, characterScreenPos.z);
        Vector3 targetWorldPos = camera.ScreenToWorldPoint(targetScreenPos);
        return targetWorldPos + followHex;
    }

    float moveXCoeff = 0.3f, moveYCoeff = 0.3f;
    Vector2 touch;
    Vector2 temp;
    float tempFloat;
    private void CameraMove()
    {
        if (!mapTouchSwitch) return;
        if (GetMoveState() == false) return;
        if (!TouchDuration.Instance.IsDrag()) return;

        StopSmoothMove();
        temp = (Vector2)Input.mousePosition - touch;
        this.transform.position += new Vector3(-temp.x * moveXCoeff, -temp.y * moveYCoeff) * Time.deltaTime;

        touch = (Vector2)Input.mousePosition;
    }

    float scrollSpeed = 180f;
    private void CameraZoom()
    {
        if (!mapTouchSwitch) return;
        if (GetZoomState() == false) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.01f) return;

        StopSmoothMove();
#if UNITY_EDITOR || UNITY_EDITOR_WIN
        this.transform.position -= Down * scroll * scrollSpeed * Time.deltaTime;
#else
        float distance = (Input.GetTouch(1).position - Input.GetTouch(0).position).magnitude;
        this.transform.position -= Down * (distance - tempFloat) * 0.1f * Time.deltaTime;
        tempFloat = distance;
#endif
        distZ = -this.transform.position.z; 
    }

    #endregion

    #region Awake OnDestroy
    private void Awake()
    {
        Main.Instance.MainCameraController = this;
        this.camera = this.GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        Main.Instance.MainCameraController = null;
    }
    #endregion
}