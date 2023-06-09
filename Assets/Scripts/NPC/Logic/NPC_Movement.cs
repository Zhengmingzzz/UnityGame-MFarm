﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.N_AStar;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System;
using UnityEditor;
using DG.Tweening;

namespace MFarm.NPC {
    public class NPC_Movement : MonoBehaviour
    {
        [Header("临时变量")]
        public string currentScene;
        private string targetScene;
        public string StartScene { set => currentScene = value; }

        private Vector3Int currentGridPosition;
        private Vector3Int targetGridPosition;

        private ScheduleDetails currentSchedule;

        private TimeSpan GameTimeSpan => TimeManager.Instance.GameTimeSpan;

        private Vector3 nextWorldPosition;

        private bool canPlayWaitAnimation;

        private TimeSpan watiTimeSpan;


        [Header("角色移动基本参数")]
        public float speed = 5f;
        public float minSpeed = 1f;
        public float maxSpeed = 100f;

        private Vector3 dir;
        private bool isMove;

        private bool isSceneLoading;

        [Header("动画片段")]
        private AnimationClip stopAnimationClip;
        public AnimationClip blankAnimationClip;


        [Header("NPC组件")]
        private SpriteRenderer NPC_SpriteRenderer;
        private Rigidbody2D RB2D;
        private BoxCollider2D boxCollider2D;
        private Animator animator;
        private AnimatorOverrideController animatorOverrideController;


        private Stack<MovementStep> npcMoveStack = new Stack<MovementStep>();

        private Grid currentGrid;


        private void Awake()
        {
            NPC_SpriteRenderer = GetComponent<SpriteRenderer>();
            RB2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();

            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        }
        private void OnEnable()
        {
            EventHandler.AfterLoadSceneEvent += OnAftarSceneLoadEvent;
            EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        }
        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAftarSceneLoadEvent;
            EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;

        }
        private void FixedUpdate()
        {
            MoveMent();
        }
        private void Update()
        {
            if(!isSceneLoading)
                SetAnimation();
        }

        private void OnAftarSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            InitNPC();
            CheckSceneValid();

