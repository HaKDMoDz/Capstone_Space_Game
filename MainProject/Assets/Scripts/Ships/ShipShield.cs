using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipShield : MonoBehaviour 
{
    [SerializeField]
    private float effectDuration = 1.0f;
    [SerializeField]
    private float effectSpeed = 0.1f;
    
    private Material shieldMat;

    private Transform trans;

    public void TakeDamage(Vector3 hitPoint)
    {
        Vector3 localHitPoint = trans.InverseTransformPoint(hitPoint);
        Debug.Log("Hit Point: " + hitPoint + " Local: " + localHitPoint);
        shieldMat.SetVector("_Position", localHitPoint.GetVector4());
        StartCoroutine(ShieldEffect());
    }

    private IEnumerator ShieldEffect()
    {
        float currentTime = 0.0f;
        while(currentTime <= effectDuration)
        {
            shieldMat.SetFloat("_Offset", Mathf.Repeat(currentTime*effectSpeed, 1.0f));
            currentTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        trans = transform;
        shieldMat = renderer.material;
    }

}
