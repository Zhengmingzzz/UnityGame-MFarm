using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemShake : MonoBehaviour
{
    private Transform playerTransform => FindObjectOfType<Player>().transform;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO:添加NPC需要实现物品晃动逻辑时在此添加
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(shake());
        }
    }


    private IEnumerator shake()
    {
        if (playerTransform.position.x > this.transform.position.x)
        {
            Debug.Log(1);
            this.transform.DORotate(new Vector3(0, -7, 1), Settings.ItemShakeTime);
        }
        else
        {
            Debug.Log(2);
            this.transform.DORotate(new Vector3(0, 7, -1), Settings.ItemShakeTime);

        }
        yield return new WaitForSeconds(Settings.ItemShakeTime);
        this.transform.DORotate(new Vector3(0, 0, 0), Settings.ItemShakeTime) ;
    }
}
