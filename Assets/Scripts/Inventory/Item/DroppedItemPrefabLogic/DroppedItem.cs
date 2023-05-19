using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private Transform itemSpriteTransform;
    private BoxCollider2D boxCollider;

    private Vector3 targetPosition;
    private Vector2 direction;

    private float distance;
    private bool isGround;
    public float gravity = -3.5f;

    private void Awake()
    {
        itemSpriteTransform = this.transform.GetChild(0).transform;
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
    }

  


    private void Update()
    {
        DroppedItemLogic();
    }


    public void InitDroppedItem(Vector3 targetPosition,Vector2 direction)
    {
        isGround = false;
        boxCollider.enabled = false;
        this.targetPosition = targetPosition;
        distance = Vector3.Distance(targetPosition, this.transform.position);
        this.direction = direction;

        itemSpriteTransform.position += Vector3.up * 1.5f;
    }

    private void DroppedItemLogic()
    {
        isGround = itemSpriteTransform.position.y <= this.transform.position.y;

        //横向移动
        if (Vector3.Distance(targetPosition, this.transform.position) > 0.1f)
        {
            this.transform.position += (Vector3)direction * -gravity * distance * Time.deltaTime;
        }

        //纵向移动判断
        if (isGround)
        {
            this.transform.position = targetPosition;
            itemSpriteTransform.position = targetPosition;

            boxCollider.enabled = true;
        }
        else
        {
            itemSpriteTransform.position += Vector3.up * gravity * Time.deltaTime;


        }
    }




}
