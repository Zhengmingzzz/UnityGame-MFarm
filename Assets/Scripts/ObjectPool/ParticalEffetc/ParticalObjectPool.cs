using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticalObjectPool : MonoBehaviour
{
    public List<S_ParticalEffect> poolPrefabList;
    private Dictionary<E_PESType, ObjectPool<GameObject>> ParticalPrefabsDic = new Dictionary<E_PESType, ObjectPool<GameObject>>();


    private void Start()
    {
        CreatePool();
    }
    private void OnEnable()
    {
        EventHandler.PEInstantiateEvent += OnPEInstantiateEvent;
    }
    private void OnDisable()
    {
        EventHandler.PEInstantiateEvent -= OnPEInstantiateEvent;
    }

    private void OnPEInstantiateEvent(E_PESType PESType, Vector3 pos)
    {
        StartCoroutine(PESProgress(PESType, pos));
    }


    IEnumerator PESProgress(E_PESType PESType, Vector3 pos)
    {
        GameObject g = ParticalPrefabsDic[PESType].Get();
        g.transform.position = pos;

        yield return new WaitForSeconds(2f);

        ParticalPrefabsDic[PESType].Release(g);
    

    }
    private void CreatePool()
    {
        foreach (S_ParticalEffect prefab in poolPrefabList)
        {
            Transform parent = new GameObject(Convert.ToString(prefab.E_particalSystem.ToString())).transform;
            parent.SetParent(transform);


            ObjectPool<GameObject> newObjectPool = new ObjectPool<GameObject>(

                () => Instantiate(prefab.ParticalEffectPrefab, parent),
                e =>  e.SetActive(true) ,
                e =>  e.SetActive(false),
                e =>  Destroy(e)
                );

            ParticalPrefabsDic.Add(prefab.E_particalSystem, newObjectPool);
        }
    }





    






}
