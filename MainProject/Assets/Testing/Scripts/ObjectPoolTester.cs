using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolTester : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;


    //internal fields
    private Transform trans;
    
    private void Start()
    {
        InputManager.Instance.RegisterMouseButtonsDown(Shoot, MouseButton.Left);
        trans = transform;
    }

    private void Shoot(MouseButton button)
    {
       GameObject currentBullet = ObjectPool.Instance.GetPooledObject(bulletPrefab, false);
        currentBullet.transform.position = trans.position;
        currentBullet.rigidbody.AddForce(trans.forward * 100.0f);
        StartCoroutine(currentBullet.GetSafeComponent<TimedAction>().SetTimedAction(2.0f,
            () =>
            {
                //Debug.Log(currentBullet.gameObject.name+" stop and pool ");
                currentBullet.rigidbody.ResetMovement();
                ObjectPool.Instance.PoolObject(bulletPrefab, currentBullet);
            }
            ));
    }
}
