using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolTester : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    Material mat;

    //internal fields
    private Transform trans;

    private void Start()
    {
        InputManager.Instance.RegisterMouseButtonsDown(Shoot, MouseButton.Left);
        FindObjectOfType<SpaceGround>().OnGroundClick += GroundClick;
        trans = transform;
        //AudioManager.Instance.SetMainTrack(Sound.TestTrack);
        GameObject obj = new GameObject("Arc");
        ArcMesh arc = obj.AddComponent<ArcMesh>();
        arc.BuildArc(1.0f, 45.0f, 20, mat);
        
    }

    void GroundClick(Vector3 worldPosition)
    {
        //Debug.Log("ground click");
        //AudioManager.Instance.PlayEffect(Sound.LaserBeam, worldPosition);
    }

    private void Shoot(MouseButton button)
    {
        GameObject currentBullet = ObjectPool.Instance.GetPooledObject(bulletPrefab, false);
        currentBullet.transform.position = trans.position;
        //AudioManager.Instance.PlayEffectAndAttachTo(Sound.Laser, currentBullet.transform);
        currentBullet.rigidbody.AddForce(trans.forward * 300.0f);
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
