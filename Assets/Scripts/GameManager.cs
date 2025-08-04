using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] public Ball ballPrefab;
    [SerializeField] public GameObject tube;
    [SerializeField] public TextMeshProUGUI currentScoreText;
    [SerializeField] public TextMeshProUGUI bestScoreText;
    [SerializeField] public AudioSource collisionAudioSource;

    [Header("Game Configuration")]
    [SerializeField] public float ballSpawnBottomPadding = 0.9f;

    // --- Private State ---
    private List<Ball> balls = new List<Ball>();
    private Ball currentBall;
    private Ball previewBall;
    private bool isPressing = false;
    private Vector2 prevPressPos;
    private Vector2 currentPressPos;
    private float spawnXPos;
    private float spawnYPos;

    // --- Public Accessors (Getters) ---
    public IReadOnlyList<Ball> AllBalls => balls;
    public Ball CurrentBall => currentBall;
    public Ball PreviewBall => previewBall;
    public bool IsPressing => isPressing;
    public Vector2 PrevPressPos => prevPressPos;
    public Vector2 CurrentPressPos => currentPressPos;
    public float SpawnXPos => spawnXPos;
    public float SpawnYPos => spawnYPos;

    // --- Public Mutators (Setters) ---
    public void SetCurrentBall(Ball ball) => currentBall = ball;
    public void SetPreviewBall(Ball ball) => previewBall = ball;
    public void SetIsPressing(bool pressing) => isPressing = pressing;
    public void SetPressPositions(Vector2 current, Vector2 previous)
    {
        currentPressPos = current;
        prevPressPos = previous;
    }
    public void SetSpawnPosition(float x, float y)
    {
        spawnXPos = x;
        spawnYPos = y;
    }

    public void AddBall(Ball ball)
    {
        balls.Add(ball);
    }

    public void RemoveBall(Ball ball)
    {
        if (balls.Contains(ball))
        {
            balls.Remove(ball);
        }
    }
}
