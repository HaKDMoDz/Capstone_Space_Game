using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class FogOfWar : MonoBehaviour 
{
    private float fadeTimer = 1000.0f;

    public IEnumerator FadeFog()
    {
        fadeTimer = 1000.0f;
        while(fadeTimer > 0.0f)
        {
            fadeTimer -= 25f;

            Color currColor = renderer.material.color;
            currColor.a = (fadeTimer / 1000.0f) * renderer.material.color.a;

            renderer.material.color = currColor;
        }

        renderer.enabled = false;
        Destroy(gameObject);
        yield return 0;
    }
}
