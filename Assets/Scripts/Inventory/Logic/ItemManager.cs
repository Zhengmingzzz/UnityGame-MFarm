using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        
        public RenderItem itemPrefab;
        private Transform itemParent;
        private Dictionary<string, List<sceneItems>> scenesItemsDic = new Dictionary<string, List<sceneItems>>();




        private void OnEnable()
        {
            
            EventHandler.InstantiateItemInScene += OnInstantiateInScene;
            EventHandler.BeforeUnLoadSceneEvent += OnUnLoadSceneEvent;
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        }

        

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateInScene;
            EventHandler.BeforeUnLoadSceneEvent -= OnUnLoadSceneEvent;
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;

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
            Debug.Log(itemID+"Instantiate");

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








    }
}