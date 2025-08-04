using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (IsGameOver())
        {
            OnGameOver();
            SceneManager.LoadScene("GameOver");
        }

        UpdateScore();
        CheckBallsCollision();

        if (gameManager.IsPressing)
        {
            UpdateSpawnPositions();
            gameManager.CurrentBall.transform.position = new Vector3(gameManager.SpawnXPos, gameManager.SpawnYPos);

            Tube tubeObj = gameManager.tube.GetComponent<Tube>();
            var previewPos = GetBallPreviewPosition(tubeObj.GetStartY(), tubeObj.GetEndY() + tubeObj.GetTubeThickness() + gameManager.PreviewBall.GetPhysics().GetRadius());
            gameManager.PreviewBall.transform.position = new Vector3(gameManager.SpawnXPos, previewPos.y);
        }
    }

    public void OnScreenPress(Vector2 pos)
    {
        gameManager.SetIsPressing(true);
        var newBall = InstantiateFreezedBall(gameManager.SpawnXPos, gameManager.SpawnYPos);
        gameManager.SetCurrentBall(newBall);

        Tube tubeObj = gameManager.tube.GetComponent<Tube>();
        var previewPos = GetBallPreviewPosition(tubeObj.GetStartY(), tubeObj.GetEndY() + tubeObj.GetTubeThickness() + gameManager.CurrentBall.GetPhysics().GetRadius());
        var newPreviewBall = InstantiateFreezedBall(gameManager.SpawnXPos, previewPos.y);
        gameManager.SetPreviewBall(newPreviewBall);
        gameManager.PreviewBall.SetNumber(gameManager.CurrentBall.GetUI().GetNumber());
        gameManager.PreviewBall.SetOpacity(0.4f);
        InitializeSpawnPositions();
    }

    public void OnScreenHold(Vector2 pos)
    {
        if (gameManager.CurrentPressPos == null)
        {
            gameManager.SetPressPositions(pos, pos);
        }
        else
        {
            gameManager.SetPressPositions(pos, gameManager.CurrentPressPos);
        }
    }

    public void OnScreenRelease(Vector2 pos)
    {
        gameManager.SetIsPressing(false);
        DropBubble();
    }

    private void InitializeSpawnPositions()
    {
        float ballRadius = gameManager.CurrentBall.GetPhysics().GetRadius();
        float aspect = (float)Screen.width / Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;

        Tube tubeObj = gameManager.tube.GetComponent<Tube>();

        float startX = -(worldWidth / 2) + tubeObj.GetHorizontalPadding() + tubeObj.GetTubeThickness() + ballRadius;
        float endX = (worldWidth / 2) - tubeObj.GetHorizontalPadding() - tubeObj.GetTubeThickness() - ballRadius;

        gameManager.SetSpawnPosition((endX + startX) / 2, worldHeight / 2 - tubeObj.GetTopPadding() + ballRadius + gameManager.ballSpawnBottomPadding);
    }

    private void UpdateScore()
    {
        int mx = 0;
        foreach (var ball in gameManager.AllBalls)
            mx = System.Math.Max(mx, ball.GetUI().GetNumber());

        if (gameManager.currentScoreText.text != mx.ToString())
            gameManager.currentScoreText.text = mx.ToString();

        int savedBest = PlayerPrefs.GetInt("BestScore", 0);
        int currentScore = int.Parse(gameManager.currentScoreText.text);
        if (savedBest < currentScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            savedBest = currentScore;
        }
        gameManager.bestScoreText.text = savedBest.ToString();
    }

    private void OnGameOver()
    {
        PlayerPrefs.SetInt("CurrentScore", int.Parse(gameManager.currentScoreText.text));
    }

    private bool IsGameOver()
    {
        if (gameManager.AllBalls.Count == 0)
            return false;

        float minY = gameManager.AllBalls[0].transform.position.y;
        for (int i = 1; i < gameManager.AllBalls.Count; i++)
            minY = System.Math.Min(minY, gameManager.AllBalls[i].transform.position.y);

        var tubeIns = gameManager.tube.GetComponent<Tube>();
        return minY < tubeIns.GetEndY();
    }

    private void UpdateSpawnPositions()
    {
        float ballRadius = gameManager.CurrentBall.GetPhysics().GetRadius();
        float aspect = (float)Screen.width / Screen.height;
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;

        Tube tubeObj = gameManager.tube.GetComponent<Tube>();

        float startX = -(worldWidth / 2) + tubeObj.GetHorizontalPadding() + tubeObj.GetTubeThickness() + ballRadius;
        float endX = (worldWidth / 2) - tubeObj.GetHorizontalPadding() - tubeObj.GetTubeThickness() - ballRadius;

        float newSpawnX = System.Math.Clamp(gameManager.SpawnXPos + (gameManager.CurrentPressPos - gameManager.PrevPressPos).x * 2, startX, endX);
        gameManager.SetSpawnPosition(newSpawnX, worldHeight / 2 - tubeObj.GetTopPadding() + ballRadius + gameManager.ballSpawnBottomPadding);
    }

    public void DropBubble()
    {
        if (gameManager.PreviewBall != null) Destroy(gameManager.PreviewBall.gameObject);
        gameManager.SetPreviewBall(null);

        gameManager.CurrentBall.GetPhysics().EnablePhysics();
        gameManager.AddBall(gameManager.CurrentBall);
        gameManager.CurrentBall.GetPhysics().OnBallDestroyed += (ball) => { gameManager.RemoveBall(ball); };
    }

    private void CheckBallsCollision()
    {
        for (int i = 0; i < gameManager.AllBalls.Count - 1; i++)
        {
            for (int j = i + 1; j < gameManager.AllBalls.Count - 1; j++)
            {
                Ball ball1 = gameManager.AllBalls[i];
                Ball ball2 = gameManager.AllBalls[j];
                CircleCollider2D c1 = ball1.GetComponent<CircleCollider2D>();
                CircleCollider2D c2 = ball2.GetComponent<CircleCollider2D>();
                if (c1.IsTouching(c2) && ball1.GetUI().GetNumber() == ball2.GetUI().GetNumber())
                {
                    ball1.Merge(ball2);
                    return; // One merge per frame
                }
            }
        }
    }

    private Ball InstantiateFreezedBall(float xPos, float yPos)
    {
        Ball ball = Instantiate(gameManager.ballPrefab, new Vector3(xPos, yPos), Quaternion.identity);
        ball.SetNumber(GenerateRandomBallNumber());
        ball.GetPhysics().DisablePhysics();
        return ball;
    }

    private Vector2 GetBallPreviewPosition(float startY, float endY)
    {
        bool hitsGround(float yPosition)
        {
            Tube tubeIns = gameManager.tube.GetComponent<Tube>();

            Vector2 border1 = new Vector2(
                tubeIns.GetStartX() + tubeIns.GetTubeThickness() + tubeIns.GetBorderRadius(),
                tubeIns.GetEndY() + tubeIns.GetTubeThickness() + tubeIns.GetBorderRadius()
            );
            Vector2 border2 = new Vector2(
               tubeIns.GetEndX() - tubeIns.GetTubeThickness() - tubeIns.GetBorderRadius(),
               border1.y
            );

            float ballRadius = gameManager.CurrentBall.GetPhysics().GetRadius();

            if (gameManager.SpawnXPos < border1.x || border2.x < gameManager.SpawnXPos)
            {
                float minDist = 100;
                Vector2 absP = Vector2.zero;

                float angleDt = 1f;
                bool isLeft = gameManager.SpawnXPos < border1.x;
                float startAngle = isLeft ? 180 : 270;
                float endAngle = isLeft ? 270 : 360;
                var borderCenter = isLeft ? border1 : border2;
                for (float angle = startAngle; angle <= endAngle; angle += angleDt)
                {
                    Vector2 localPos = new Vector2(tubeIns.GetBorderRadius() * Mathf.Cos(Mathf.Deg2Rad * angle), tubeIns.GetBorderRadius() * Mathf.Sin(Mathf.Deg2Rad * angle));
                    Vector2 absPos = borderCenter + localPos;
                    float dist = Vector2.Distance(new Vector2(gameManager.SpawnXPos, yPosition), absPos);
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
            foreach (var ball in gameManager.AllBalls)
            {
                if (Utils.CheckCirclesOverlap(ball.transform.position, ball.GetPhysics().GetRadius(), new Vector2(gameManager.SpawnXPos, midY), gameManager.CurrentBall.GetPhysics().GetRadius()))
                    overlaps = true;

            }
            if (overlaps)
                endY = midY;
            else
                startY = midY;
        }
        return new Vector2(gameManager.SpawnXPos, (startY + endY) / 2);
    }

    private int GenerateRandomBallNumber(int maxNum = 2048)
    {
        float threshold = 1 - (1.0f / maxNum);
        float randomNum = Random.Range(0, threshold);
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
}

public static class Utils
{
    public static bool CheckCirclesOverlap(Vector2 pos1, float radius1, Vector2 pos2, float radius2)
    {
        float dist = Vector2.Distance(pos1, pos2);
        return dist < radius1 + radius2;
    }
}
