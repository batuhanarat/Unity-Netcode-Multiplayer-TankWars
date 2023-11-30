using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{

    private static List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    private void OnEnable()
    {
        SpawnPoints.Add(this);   
    }

    private void OnDestroy()
    { 
        SpawnPoints.Remove(this);
    }

    public static Vector3 GetRandomSpawnPos()
    {
        if (SpawnPoints.Count > 0)
        {
            int index = Random.Range(0, SpawnPoints.Count);
            SpawnPoint point = SpawnPoints[index];
            return point.transform.position;

        }

        return Vector3.zero;


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,1);
    }
}
