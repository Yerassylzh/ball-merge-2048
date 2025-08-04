using UnityEngine;
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

    public void Merge(Ball otherBall)
    {
        Ball thisBall = GetComponent<Ball>();

        if (otherBall != null && otherBall.GetUI().GetNumber() == thisBall.GetUI().GetNumber())
        {
            Ball upperBall;
            Ball lowerBall;

            if (transform.position.y > otherBall.transform.position.y)
            {
                upperBall = thisBall;
                lowerBall = otherBall;
            }
            else
            {
                upperBall = otherBall;
                lowerBall = thisBall;
            }

            upperBall.GetPhysics().OnBallDestroyed.Invoke(upperBall);
            Destroy(upperBall.gameObject);

            int newNumber = lowerBall.GetUI().GetNumber() * 2;
            lowerBall.SetNumber(newNumber);
        }
    }
}
