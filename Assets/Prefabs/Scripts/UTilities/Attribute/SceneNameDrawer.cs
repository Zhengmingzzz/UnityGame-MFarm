using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    int sceneIndex = -1;
    GUIContent[] sceneNames;
    string[] splitString = { "/", ".unity" };


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        //判断buildSetting的场景数是否为0
        if (EditorBuildSettings.scenes.Length == 0) return;
        if (sceneIndex == -1)
        {
            GetSceneNameArray(property);
        }

        int oldSceneIndex = sceneIndex;
        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);

        if (oldSceneIndex != sceneIndex)
        {
            property.stringValue = sceneNames[sceneIndex].text;
        }
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;

        sceneNames = new GUIContent[scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            string path = scenes[i].path;

            string[] splitPath = path.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";
            if (splitPath.Length > 0)
            {
                sceneName = splitPath[splitPath.Length - 1];
            }
            else
            {
                sceneName = "(Deleted Scene)";
            }

            sceneNames[i] = new GUIContent(sceneName);
        }

        if (sceneNames.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Check Your BuildSettings") };
        }

        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameIsFound = false;

            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameIsFound = true;
                    break;
                }
            }

            if (nameIsFound == false)
            {
                sceneIndex = 0;
            }
        }
        else
        {
            sceneIndex = 0;
        }

        property.stringValue = sceneNames[sceneIndex].text;


    }

    
}
