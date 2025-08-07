using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehavior : MonoBehaviour
{
    private string enemyTag = "Enemy";
    private string playerTag = "Player";

    [Header("Owner Settings")]
    public string ownerTag;
    public GameObject owner; 

    [Header("Distance Settings")]
    public float maxDistance = 2f;

    [Header("Debug")]
    public bool showDebugInfo = false;

    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Boome"), LayerMask.NameToLayer("DetectionZone"));
        if (owner == null)
        {
            Debug.LogError($"[{gameObject.name}] Owner not set! Call SetOwner() after Instantiate.");
        }
    }

    void Update()
    {
        CheckDistanceFromOwner();
    }

    void CheckDistanceFromOwner()
    {

        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }


        float distance = Vector3.Distance(transform.position, owner.transform.position);

        if (distance > maxDistance)
        {
            Destroy(gameObject);
        }

        //Debug visual
        if (showDebugInfo)
        {
            Debug.DrawLine(transform.position, owner.transform.position, Color.yellow);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("DetectionZone"))
        {
            if (showDebugInfo)
                Debug.Log($"[{gameObject.name}] ❌ Hit detection zone, ignored");
            return;
        }

        // Player projectile đánh Enemy
        if (ownerTag == playerTag && other.CompareTag(enemyTag))
        {
            if (showDebugInfo)
                Debug.Log($"[{gameObject.name}] ✅ PLAYER PROJECTILE HIT ENEMY: {other.name}");

            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        // Enemy projectile đánh Player
        else if (ownerTag == enemyTag && other.CompareTag(playerTag))
        {
            if (showDebugInfo)
                Debug.Log($"[{gameObject.name}] ✅ ENEMY PROJECTILE HIT PLAYER: {other.name}");

            // Trigger game over logic
            TriggerGameOver();

            Destroy(gameObject);
            Destroy(other.gameObject);
            //deathScene.SetActive(true);
        }
        else
        {
            // Debug không match
            if (showDebugInfo)
            {
                Debug.Log($"[{gameObject.name}] ❌ NO MATCH - Owner:'{ownerTag}' vs Target:'{other.tag}'");
            }
        }
    }

    void TriggerGameOver()
    {
        // Tìm death scene UI
        GameObject deathScene = GameObject.Find("DeathPanel");
        if (deathScene == null) deathScene = GameObject.Find("GameOverPanel");
        if (deathScene == null) deathScene = GameObject.Find("DeathScene");
        if (deathScene == null) deathScene = GameObject.Find("GameOverUI");

        if (deathScene != null)
        {
            deathScene.SetActive(true);
            Time.timeScale = 0f; // Pause game

            if (showDebugInfo)
                Debug.Log("Game Over triggered!");
        }
        else
        {
            Debug.LogWarning("Death scene UI not found! Create a GameObject named 'DeathPanel', 'GameOverPanel', or 'DeathScene'");
        }
    }

    // PUBLIC METHODS - Để script khác gọi

    /// <summary>
    /// Set owner khi spawn projectile - GỌI NGAY SAU KHI INSTANTIATE
    /// </summary>
    public void SetOwner(GameObject ownerObject)
    {
        if (ownerObject != null)
        {
            owner = ownerObject;
            ownerTag = ownerObject.tag;

            if (showDebugInfo)
            {
                Debug.Log($"[{gameObject.name}] Owner set to: {ownerObject.name} (Tag: {ownerTag})");
            }
        }
    }

    /// <summary>
    /// Set owner với custom tag
    /// </summary>
    public void SetOwner(GameObject ownerObject, string customTag)
    {
        owner = ownerObject;
        ownerTag = customTag;

        if (showDebugInfo)
        {
            Debug.Log($"[{gameObject.name}] Owner set to: {ownerObject.name} with custom tag: {customTag}");
        }
    }

    /// <summary>
    /// Set khoảng cách tối đa
    /// </summary>
    public void SetMaxDistance(float distance)
    {
        maxDistance = distance;
    }

    // DEBUG GIZMOS
    void OnDrawGizmosSelected()
    {
        if (owner != null)
        {
            // Vẽ line từ projectile đến owner
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, owner.transform.position);

            // Vẽ sphere radius xung quanh owner
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(owner.transform.position, maxDistance);

            // Hiển thị khoảng cách hiện tại
            float currentDistance = Vector3.Distance(transform.position, owner.transform.position);
            Gizmos.color = currentDistance > maxDistance ? Color.red : Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}