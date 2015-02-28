using UnityEngine;
using System.Collections;

public class RotateRing : MonoBehaviour 
{
    private float rotationSpeed = -0.02f;

    public IEnumerator Rotate(float _delay)
    {
        transform.Rotate(0, 0, rotationSpeed, Space.Self);

        yield return new WaitForSeconds(_delay);
        yield return StartCoroutine(Rotate(_delay));
    }

    public void Awake()
    {
        //transform.Rotate(0, Random.Range(0, 360), 0, Space.Self);
        StartCoroutine(Rotate(0.016f));
    }
}
