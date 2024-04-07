using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


public class NPC_Manager : Singleton<NPC_Manager>
{
    public List<NPC_Position> npcDateList = new List<NPC_Position>();
    /// <summary>
    /// 记录了NPC从一个场景走到另一个场景的路径的SO文件
    /// </summary>
    public SceneRouteDataList_SO npcRoutes_SO;
    /// <summary>
    /// 将from和to的场景和路径相关联用于查找场景路径
    /// </summary>
    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }
    private void InitSceneRouteDict()
    {
        foreach (SceneRoute route in npcRoutes_SO.sceneRouteList)
        {
            string key = route.fromSceneName + route.toSceneName;
            if (sceneRouteDict.ContainsKey(key))
                continue;
            sceneRouteDict.Add(key, route);
        }
    }
    /// <summary>
    /// 查找异场景移动的路径
    /// </summary>
    /// <param name="fromSceneName"></param>
    /// <param name="toSceneName"></param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        if (sceneRouteDict.ContainsKey(fromSceneName + toSceneName))
        {
            return sceneRouteDict[fromSceneName + toSceneName];
        }
        Debug.Log("字典中不包含" + fromSceneName + "到" + toSceneName);
        return null;
    }

}
