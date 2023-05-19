using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        [Header("Prefabs")]
        public RenderItem itemPrefab;
        public RenderItem DroppedItemPrefab;

        private Transform itemParent;
        private Dictionary<string, List<sceneItems>> scenesItemsDic = new Dictionary<string, List<sceneItems>>();

        private Transform playerTransform => FindObjectOfType<Player>().transform;


        private void OnEnable()
        {
            
            EventHandler.InstantiateItemInScene += OnInstantiateInScene;
            EventHandler.BeforeUnLoadSceneEvent += OnUnLoadSceneEvent;
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
        }

        

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateInScene;
            EventHandler.BeforeUnLoadSceneEvent -= OnUnLoadSceneEvent;
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;

        }


        private void OnUnLoadSceneEvent()
        {
            GetAllItemsInScenes();
        }
        public void OnAfterLoadSceneEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            LoadSceneItems();
        }

        public void OnInstantiateInScene(int itemID, Vector3 ItemPos)
        {

            var item = Instantiate(itemPrefab, ItemPos, Quaternion.identity, itemParent);

            item.ItemID = itemID;
        }



        private void GetAllItemsInScenes()
        {

            List<sceneItems> currentsceneItems = new List<sceneItems>();
            foreach (var i in FindObjectsOfType<RenderItem>())
            {
                sceneItems items = new sceneItems
                {
                    itemID = i.ItemID,
                    itemPos = new SerializedVector3(i.transform.position)
                };
                currentsceneItems.Add(items);
            }

            if (scenesItemsDic.ContainsKey(SceneManager.GetActiveScene().name))
            {
                scenesItemsDic[SceneManager.GetActiveScene().name] = currentsceneItems;
            }
            else
            {
                scenesItemsDic.Add(SceneManager.GetActiveScene().name, currentsceneItems);
            }
        }

        private void LoadSceneItems()
        {
            //得到场景列表的物品
            List<sceneItems> currentsceneItems = new List<sceneItems>();

            if (scenesItemsDic.TryGetValue(SceneManager.GetActiveScene().name, out currentsceneItems))
            {
                if (currentsceneItems != null)
                {
                    //清除场景所有物品
                    foreach (var i in FindObjectsOfType<RenderItem>())
                    {
                        Destroy(i.gameObject);
                    }

                    //加载列表中的物品
                    foreach (var i in currentsceneItems)
                    {

                        OnInstantiateInScene(i.itemID, i.itemPos.ToVector3());                    
                    }
                }
                else
                {
                    GetAllItemsInScenes();
                }
            }




        }

        private void OnDropItemEvent(int ItemID, Vector3 GridPos)
        {
            //TODO:实现扔东西效果
            var item = Instantiate(DroppedItemPrefab, playerTransform.position, Quaternion.identity, itemParent);
            item.ItemID = ItemID;

            var direction = (GridPos - playerTransform.position).normalized;

            item.GetComponent<DroppedItem>().InitDroppedItem(GridPos, direction);
        }


    }
}