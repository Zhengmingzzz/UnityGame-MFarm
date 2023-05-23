using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    private float InputX;
    private float InputY;
    private Vector2 movementInput;

    Animator[] animators;
    bool isMoving;

    bool isChangeScene = false;

    //播放动画参数
    private float mouseX;
    private float mouseY;
    private bool isUseingTool;


    private bool mouseDisable = false;


    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animators = this.GetComponentsInChildren<Animator>();

    }



    private void FixedUpdate()
    {
        if (!isChangeScene && !mouseDisable)
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
        EventHandler.mouseClickedEvent += OnMouseClickedEvent;

    }



    private void OnDisable()
    {
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.MoveToNewSceneEvent -= OnMoveToNewSceneEvent;
        EventHandler.mouseClickedEvent -= OnMouseClickedEvent;

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
        foreach (var a in animators)
        {
            a.SetBool("IsMoving", isMoving);

            if (isUseingTool)
            {
                a.SetFloat("InputX", mouseX);
                a.SetFloat("InputY", mouseY);
            }

            if (isMoving)
            {
                a.SetFloat("InputX", InputX);
                a.SetFloat("InputY", InputY);
            }
        }
    }
    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails clickedItemDetail)
    {
        if (EventSystem.current!=null&&!EventSystem.current.IsPointerOverGameObject())
        {
            //TODO:播放执行动画
            //播放使用工具动画
            if (clickedItemDetail.itemType != ItemType.Commodity && clickedItemDetail.itemType != ItemType.Furniture && clickedItemDetail.itemType != ItemType.Seed)
            {
                StartCoroutine(useTool(mouseWorldPos, clickedItemDetail));
            }
            else
            {
                //执行实际产生结果
                EventHandler.CallUpExecuteActionAfterAnimation(mouseWorldPos, clickedItemDetail);

            }
        }

    }

    IEnumerator useTool(Vector3 mouseWorldPos, ItemDetails clickedItemDetail)
    {
        if (isUseingTool)
            yield break;
        mouseDisable = true;
        isUseingTool = true;
        mouseX = mouseWorldPos.x - this.transform.position.x;
        mouseY = mouseWorldPos.y - this.transform.position.y;

        if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
        {
            mouseY = 0;
        }
        else
        {
            mouseX = 0;
        }


        yield return null;



        foreach (var anim in animators)
        {
            anim.SetTrigger("isUseingTool");
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallUpExecuteActionAfterAnimation(mouseWorldPos, clickedItemDetail);
        yield return new WaitForSeconds(0.25f);

        isUseingTool = false;
        mouseDisable = false;

    }

}
