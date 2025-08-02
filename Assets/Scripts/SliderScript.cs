using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection;

public class SliderScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Slider slider;
    public UnityEvent<float> onValueChanged;
    public UnityEvent onSliderPressed;
    public UnityEvent onSliderReleased;

    private void Awake()
    {
        slider.onValueChanged.AddListener(onValueChanged.Invoke);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onSliderPressed.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onSliderReleased.Invoke();
    }
}