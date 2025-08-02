using System;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    [SerializeField] private Color[] colors;

    private SpriteRenderer spriteRenderer;

    public void SetColorByIndex(int index)
    {
        index = Math.Min(index, colors.Length - 1);
        spriteRenderer.color = colors[index];
    }

    public void SetOpacity(float alpha)
    {
        var spriteR = GetComponent<SpriteRenderer>();
        spriteR.color = new Color(spriteR.color.r, spriteR.color.g, spriteR.color.b, alpha);
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}