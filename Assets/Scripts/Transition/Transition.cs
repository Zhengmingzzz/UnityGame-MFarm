using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Transition
{
    public class Transition : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup loadingCanvasGroup;
        /// <summary>
        /// 是否正在切换场景
        /// </summary>
        private bool isFading;

        private void Awake()
        {
            StartCoroutine(loadSceneSetActive(startSceneName));
        }

        private void Start()
        {
            loadingCanvasGroup = FindObjectOfType<CanvasGroup>();
        }

        private void OnEnable()
        {
            EventHandler.TransitionSceneEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionSceneEvent -= OnTransitionEvent;

        }

        public void OnTransitionEvent(string newSceneName, Vector3 newScenePos)
        {
            if(!isFading)
                StartCoroutine(TransitionScene(newSceneName, newScenePos));
        }

        private IEnumerator loadSceneSetActive(string SceneName)
        {
            yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);

            EventHandler.CallUpAfterLoadSceneEvent();

        }
        /// <summary>
        /// 根据要跳转的场景名跳转场景
        /// </summary>
        /// <param name="newSceneName">新场景名</param>
        /// <param name="newScenePos">在新场景的位置</param>
        /// <returns></returns>
        public IEnumerator TransitionScene(string newSceneName, Vector3 newScenePos)
        {
            EventHandler.CallUpBeforeUnLoadSceneEvent();

            yield return SetCanvasGroupAlpha(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return loadSceneSetActive(newSceneName);

            EventHandler.CallUpMoveToNewSceneEvent(newScenePos);

            yield return SetCanvasGroupAlpha(0);



            //EventHandler.CallUpAfterLoadSceneEvent();

        }

        /// <summary>
        /// 1表示显示加载页面 0表示消失加载页面
        /// </summary>
        /// <param name="targetAlpha"></param>
        private IEnumerator SetCanvasGroupAlpha(float targetAlpha)
        {
            isFading = true;
            loadingCanvasGroup.blocksRaycasts = true;


            float fadeSpeed = Mathf.Abs(loadingCanvasGroup.alpha - targetAlpha) / Settings.loadingFadeDuration;

            while (!Mathf.Approximately(loadingCanvasGroup.alpha, targetAlpha))
            {
                loadingCanvasGroup.alpha = Mathf.MoveTowards(loadingCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }

            isFading = false;
            loadingCanvasGroup.blocksRaycasts = false;
        }
    }
}