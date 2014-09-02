using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDirector : SingletonComponent<CameraDirector> 
    //public class CameraDirector : MonoBehaviour
{

    [SerializeField] Transform sceneStartPos;
    [SerializeField] float movementEpsilon = 0.2f;
    [SerializeField]
    float heightAboveFocusTarget=200f;


    float initialAngleX;
    Transform trans;

    //void Awake()
    //{
    //    trans = transform;
        
    //}
    protected override void Awake()
    {
        base.Awake();
        trans = transform;
        initialAngleX = Mathf.Deg2Rad* trans.rotation.eulerAngles.x;
    }
    public IEnumerator MoveTo(Vector3 destination, float period)
    {
        float time = 0f;
        Vector3 startPos = trans.position;
        while(time<1f)
        {
            trans.position = Vector3.Lerp(startPos, destination, time);
            time += Time.deltaTime / period;
            yield return null;
        }
    }
    public IEnumerator LerpTo(Vector3 destination, float period)
    {
        yield return null;
    }

    public IEnumerator FocusOn(Transform target, float period)
    {
        Vector3 targetPos = target.position;
        targetPos.y += heightAboveFocusTarget;
        Debug.Log(initialAngleX);
        Debug.Log(Mathf.Tan(initialAngleX));

        targetPos.z -= heightAboveFocusTarget / Mathf.Tan( initialAngleX);

        yield return StartCoroutine(MoveTo(targetPos, period));
    }
	
}
