using System;
using UnityEngine;


public class BallPhysics : MonoBehaviour
{
    [SerializeField] private float startingBallRadiusScale = 0.7f;
    [SerializeField] private float ballRadiusScaleDelta = 0.2f;
    public Action<Ball> OnBallDestroyed;


    public void SetRadiusByIndex(int index)
    {
        float scale = startingBallRadiusScale + index * ballRadiusScaleDelta;
        transform.localScale = new Vector2(scale, scale);
    }

    public void DisablePhysics()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.simulated = false;
    }

    public void EnablePhysics()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.simulated = true;
    }

    public float GetRadius()
    {
        var circle = GetComponent<CircleCollider2D>();
        return circle.radius * transform.localScale.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Ball>(out var otherBall)) return;

        this.gameObject.GetComponent<Ball>().Merge(otherBall);
    }
}