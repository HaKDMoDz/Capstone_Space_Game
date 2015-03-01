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
        if(value < 0.0f || value > 1.0f)
        {
            Debug.LogError("Value should be between 0 and 1 but is " + value);
            return;
        }
#endif
        fillImage.fillAmount = value;
    }

    public void ChangeValue(float delta)
    {
#if FULL_DEBUG
        float currentVal = fillImage.fillAmount;
        float newVal = currentVal - delta;
        //if(newVal < 0.0f || newVal > 1.0f)
        //{
        //    Debug.LogError("Value would be outside the 0 - 1 range if incremented by " + delta);
        //    return;
        //}
#endif
        fillImage.fillAmount += delta;
    }

}
