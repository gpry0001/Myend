using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playercontrol : MonoBehaviour
{
    public float interactionDistance = 2.5f;
    public LayerMask interactableLayer;

    private Animator animator;  
    private Rigidbody2D playerRigidybody;
    private movement playerMovement;


    private bool canMove = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerRigidybody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<movement>();

        DialogueLoader.Instance.OnDialogueFinished += ResumePlayerMovement;
    }
    void Update()
    {
        if (canMove) 
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                TryInteract(); // 상호작용 시도 함수를 호출합니다.
            }
        }
        else 
        {
            playerRigidybody.velocity = Vector2.zero;

            animator.SetBool("isMove", false);

            if (DialogueLoader.Instance != null && DialogueLoader.Instance.dialoguePanel.activeSelf)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    DialogueLoader.Instance.AdvanceDialogue();
                }
            }
        }
    }
    void TryInteract()
    {
        Vector2 rayOrigin = transform.position; 
        Vector2 rayDirection = GetFacingDirection();

        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red, 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, interactionDistance, interactableLayer);

        if (hit.collider != null) 
        {
            npcid npcTrigger = hit.collider.GetComponent<npcid>();
            if (npcTrigger != null) 
            {
                if (playerMovement != null)
                {
                    playerMovement.isTalking = true; // 대화 시작 시 이동을 막음
                }
                canMove = false; 
                npcTrigger.OnPlayerInteract(); 
            }
        }
    }
    Vector2 GetFacingDirection()
    {
        if (animator == null) return Vector2.up; 

        float lastX = animator.GetFloat("lastX");
        float lastY = animator.GetFloat("lastY");

        if (lastX == 0 && lastY == 0) return Vector2.up;

        return new Vector2(lastX, lastY).normalized;
    }
    void ResumePlayerMovement()
    {
        canMove = true;
    }
    void OnDestroy()
    {
        if (DialogueLoader.Instance != null)
        {
            DialogueLoader.Instance.OnDialogueFinished -= ResumePlayerMovement;
        }
    }
}
