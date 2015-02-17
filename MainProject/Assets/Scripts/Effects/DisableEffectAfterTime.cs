using UnityEngine;
using System.Collections;

public class DisableEffectAfterTime : MonoBehaviour 
{

	public IEnumerator StartEffect()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(StopEffect());
    }

    public IEnumerator StopEffect()
    {
        gameObject.SetActive(false);
        yield return null;
    }
}
