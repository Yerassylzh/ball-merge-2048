using UnityEngine;

public class ObjectSwitch : MonoBehaviour
{
    [SerializeField] GameObject OnObj;
    [SerializeField] GameObject OffObj;
    [SerializeField] bool initialState = true;

    private bool currentState;

    public void SwitchObjs()
    {
        currentState = !currentState;
        UpdateObjStates();
    }

    public void SetState(bool state)
    {
        currentState = state;
        UpdateObjStates();
    }

    void Awake()
    {
        currentState = initialState;
        UpdateObjStates();
    }

    void UpdateObjStates()
    {
        OnObj.SetActive(currentState == true);
        OffObj.SetActive(currentState == false);
    }
}
