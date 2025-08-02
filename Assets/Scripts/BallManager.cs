using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;

public class BallManager : MonoBehaviour
{
    [SerializeField] Ball ballPrefab;
    [SerializeField] float ballSpawnBottomPadding = 0.9f;
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] GameObject tube;
    List<Ball> balls;
    Ball currentBall;
    Ball previewBall;

    bool isPressing = false;
    Vec2 prevPressPos;
    Vec2 currentPressPos;

    float spawnXPos;
    float spawnYPos;


    void Awake()
    {
        balls = new List<Ball>();
    }

    void Update()
    {
        if (IsGameOver())
        {
            OnGameOver();
            SceneManager.LoadScene("GameOver");
        }

        UpdateCurrentScore();
        UpdateBestScore();

        if (isPressing)
        {
            UpdateSpawnPositions();

            currentBall.transform.position = new Vec3(spawnXPos, spawnYPos);

            Tube tubeObj = tube.transform.GetComponent<Tube>();
            var previewPos = GetBallPreviewPosition(tubeObj.GetStartY(), tubeObj.GetEndY() + tubeObj.GetTubeThickness() + previewBall.GetPhysics().GetRadius());
            previewBall.transform.position = new Vec3(spawnXPos, previewPos.y);
        }
    }

    public void OnScreenPress(Vec2 pos)
    {
        isPressing = true;
        currentBall = InstantiateFreezedBall(spawnXPos, spawnYPos);

        Tube tubeObj = tube.transform.GetComponent<Tube>();
        var previewPos = GetBallPreviewPosition(tubeObj.GetStartY(), tubeObj.GetEndY() + tubeObj.GetTubeThickness() + currentBall.GetPhysics().GetRadius());
        previewBall = InstantiateFreezedBall(spawnXPos, previewPos.y);
        previewBall.SetNumber(currentBall.GetUI().GetNumber());
        previewBall.SetOpacity(0.4f);
        InitializeSpawnPositions();
    }

    public void OnScreenHold(Vec2 pos)
    {
        if (currentPressPos == null)
        {
            currentPressPos = pos;
            prevPressPos = pos;
        }
        else
        {
            prevPressPos = currentPressPos;
            currentPressPos = pos;
        }
    }

    public void OnScreenRelease(Vec2 pos)
    {
        isPressing = false;
        DropBubble();
    }

    private void InitializeSpawnPositions()
    {
        float ballRadius = currentBall.GetPhysics().GetRadius();
        float aspect = (float)Screen.width / Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;

        Tube tubeObj = tube.transform.GetComponent<Tube>();

        float startX = -(worldWidth / 2) + tubeObj.GetHorizontalPadding() + tubeObj.GetTubeThickness() + ballRadius;
        float endX = (worldWidth / 2) - tubeObj.GetHorizontalPadding() - tubeObj.GetTubeThickness() - ballRadius;

        spawnXPos = (endX + startX) / 2;
        spawnYPos = worldHeight / 2 - tubeObj.GetTopPadding() + ballRadius + ballSpawnBottomPadding;
    }

    private void UpdateCurrentScore()
    {
        int mx = 0;
        for (int i = 0; i < balls.Count; i++)
            mx = Math.Max(mx, balls[i].GetUI().GetNumber());

        if (currentScoreText.text != mx.ToString())
            currentScoreText.text = mx.ToString();
    }

    private void OnGameOver()
    {
        PlayerPrefs.SetInt("CurrentScore", int.Parse(currentScoreText.text));
    }

    private void UpdateBestScore()
    {
        int savedBest = PlayerPrefs.GetInt("BestScore", 0);
        int currentScore = int.Parse(currentScoreText.text);
        if (savedBest < currentScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            savedBest = currentScore;
        }
        bestScoreText.text = savedBest.ToString();
    }

    private bool IsGameOver()
    {
        if (balls.Count == 0)
            return false;

        float minY = balls[0].transform.position.y;
        for (int i = 1; i < balls.Count; i++)
            minY = Math.Min(minY, balls[i].transform.position.y);

        var tubeIns = tube.GetComponent<Tube>();
        return minY < tubeIns.GetEndY();

    }

    // Before calling this, make sure currentBall != null
    private void UpdateSpawnPositions()
    {
        float ballRadius = currentBall.GetPhysics().GetRadius();
        float aspect = (float)Screen.width / Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;

        Tube tubeObj = tube.transform.GetComponent<Tube>();

        float startX = -(worldWidth / 2) + tubeObj.GetHorizontalPadding() + tubeObj.GetTubeThickness() + ballRadius;
        float endX = (worldWidth / 2) - tubeObj.GetHorizontalPadding() - tubeObj.GetTubeThickness() - ballRadius;

        spawnXPos = Math.Clamp(spawnXPos + (currentPressPos - prevPressPos).x * 2, startX, endX);
        spawnYPos = worldHeight / 2 - tubeObj.GetTopPadding() + ballRadius + ballSpawnBottomPadding;
    }

    public void DropBubble()
    {
        Destroy(previewBall.gameObject);
        previewBall = null;

        currentBall.GetPhysics().EnablePhysics();
        balls.Add(currentBall);
    }

    private int GenerateRandomBallNumber(int maxNum = 2048)
    {
        float threshold = 1 - (1.0f / maxNum);
        float randomNum = UnityEngine.Random.Range(0, threshold);
        int currentPow = 1;
        float currentVal = 0;
        for (int i = 1; (1 << currentPow) < maxNum; i++)
        {
            currentVal += 1.0f / (1 << currentPow);
            if (randomNum <= currentVal)
            {
                return 1 << currentPow;
            }
            currentPow += 1;
        }
        return maxNum;
    }

    private Vec2 GetBallPreviewPosition(float startY, float endY)
    {
        bool hitsGround(float yPosition)
        {
            Tube tubeIns = tube.GetComponent<Tube>();

            Vec2 border1 = new(
                tubeIns.GetStartX() + tubeIns.GetTubeThickness() + tubeIns.GetBorderRadius(),
                tubeIns.GetEndY() + tubeIns.GetTubeThickness() + tubeIns.GetBorderRadius()
            );
            Vec2 border2 = new(
               tubeIns.GetEndX() - tubeIns.GetTubeThickness() - tubeIns.GetBorderRadius(),
               border1.y
            );

            float ballRadius = currentBall.GetPhysics().GetRadius();

            if (spawnXPos < border1.x || border2.x < spawnXPos)
            {
                float minDist = 100;
                Vec2 absP = Vec2.zero;

                float angleDt = 1f;
                bool isLeft = spawnXPos < border1.x;
                float startAngle = isLeft ? 180 : 270;
                float endAngle = isLeft ? 270 : 360;
                var borderCenter = isLeft ? border1 : border2;
                for (float angle = startAngle; angle <= endAngle; angle += angleDt)
                {
                    Vec2 localPos = new(tubeIns.GetBorderRadius() * Mathf.Cos(Mathf.Deg2Rad * angle), tubeIns.GetBorderRadius() * Mathf.Sin(Mathf.Deg2Rad * angle));
                    Vec2 absPos = borderCenter + localPos;
                    float dist = Vec2.Distance(new Vec2(spawnXPos, yPosition), absPos);
                    if (dist < ballRadius)
                        return true;

                    if (dist < minDist)
                    {
                        minDist = dist;
                        absP = absPos;
                    }
                }

                return false;
            }
            else
            {
                float groudLevel = tubeIns.GetEndY() + tubeIns.GetTubeThickness();
                return yPosition - ballRadius < groudLevel;
            }
        }

        for (int _ = 0; _ < 60; _++)
        {
            float midY = (startY + endY) / 2;
            bool overlaps = hitsGround(midY);
            for (int i = 0; i < balls.Count && !overlaps; i++)
            {
                var ball = balls[i];
                if (Utils.CheckCirclesOverlap(ball.transform.position, ball.GetPhysics().GetRadius(), new Vec2(spawnXPos, midY), currentBall.GetPhysics().GetRadius()))
                    overlaps = true;

            }
            if (overlaps)
                endY = midY;
            else
                startY = midY;
        }
        return new Vec2(spawnXPos, (startY + endY) / 2);
    }

    private Ball InstantiateFreezedBall(float xPos, float yPos)
    {
        Ball ball = Instantiate(ballPrefab, new Vec3(xPos, yPos), UnityEngine.Quaternion.identity);
        ball.SetNumber(GenerateRandomBallNumber());
        ball.GetPhysics().DisablePhysics();
        return ball;
    }

}

public static class Utils
{
    public static bool CheckCirclesOverlap(Vec2 pos1, float radius1, Vec2 pos2, float radius2)
    {
        float dist = Vec2.Distance(pos1, pos2);
        return dist < radius1 + radius2;
    }
}

