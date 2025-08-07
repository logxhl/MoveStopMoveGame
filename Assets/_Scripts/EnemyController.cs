using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxDistance = 5f;
    public float minDistance = 2f;
    public float speed = 3f;
    public Transform player;
    public DetectionZone detectionZone;
    private Animator anim;

    private bool isAttacking = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        detectionZone = GetComponentInChildren<DetectionZone>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > maxDistance)
        {
            StopMoving();
            return;
        }
        else if(distance > minDistance)
        {
            MoveToPlayer();
        }
        else
        {
            StopMoving();
            if(detectionZone != null && detectionZone.CanAttack())
            {
                detectionZone.ThrowHammerAt(player.position);
                anim.SetBool("IsAttack", true);
            }
        }

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if(direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        }
    }
    /// <summary>
    /// Stop Moving Enemy
    /// </summary>
    private void StopMoving()
    {
        anim.SetBool("IsRun", false);
        anim.SetBool("IsIdle", true);
    }

    private void MoveToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        anim.SetBool("IsRun", true);
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsAttack", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,minDistance);
    }
}
