using UnityEngine;
using Vec3 = UnityEngine.Vector3;

using Vec2 = UnityEngine.Vector2;
using TMPro;
using UnityEngine.UI;
public class BallUI : MonoBehaviour
{
    [SerializeField] private float maxFontSize = 0.4f;
    [SerializeField] private float fontSizeFactor = 0.06f;
    [SerializeField] private float minFontSize = 0.1f;
    [SerializeField] private float uiSizeFactor = 1.0f;

    private TextMeshProUGUI ballNumberText;
    private int ballNumber;

    public int GetNumber()
    {
        return ballNumber;
    }

    public void SetOpacity(float alpha)
    {
        ballNumberText.color = new Color(255, 255, 255, alpha);
    }

    public void SetBallNumber(int number)
    {
        ballNumber = number;
        if (ballNumberText != null)
        {
            ballNumberText.text = number.ToString();
            ballNumberText.text = number.ToString();
            int charCount = number.ToString().Length;
            ballNumberText.fontSize = Mathf.Clamp((float)(maxFontSize - (charCount - 1) * fontSizeFactor), minFontSize, maxFontSize);
        }
    }

    private void CreateUI()
    {
        GameObject canvasGO = new("SoftBallCanvas");
        canvasGO.transform.SetParent(transform);
        canvasGO.transform.localPosition = Vec3.zero;

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        if (Camera.main == null)
        {
            Debug.LogWarning("No main camera found at AddCanvas for SoftBallUI. Ensure your camera is tagged 'MainCamera'.", this);
        }
        canvas.worldCamera = Camera.main;

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vec2(uiSizeFactor, uiSizeFactor);

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var textGO = new GameObject("SoftBallLabel");
        textGO.transform.SetParent(canvasGO.transform);
        textGO.transform.localPosition = Vec3.zero;

        ballNumberText = textGO.AddComponent<TextMeshProUGUI>();
        ballNumberText.alignment = TextAlignmentOptions.Center;
        ballNumberText.rectTransform.sizeDelta = new Vec2(uiSizeFactor, uiSizeFactor);
        ballNumberText.enableAutoSizing = false;
        ballNumberText.outlineWidth = 0.03f;
        ballNumberText.outlineColor = new Color(0, 0, 0, 0.5f);
    }

    void Awake()
    {
        CreateUI();
    }
}