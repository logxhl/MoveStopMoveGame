using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerJoystick : MonoBehaviour
{
    public float speed = 5f;
    public Animator anim;
    public Rigidbody rb;
    public FixedJoystick joystick;
    private Vector3 movement;
    public GameObject deathScene;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        movement = new Vector3(horizontal, 0f, vertical);
        transform.Translate(movement.normalized * speed * Time.deltaTime, Space.World);
        bool isMoving = movement.magnitude > 0.1f;
        anim.SetBool("IsRun", isMoving);
        anim.SetBool("IsIdle", !isMoving);
        if (isMoving)
        {
            MovePlayer();
        }
    }
    void MovePlayer()
    {
        if (movement.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, 0.1f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Boome"))
        {
            deathScene.SetActive(true);
            SpawnEnemy.Instance.canSpawn = false;
            //Destroy(other.gameObject);
            //Destroy(gameObject);
            //Time.timeScale = 0f;
        }
    }


}