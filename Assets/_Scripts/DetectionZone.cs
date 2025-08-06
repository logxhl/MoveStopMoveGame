using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    //public static DetectionZone instance;
    public GameObject hammerPrefab;
    public Transform throwPoint;
    public float throwForce = 20f;

    public GameObject handHammer;

    public bool isPlayer = true;
    private float respawnDelay = 0.5f;

    private bool canThrow = true;
    private List<Transform> enemiesInZone = new List<Transform>();
    private Queue<Transform> throwQueue = new Queue<Transform>();
    public Animator animPlayer;
    //private void Start()
    //{
    //    instance = this;
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayer) return;
        if (other.CompareTag("Enemy"))
        {
            Transform enemy = other.transform;

            if (!enemiesInZone.Contains(enemy))
            {
                enemiesInZone.Add(enemy);
                throwQueue.Enqueue(enemy);
                TryThrowNext();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayer) return;
        if (other.CompareTag("Enemy"))
        {
            Transform enemy = other.transform;

            enemiesInZone.Remove(enemy);

            Queue<Transform> tempQueue = new Queue<Transform>();
            while (throwQueue.Count > 0)
            {
                Transform e = throwQueue.Dequeue();
                if (e != enemy)
                {
                    tempQueue.Enqueue(e);
                }
            }
            throwQueue = tempQueue;
        }
    }
    private void TryThrowNext()
    {
        if (canThrow && throwQueue.Count > 0)
        {
            Transform target = throwQueue.Dequeue();
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                TryThrowNext();
                return;
            }
            StartCoroutine(ThrowHammerAtTarget(target.position));
        }
    }

    public bool CanAttack()
    {
        return canThrow;
    }

    public void ThrowHammerAt(Vector3 targetPosition)
    {
        if (!canThrow) return;
        StartCoroutine(ThrowHammerAtTarget(targetPosition));
    }

    private IEnumerator ThrowHammerAtTarget(Vector3 targetPosition)
    {
        canThrow = false;

        if (handHammer != null)
        {
            handHammer.SetActive(false);
        }

        if (animPlayer != null)
        {
            animPlayer.SetBool("IsIdle", false);
            animPlayer.SetBool("IsRun", false);
            animPlayer.SetBool("IsAttack", true);
        }

        yield return new WaitForSeconds(0.1f);


        GameObject hammer = Instantiate(hammerPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = hammer.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            Vector3 direction = (targetPosition - throwPoint.position).normalized;
            rb.velocity = direction * throwForce;
        }

        yield return new WaitForSeconds(respawnDelay);
        if (handHammer != null)
        {
            handHammer.SetActive(true);
        }
        if (animPlayer != null)
        {
            animPlayer.SetBool("IsAttack", false);
            animPlayer.SetBool("IsIdle", true);
        }

        yield return new WaitForSeconds(1f);

        canThrow = true;

        if(isPlayer)
        {
            TryThrowNext();
        }
    }
}
