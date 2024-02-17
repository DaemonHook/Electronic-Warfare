using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 可移动的镜头
/// </summary>
public class MovableCamera : MonoBehaviourSingleton<MovableCamera>
{
    private Camera thisCamera;


    [Header("相机移动范围")] public Vector2 leftDown, upRight;

    [Header("移动容差")] public float tolerance;

    [Header("判断是否在拖动状态的阈值")] public float IndragThreshold;

    public enum MoveMode
    {
        cam,
        obj,
    }


    //上次鼠标位置
    Vector2 prevMousePos = Vector3.zero;

    //滑动结束时的瞬时速度
    Vector3 Speed = Vector3.zero;

    //每帧偏差
    Vector3 offSet = Vector3.zero;

    //鼠标开始位置
    Vector3 startMousePosition = Vector3.zero;

    //速度衰減率
    [Header("惯性滑动缩减率")] public float decelerationRate = 0.1f;

    //摄像机
    public Camera m_camera;

    //移动模式
    public MoveMode m_moveMode = MoveMode.obj;

    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.touchCount > 0)
        {
            //按下时记录位置
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                prevMousePos = Input.GetTouch(0).position;
                startMousePosition = Input.GetTouch(0).position;
            }
        }
        //移动时更新位置

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector3 curMousePosition = Input.GetTouch(0).position; //当前鼠标的屏幕坐标系
            //偏差值
            offSet = m_camera.ScreenToWorldPoint(curMousePosition) - m_camera.ScreenToWorldPoint(prevMousePos);
            prevMousePos = curMousePosition;
            //瞬时速度
            Speed = offSet / Time.deltaTime;
        }
        else //最后递减
        {
            Speed *= Mathf.Pow(decelerationRate, Time.deltaTime);
            if (Mathf.Abs(Vector3.Magnitude(Speed)) < 1)
            {
                Speed = Vector3.zero;
            }
        }

        Move(Speed);
    }

    public bool Indrag => Speed.magnitude >= IndragThreshold;

    public void Move(Vector3 speed)
    {
        if (Vector3.Magnitude(Speed) == 0)
        {
            return;
        }

        // Debug.Log("Current Speed" + Vector3.Magnitude(speed));
        if (m_moveMode == MoveMode.obj)
        {
            var localPosition = transform.localPosition;
            localPosition -= speed * Time.deltaTime;
            localPosition = new Vector3(
                Math.Clamp(localPosition.x, leftDown.x + tolerance, upRight.x - tolerance),
                Math.Clamp(localPosition.y, leftDown.y + tolerance, upRight.y - tolerance),
                -10
            );
            transform.localPosition = localPosition;
        }
        else
        {
            var localPosition = m_camera.transform.localPosition;
            localPosition -= speed * Time.deltaTime;
            localPosition = new Vector3(
                Math.Clamp(localPosition.x, leftDown.x + tolerance, upRight.x - tolerance),
                Math.Clamp(localPosition.y, leftDown.y + tolerance, upRight.y - tolerance),
                -10
            );
            m_camera.transform.localPosition = localPosition;
        }
    }

    public void Init(int width, int height, int originX, int originY)
    {
        transform.position = new Vector3(originX, originY, -10);
        leftDown = new Vector2(-0.5f, -0.5f);
        upRight = new Vector2(width - 0.5f, height - 0.5f);
    }

    public override void Awake()
    {
        base.Awake();
        m_camera = GetComponent<Camera>();
#if UNITY_STANDALONE || UNITY_EDITOR
        decelerationRate = 0.0f;
#elif UNITY_IOS || UNITY_ANDROID
        decelerationRate = 0.1f;
#endif
    }
}