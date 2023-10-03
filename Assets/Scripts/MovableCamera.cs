/*
 * file: CameraMove.cs
 * author: D.H.
 * feature: 移动相机
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 可移动的相机
/// </summary>
public class MovableCamera : MonoBehaviour
{
    private Camera thisCamera;

    public static MovableCamera Instance;

    [Header("相机移动范围")] public Vector2 leftDown, upRight;

    [Header("移动容差")] public float tolerance;

    [Header("移动灵敏度")] public float sensitivity;

    [Header("移动时间")] public float duration;

    private IEnumerator _enumerator;

    public bool Indrag { get; private set; } = false;

    public bool Moveable { get; set; } = true;

    private void Start()
    {
    }

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
        Instance = this;
    }

    Vector3 GetMousePositionInWorld()
    {
        Vector3 screenPosition = Input.mousePosition;
        return thisCamera.ScreenToWorldPoint(screenPosition);
    }

    private IEnumerator DragMoveCoroutine()
    {
        Vector3 initialMousePosition = GetMousePositionInWorld();
        Indrag = true;
        while (Input.GetMouseButton(0) && Moveable)
        {
            //Debug.Log("Check Drag!");
            Vector3 currentMousePosition = GetMousePositionInWorld();
            Vector3 travel = currentMousePosition - initialMousePosition;
            //Debug.Log($"target: {transform.position - travel}");
            travel.z = 0;
            if (travel.magnitude / transform.GetComponent<Camera>().orthographicSize > sensitivity)
            {
                CameraMoveWithTolerance(transform.position - travel);
            }
            yield return null;
        }

        Indrag = false;
    }

    private void CameraMoveWithTolerance(Vector3 target)
    {
        target.x = Mathf.Clamp(target.x, leftDown.x + tolerance, upRight.x - tolerance);
        target.y = Mathf.Clamp(target.y, leftDown.y + tolerance, upRight.y - tolerance);
        //Debug.Log($"Camera move to {target}");
        transform.DOMove(target, duration);
    }

    public void Init(int width, int height, int startX, int startY)
    {
        leftDown = new Vector2(0, 0);
        upRight = new Vector2(width, height);
        transform.position = new Vector3(startX, startY, -10);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !Indrag && Moveable)
        {
            StartCoroutine(DragMoveCoroutine());
        }
    }
}