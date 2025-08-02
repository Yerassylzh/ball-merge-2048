using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.U2D;

using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;

public class Tube : MonoBehaviour
{
    [SerializeField] float borderRadius = 0.2f;
    [SerializeField] float topPadding = 2.0f;
    [SerializeField] float bottomPadding = 1f;
    [SerializeField] float horizontalPadding = 0.1f;
    [SerializeField] float tubeThickness = 0.2f;
    [SerializeField] SpriteShapeController spriteControllerPrefab;

    SpriteShapeController spriteController;
    float startX;
    float endX;
    float startY;
    float endY;

    int prevScreenWidth;

    void Awake()
    {
        prevScreenWidth = Screen.width;
        CheckComponents();
        CreateTube();
    }

    void Update()
    {
        if (prevScreenWidth != Screen.width)
        {
            UpdatePositionVars();
            UpdateSplines();
            UpdateEdgeColliders();
        }
        prevScreenWidth = Screen.width;
    }

    void CheckComponents()
    {
        if (!GetComponent<EdgeCollider2D>())
        {
            gameObject.AddComponent<EdgeCollider2D>();
        }
    }

    void CreateTube()
    {
        spriteController = Instantiate(spriteControllerPrefab, transform.position, Quaternion.identity);
        spriteController.transform.parent = transform;
        UpdatePositionVars();
        UpdateSplines();
        UpdateEdgeColliders();
    }

    void UpdatePositionVars()
    {
        float aspect = (float)Screen.width / Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;

        startX = -(worldWidth / 2) + horizontalPadding;
        endX = (worldWidth / 2) - horizontalPadding;
        startY = worldHeight / 2 - topPadding;
        endY = -(worldHeight / 2) + bottomPadding;
    }

    public float GetStartX()
    {
        return startX;
    }
    public float GetEndX()
    {
        return endX;
    }
    public float GetStartY()
    {
        return startY;
    }
    public float GetEndY()
    {
        return endY;
    }
    public float GetTubeThickness()
    {
        return tubeThickness;
    }

    public float GetHorizontalPadding()
    {
        return horizontalPadding;
    }

    public float GetTopPadding()
    {
        return topPadding;
    }

    public float GetBorderRadius()
    {
        return borderRadius;
    }

    void UpdateSplines()
    {
        var points = new List<Vec2>();

        points.Add(new(startX + tubeThickness / 2, startY));

        Vec2 center1 = new(startX + tubeThickness + borderRadius, endY + borderRadius + tubeThickness);
        Vec2 center2 = new(endX - tubeThickness - borderRadius, endY + borderRadius + tubeThickness);
        points.AddRange(GetBorderEdgePoints(center1, borderRadius + tubeThickness / 2, 10, 180, 270));
        points.AddRange(GetBorderEdgePoints(center2, borderRadius + tubeThickness / 2, 10, 270, 360));

        points.Add(new(endX - tubeThickness / 2, startY));

        for (int i = 0; i < points.Count; i++)
        {
            UpdateSpline(i, points[i]);
        }
    }

    void UpdateSpline(int index, Vec2 pos)
    {
        if (spriteController.spline.GetPointCount() <= index)
        {
            spriteController.spline.InsertPointAt(index, pos);
        }
        else
        {
            spriteController.spline.SetPosition(index, pos);
        }
        spriteController.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        spriteController.spline.SetRightTangent(index, Vec2.zero);
        spriteController.spline.SetLeftTangent(index, Vec2.zero);
        spriteController.spline.SetHeight(index, tubeThickness * 2);
    }

    void UpdateEdgeColliders()
    {
        var edgeCollider = GetComponent<EdgeCollider2D>();
        Vec2 center1 = new(startX + tubeThickness + borderRadius, endY + borderRadius + tubeThickness);
        Vec2 center2 = new(endX - tubeThickness - borderRadius, endY + borderRadius + tubeThickness);

        List<Vec2> points = new();
        points.Add(new Vec2(startX, startY));
        points.Add(new Vec2(startX + tubeThickness, startY));
        points.AddRange(GetBorderEdgePoints(
            center1,
            borderRadius,
            1,
            180,
            270
        ));
        points.AddRange(GetBorderEdgePoints(
            center2,
            borderRadius,
            1,
            270,
            360
        ));
        points.Add(new Vec2(endX - tubeThickness, startY));
        points.Add(new Vec2(endX, startY));

        edgeCollider.SetPoints(points);
    }

    List<Vec2> GetBorderEdgePoints(Vec2 center, float borderRadius, int step = 1, int startAngle = 180, int endAngle = 270)
    {
        List<Vec2> points = new();
        for (int angle = startAngle; angle <= endAngle; angle += step)
        {
            float rad = Mathf.Deg2Rad * angle;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            points.Add(center + new Vec2(borderRadius * cos, borderRadius * sin));
        }
        return points;
    }
}
