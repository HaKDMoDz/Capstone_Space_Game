using UnityEngine;
using System.Collections;

public class DisableEffectAfterTime : MonoBehaviour 
{

	public IEnumerator StartEffect()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        StopEffect();
    }

    public void StopEffect()
    {
        gameObject.SetActive(false);
    }
}
