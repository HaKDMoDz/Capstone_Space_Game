using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyConfig : ScriptableObject 
{

    [SerializeField]
    private float systemAnimationPeriod = 5.0f;

    public static float SystemAnimationPeriod;

    private void OnEnable()
    {
        SystemAnimationPeriod = systemAnimationPeriod;
    }
	
}
