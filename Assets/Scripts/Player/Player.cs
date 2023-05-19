using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    private float InputX;
    private float InputY;
    private Vector2 movementInput;

    Animator[] animator;
    bool isMoving;

    bool isChangeScene = false;


    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponentsInChildren<Animator>();

    }



    private void FixedUpdate()
    {
        if (!isChangeScene)
            playerInput();
        else
        {
            isMoving = false;
            movementInput = Vector2.zero;
        }
        SwitchAnimation();
        Movement();
    }

    private void OnEnable()
    {
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.MoveToNewSceneEvent += OnMoveToNewSceneEvent;

    }



    private void OnDisable()
    {
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.MoveToNewSceneEvent -= OnMoveToNewSceneEvent;

    }


    private void OnBeforeUnLoadSceneEvent()
    {
        isChangeScene = true;
    }
    private void OnAfterLoadSceneEvent()
    {
        isChangeScene = false;
    }
    private void OnMoveToNewSceneEvent(Vector3 newScenePos)
    {
        this.transform.position = newScenePos;
    }


    private void playerInput()
    {
        InputX = Input.GetAxisRaw("Horizontal");
        InputY = Input.GetAxisRaw("Vertical");

        if(InputX != 0 && InputY != 0)
        {
            InputX *= 0.6f;
            InputY *= 0.6f;
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            InputX *= 0.5f;
            InputY *= 0.5f;
        }
        movementInput = new Vector2(InputX, InputY);

        isMoving = movementInput != Vector2.zero;

        
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }


    private void SwitchAnimation()
    {
        foreach (var a in animator)
        {
            a.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                a.SetFloat("InputX", InputX);
                a.SetFloat("InputY", InputY);
            }
        }
    }


}
