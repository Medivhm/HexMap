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
    FreeMove,            // 可自由移动
    FollowPlayer,        // 不可自由移动
}

public class CameraController : MonoBehaviour
{
    public float EulerRotateX => this.transform.rotation.eulerAngles.x;

    new Camera camera;
    bool mapTouchSwitch => Main.Instance.Mode == GameMode.MoveMap;
    bool canTouchMove = true;

    #region 摄像机偏移量
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

                //Debug.Log("变成None了");
            }
            else if(value == HandType.OneFinger)
            {
                if(_handType == HandType.TwoFinger)
                {
                    SetZoomState(false);
                }
                SetMoveState(true);
                SetMoveType(MoveType.FreeMove);
                //Debug.Log("变成One了");
            }
            else if(value == HandType.TwoFinger)
            {
                if (_handType == HandType.OneFinger)
                {
                    SetMoveState(false);
                }
                SetZoomState(true);
                SetMoveType(MoveType.FreeMove);
                //Debug.Log("变成Two了");
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

        CheckHandTouch();    // 判断触摸模式
        if(MoveType == MoveType.FreeMove && canTouchMove)
        {
            CameraMove();        // 地图拖动
            CameraZoom();        // 地图放大缩小
        }
        else if(MoveType == MoveType.FollowPlayer)
        {
            CameraMoveToPos(Main.Instance.MainPlayer.Unit.transform.position);
        }
        CameraSmoothMove();  // 摄像机缓慢移向目标位置
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

    #region 对外启用禁用触摸移动放大
    public void DisableTouch()
    {
        canTouchMove = false;
    }

    public void EnableTouch()
    {
        canTouchMove = true;
    }
    #endregion

    #region 对外平滑移动接口
    /// <summary>
    /// 游戏开始移动摄像机
    /// </summary>
    public void StartGameCamera()
    {
        // 如果FollowPlayer过早的话（在场景刚加载完后），CameraMoveToPos摄像机移动位置会有误
        SetMoveType(MoveType.FollowPlayer);
    }

    public void FollowPlayer(Action ac = null)
    {
        CameraMoveToPos(Main.Instance.MainPlayer.Unit.transform.position, () =>
        {
            SetMoveType(MoveType.FollowPlayer);
            if (ac != null)
            {
                ac.Invoke();
            }
        });
    }


    Vector3 targetPos;
    float smoothCoeff = 1.5f;
    bool isMoving = false;
    Action arriveCB;

    public void CameraMoveToHex(HexNode hexNode, Action ac = null)
    {
        CameraMoveToPos(hexNode.Coords.WorldPos, ac);
    }

    public void CameraMoveToPos(Vector3 pos, Action ac = null)
    {
        targetPos = CalcuFollowPos(pos);
        isMoving = true;
        arriveCB = ac;
    }

    /// <summary>
    /// 中断平滑移动
    /// </summary>
    public void StopSmoothMove()
    {
        isMoving = false;
    }

    #endregion

    #region 内部状态切换，不对外
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

    #region 内部平滑移动，触摸移动、放大逻辑

    /// <summary>
    /// 通用平滑移动
    /// </summary>
    private void CameraSmoothMove()
    {
        if(!isMoving) return;

        float lerp = smoothCoeff * (1 - Mathf.Exp(-Time.deltaTime));
        transform.position = Vector3.Lerp(transform.position, targetPos, lerp > 0.023f ? lerp : 0.023f);
        if (Vector3.Distance(targetPos, this.transform.position) < 0.001f)
        {
            this.transform.position = targetPos;
            isMoving = false;
            if(arriveCB != null)
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
        distZ = -this.transform.position.z;
        
#else
        float distance = (Input.GetTouch(1).position - Input.GetTouch(0).position).magnitude;
        this.transform.position -= Down * (distance - tempFloat) * 0.1f * Time.deltaTime;
        tempFloat = distance;
#endif
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