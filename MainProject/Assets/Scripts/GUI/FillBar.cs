/*
  FillBar.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 28/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FillBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;
    private float fillSpeed = 10.0f;

    private float targetValue;

    public void SetValue(float value, bool lerp)
    {
#if FULL_DEBUG
        if (value < 0.0f || value > 1.0f)
        {
            Debug.LogWarning("Value should be between 0 and 1 but is " + value);
            return;
        }
#endif
        //Debug.Log("Value " + value);
        //fillImage.fillAmount = value;
        //Debug.Log("enabled " + this.enabled);
        if (lerp && gameObject.activeInHierarchy)
        {
            targetValue = value;
            StopCoroutine("LerpToTargetValue");
            StartCoroutine("LerpToTargetValue");
        }
        else
        {
            fillImage.fillAmount = value;
        }
    }

    public void ChangeValue(float delta, bool lerp)
    {
#if FULL_DEBUG
        float currentVal = fillImage.fillAmount;
        float newVal = currentVal + delta;
        if (newVal < -float.Epsilon || newVal > 1.0f + float.Epsilon)
        {
            Debug.LogWarning("Value would be outside the 0 - 1 range if incremented by " + delta + "current value: " + currentVal + " result: "+newVal);
        }
#endif
        //fillImage.fillAmount += delta;
        SetValue(fillImage.fillAmount + delta, lerp);
    }

    public void SetFillColour(Color fillColour)
    {
        fillImage.color = fillColour;
    }

    private IEnumerator LerpToTargetValue()
    {
        float currentVal = fillImage.fillAmount;
        while (Mathf.Abs(currentVal - targetValue) > float.Epsilon)
        {
            currentVal = Mathf.Lerp(currentVal, targetValue, fillSpeed * Time.deltaTime);
            fillImage.fillAmount = currentVal;
            yield return null;
        }
        fillImage.fillAmount = targetValue;
    }
}
