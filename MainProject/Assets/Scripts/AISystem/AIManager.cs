using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : Singleton<AIManager>
{
    //Area for static vars used by AI systems
    public static float tgtClosest = 0.35f;
    public static float tgtFarthest = 0.5f;
    public static float tgtStrongest = 0.75f;
    public static float tgtWeakest = 0.85f;

    public static float cmpWeapon = 0.25f;
    public static float cmpDefence = 0.25f;
    public static float cmpEngineering = 0.25f;
    public static float cmpSupport = 0.25f;
}
