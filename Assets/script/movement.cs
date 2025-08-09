using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    private Rigidbody2D playerRigidybody;
    public float speed = 8f;
    private Animator animator;

    private Vector2 lastMoveDir;


    public bool isTalking = false;
    void Start()
    {
        playerRigidybody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = input.sqrMagnitude > 0;

        Vector2 animDir = Vector2.zero;

        if (input.x != 0) // XÃà ¿ì¼±
        {
            animDir = new Vector2(input.x, 0);
        }
        else if (input.y != 0)
        {
            animDir = new Vector2(0, input.y);
        }

        if (isMoving)
        {
            animator.SetFloat("moveX", animDir.x);
            animator.SetFloat("moveY", animDir.y);
            animator.SetFloat("lastX", animDir.x);
            animator.SetFloat("lastY", animDir.y);
        }
        else
        {
            animator.SetFloat("moveX", animator.GetFloat("lastX"));
            animator.SetFloat("moveY", animator.GetFloat("lastY"));
        }

        animator.SetBool("isMove", isMoving);
    }

    void FixedUpdate()
    {
        if (isTalking) {
            playerRigidybody.velocity = Vector2.zero;
            return; 
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        playerRigidybody.velocity = input * speed;
    }
}
