/*
  NavTester.cs
  Mission: Invasion
  Created by Rohun Banerji on March 31, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavTester : MonoBehaviour 
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private float radius = 5.0f;

    //private NavMeshAgent agent;
    private Vector3 destination;
    private Vector3 gizmoDest;
    private Transform trans;
    private bool moving = false;

    private void Start()
    {
        trans = transform;
        destination = trans.position;
        //agent = GetComponent<NavMeshAgent>();
        SpaceGround.Instance.OnGroundClick += OnGroundClick;
    }

    void OnGroundClick(Vector3 worldPosition)
    {
        //Instantiate(bulletPrefab, worldPosition, Quaternion.identity);
        //agent.SetDestination(worldPosition);
        //NavMeshHit hit;
        //NavMeshPath path=new NavMeshPath();

        ////if(agent.Raycast(worldPosition, out hit))
        //if(agent.CalculatePath(worldPosition,  path))
        //{
        //    Debug.Log("can reach " +path.status +" pos "+ worldPosition);
        //}
        //else
        //{
        //    Debug.Log("Cannot reach " + worldPosition);
        //}
        gizmoDest = worldPosition;
        Collider[] hitColliders = Physics.OverlapSphere(worldPosition, radius);
        foreach (var item in hitColliders)
        {
            Debug.Log("col: " + item.name);
        }
        //colliding with ground or itself is valid
        if (hitColliders.Any(col => col.gameObject.layer != TagsAndLayers.SpaceGroundLayer && col.gameObject!=gameObject))
        {
            Debug.Log("Cannot move to " + worldPosition);
        }
        else
        {
            destination = worldPosition;
            if (!moving) StartCoroutine(MoveToDestination());
        }
    }
    private IEnumerator MoveToDestination()
    {
        moving = true;
        Vector3 moveDir = destination - trans.position;
        float mag = moveDir.magnitude;
        while(mag > GlobalVars.LerpDistanceEpsilon )
        {
            trans.position += moveDir / mag * moveSpeed * Time.deltaTime;
            moveDir = destination - trans.position;
            mag = moveDir.magnitude;
            yield return null;
        }
        trans.position = destination;
        moving = false;
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(destination, radius);
        Gizmos.DrawWireSphere(gizmoDest, radius);
    }

}