            isSceneLoading = false;
        }

        private void OnBeforeUnLoadSceneEvent()
        {
            isSceneLoading = true;
        }

        #region 设置NPC显示情况
        private void CheckSceneValid()
        {
            if (SceneManager.GetActiveScene().name == currentScene)
            {
                SetActiveInScene();
            }
            else
            {
                SetEnActiveInScene();
            }
        }

        private void SetActiveInScene()
        {
            NPC_SpriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
            //TODO:对影子操作
            //transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        }

        private void SetEnActiveInScene()
        {
            NPC_SpriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
            //TODO:对影子操作
            //transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
        #endregion 

        private void InitNPC()
        {
            targetScene = currentScene;
            currentGridPosition = currentGrid.WorldToCell(transform.position);

            transform.localPosition = new Vector3(currentGridPosition.x + Settings.baseCellSize / 2, currentGridPosition.y + Settings.baseCellSize / 2, 0) ;
            Debug.Log(transform.position);
            Debug.Log(transform.localPosition);

            targetGridPosition = currentGridPosition;
        }

        public void BuildPath(ScheduleDetails schedule)
        {
            npcMoveStack.Clear();
            currentSchedule = schedule;

            stopAnimationClip = schedule.targetAnimation;

            //同场景
            if (schedule.targetScene == currentScene)
            {
                AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, (Vector2Int)schedule.targetGridPos, npcMoveStack);
            }
            else
            {
                //TODO:创建异场景路径创建代码
            }

            if (npcMoveStack.Count > 0)
            {
                UpdateStepsTimeSpan();
            }

        }
        private void UpdateStepsTimeSpan()
        {
            if (npcMoveStack.Count > 0)
            {
                MovementStep priviousStep = null;

                TimeSpan currentTime = GameTimeSpan;

                foreach (MovementStep currentStep in npcMoveStack)
                {

                    if (priviousStep == null)
                        priviousStep = currentStep;

                    currentStep.hour = currentTime.Hours;
                    currentStep.minute = currentTime.Minutes;
                    currentStep.seconds = currentTime.Seconds;

                    float moveDistance;
                    if (IsMoveInDiagonalGrid(priviousStep, currentStep))
                    {
                        moveDistance = Settings.baseCellDiagonalSize;
                        
                    }
                    else
                    {
                        moveDistance = Settings.baseCellSize;

                    }
                    float moveTime = (moveDistance / speed / Settings.secondThreshold);
                    if (moveTime % 1 > 0.5f)
                    {
                        moveTime++;
                    }


                    TimeSpan moveToNextGridTime = new TimeSpan(0, 0, (int)moveTime) ;

                    currentTime+= moveToNextGridTime;

                    priviousStep = currentStep;
                }

            }
        }

        private bool IsMoveInDiagonalGrid(MovementStep priviousStep,MovementStep currentStep)
        {
            return (priviousStep.gridCoordinate.x != currentStep.gridCoordinate.x) && (priviousStep.gridCoordinate.y != currentStep.gridCoordinate.y);
        }

        #region 移动相关代码
        private void MoveMent()
        {

            if (!isSceneLoading && !isMove)
            {
                if (npcMoveStack.Count > 0)
                {
                    MovementStep nextStep = npcMoveStack.Pop();

                    currentScene = nextStep.SceneName;
                    CheckSceneValid();

                    TimeSpan nextStepTimeSpan = new TimeSpan(nextStep.hour, nextStep.minute, nextStep.seconds);
                    Vector3Int nextGridPos = (Vector3Int)nextStep.gridCoordinate;

                    MoveToNextStep(nextStepTimeSpan, nextGridPos);
                }

            }
        }

        private void MoveToNextStep(TimeSpan nextStepTimeSpan, Vector3Int nextGridPos)
        {
            StartCoroutine(moveCooroutine(nextStepTimeSpan, nextGridPos));

            if (npcMoveStack.Count == 0)
            {
                canPlayWaitAnimation = true;
                watiTimeSpan = GameTimeSpan;

            }
        }


        IEnumerator moveCooroutine(TimeSpan nextStepTimeSpan, Vector3Int nextGridPos)
        {
            isMove = true;
            nextWorldPosition = GridToWorldPosition(nextGridPos);

            if (nextStepTimeSpan > GameTimeSpan)
            {
                float distance = Mathf.Abs(Vector3.Distance(nextWorldPosition, transform.localPosition));
                float totalTime = (float)(nextStepTimeSpan.TotalSeconds - GameTimeSpan.TotalSeconds);
                float normalSpeed = Mathf.Max(minSpeed, distance / totalTime / Settings.secondThreshold);

                dir = (nextWorldPosition - transform.localPosition).normalized;

                Vector3 startWorldPos = transform.localPosition;
                Vector3 targetWorldPos = GridToWorldPosition(nextGridPos);

                if (normalSpeed <= maxSpeed)
                {

                    //开始移动
                    while (Mathf.Abs(Vector3.Distance(nextWorldPosition, transform.localPosition)) > Settings.pixelSize)
                    {

                        Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                        RB2D.MovePosition(RB2D.position + posOffset);
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
            RB2D.transform.position = nextWorldPosition;
            isMove = false;
            currentGridPosition = nextGridPos;
            targetGridPosition = nextGridPos;


        }


        private Vector3 GridToWorldPosition(Vector3Int gridPos)
        {
            Vector3 worldPos = currentGrid.CellToWorld(gridPos);
            return new Vector3(worldPos.x + Settings.baseCellSize / 2, worldPos.y + Settings.baseCellSize / 2, 0);
        }
        #endregion

        private void SetAnimation()
        {
            animator.SetBool("isMove", isMove);

            if (isMove)
            {
                animator.SetFloat("DirX", dir.x);
                animator.SetFloat("DirY", dir.y);

                watiTimeSpan = GameTimeSpan;
            }
            else
            {
                if (GameTimeSpan.TotalSeconds > watiTimeSpan.TotalSeconds + Settings.NPCWatiEventTime)
                {
                    playWatiAnimationClip();
                }
            }
            
            animator.SetBool("ExitAction", isMove);


        }

        private void playWatiAnimationClip()
        {
            if (stopAnimationClip != null && canPlayWaitAnimation)
            {
                animatorOverrideController[blankAnimationClip] = stopAnimationClip;
                animator.SetTrigger("ReadinessAction");

                watiTimeSpan = GameTimeSpan;

                canPlayWaitAnimation = false;
            }
            else
            {
                animatorOverrideController[stopAnimationClip] = blankAnimationClip;

            }
        }
    }
}