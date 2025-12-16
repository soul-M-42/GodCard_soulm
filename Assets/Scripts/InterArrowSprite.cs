using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class InterArrowSprite : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool isActive = false;
    private Vector3 posFrom;
    private Vector3 posTo;


    // 箭头参数
    public float arrowHeadAngle = 20f;  // 箭头角度
    public float arrowHeadLength = 0.5f; // 箭头长度

    // Inspector 可调参数
    public float lineWidth = 0.1f;       // 线宽
    public Color lineColor = Color.white; // 线颜色

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false; // 初始隐藏

        // 初始化 LineRenderer 属性
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void Update()
    {
        // 实时更新线条宽度和颜色
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        if (isActive)
        {
            DrawArrow(posFrom, posTo);
        }
    }

    public void Activate()
    {
        isActive = true;
        lineRenderer.enabled = true;
    }

    public void Deactivate()
    {
        isActive = false;
        lineRenderer.enabled = false;
    }

    // 传入屏幕坐标，转换为世界坐标，Z固定为0
    public void SetPos(string target, Vector3 pos, bool fromScreen = true)
    {
        Vector3 worldPos = fromScreen ? Camera.main.ScreenToWorldPoint(pos) : pos;

        if(target == "from")
            posFrom = new Vector3(worldPos.x, worldPos.y, 0f);
        else if(target == "to")
            posTo = new Vector3(worldPos.x, worldPos.y, 0f);
    }

    public void SetPosTo(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        posTo = new Vector3(worldPos.x, worldPos.y, 0f);
    }

    private void DrawArrow(Vector3 from, Vector3 to)
    {
        // 主线
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);

        // 箭头
        Vector3 direction = (to - from).normalized;
        Vector3 right = Quaternion.Euler(0, 0, arrowHeadAngle) * -direction;
        Vector3 left = Quaternion.Euler(0, 0, -arrowHeadAngle) * -direction;

        // 绘制箭头线（仅调试用）
        Debug.DrawLine(to, to + right * arrowHeadLength, Color.red);
        Debug.DrawLine(to, to + left * arrowHeadLength, Color.red);
    }
}
