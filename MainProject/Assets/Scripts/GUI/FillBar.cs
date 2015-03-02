using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FillBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;

    public void SetValue(float value)
    {
#if FULL_DEBUG
        if (value < 0.0f || value > 1.0f)
        {
            Debug.LogWarning("Value should be between 0 and 1 but is " + value);
            return;
        }
#endif
        //Debug.Log("Value " + value);
        fillImage.fillAmount = value;
    }

    public void ChangeValue(float delta)
    {
#if FULL_DEBUG
        float currentVal = fillImage.fillAmount;
        float newVal = currentVal + delta;
        if (newVal < -float.Epsilon || newVal > 1.0f + float.Epsilon)
        {
            Debug.LogWarning("Value would be outside the 0 - 1 range if incremented by " + delta + "current value: " + currentVal + " result: "+newVal);
        }
#endif
        fillImage.fillAmount += delta;
    }

    public void SetFillColour(Color fillColour)
    {
        fillImage.color = fillColour;
    }

}
