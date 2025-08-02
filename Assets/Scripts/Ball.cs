using TMPro;
using UnityEngine;

using Vec3 = UnityEngine.Vector3;
using Vec2 = UnityEngine.Vector2;
using UnityEngine.UI;
using System;

public class Ball : MonoBehaviour
{
    BallUI ballUI;
    BallVisuals ballVisuals;
    BallPhysics ballPhysics;

    private void Awake()
    {
        ballUI = GetComponent<BallUI>();
        ballVisuals = GetComponent<BallVisuals>();
        ballPhysics = GetComponent<BallPhysics>();
    }

    public void SetNumber(int number)
    {
        int index = (int)(Math.Log(number, 2)) - 1;
        ballUI.SetBallNumber(number);
        ballVisuals.SetColorByIndex(index);
        ballPhysics.SetRadiusByIndex(index);
    }

    public void SetOpacity(float alpha)
    {
        ballUI.SetOpacity(alpha);
        ballVisuals.SetOpacity(alpha);
    }

    public BallUI GetUI()
    {
        return ballUI;
    }

    public BallVisuals GetVisuals()
    {
        return ballVisuals;
    }

    public BallPhysics GetPhysics()
    {
        return ballPhysics;
    }
}
