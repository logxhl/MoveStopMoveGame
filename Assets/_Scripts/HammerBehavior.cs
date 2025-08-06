using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehavior : MonoBehaviour
{
    private string detectionZoneTag = "DetectionZone";
    private string enemyTag = "Enemy";
    private string playerTag = "Player";
    private Transform detectionZoneCenter;
    private float detectionRadius = 0f;
    public string ownerTag;
    // Start is called before the first frame update
    void Start()
    {
        GameObject zone = GameObject.FindGameObjectWithTag(detectionZoneTag);
        if(zone != null)
        {
            detectionZoneCenter = zone.transform;
            SphereCollider col = zone.GetComponent<SphereCollider>();
            if(col != null)
            {
                detectionRadius = col.radius * zone.transform.lossyScale.x;
            }
        }
        else
        {
            Debug.LogWarning("No DetectionZone found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(detectionZoneCenter != null)
        {
            float distane = Vector3.Distance(transform.position, detectionZoneCenter.position);
            if(distane > detectionRadius)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(ownerTag == playerTag && other.CompareTag(enemyTag))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        else if (ownerTag == enemyTag && other.CompareTag(playerTag))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
