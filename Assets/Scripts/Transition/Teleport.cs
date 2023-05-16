using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Transition
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string newSceneName;
        public Vector3 newScenePos;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                EventHandler.CallUpTransitionSceneEvent(newSceneName, newScenePos);
            }
        }
    }

}