using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.N_AStar;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System;
using UnityEditor;
using DG.Tweening;
using MFarm.Transition;

namespace MFarm.NPC
{
    /// <summary>
    /// 每个NPC身上都会挂在这个脚本 它会根据其中的schedules_so文件执行对应的行为
    /// </summary>
    public class NPC_Movement : MonoBehaviour
    {
        public string currentScene;
        private string targetScene;
        public string StartScene { set => currentScene = value; }

        private Vector3Int currentGridPosition;
        // private Vector3Int targetGridPosition;

        private ScheduleDetails currentSchedule;

        private TimeSpan GameTimeSpan => TimeManager.Instance.GameTimeSpan;

        private Vector3 nextWorldPosition;

        private bool isEnforceSchedule = false;

        #region 时间匹配所需的参数
        // 为时间表排序用于在OnGameMinute中对比是否到当前时间了
        // OnGameMinute函数在更新时间后会进行调用
        // 若时间匹配，会调用BuildPath函数
        public ScheduleDetails_SO schedulesData;
        // 用于schedule的排序，便于寻找
        private SortedSet<ScheduleDetails> scheduleSet;
        #endregion
        /// <summary>
        /// 是否可以播放等待动画
        /// 由Updata中的等待时间和Movement函数来控制
        /// </summary>
        private bool canPlayWaitAnimation = false;

        // 角色待在原地等待的时间戳
        // 只有在播放等待动画时才会对它进行重置
        private float waitTime;


        [Header("角色移动基本参数")]
        public float normalSpeed = 5f;
        public float minSpeed = 1f;
        public float maxSpeed = 100f;

        private Vector3 dir;
        private bool isMove;

        private bool isSceneLoading;

        [Header("动画片段")]
        private AnimationClip stopAnimationClip;
        /// <summary>
        /// 空的animatorClip
        /// </summary>
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

            // 动态创建和原来相同新的animatorcontroller来覆盖原先的controller 从而实现切换animator中clip的效果
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;

            // 初始化ScheduleSet
            scheduleSet = new SortedSet<ScheduleDetails>();
            // 将Schedule_SO的数据传入Set中
            foreach (ScheduleDetails schedule in schedulesData.ScheduleList)
            {
                scheduleSet.Add(schedule);
            }
        }

        private void OnEnable()
        {
            EventHandler.AfterLoadSceneEvent += OnAftarSceneLoadEvent;
            EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
            EventHandler.UpdataTime += OnGameMinuteEvent;
        }



        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAftarSceneLoadEvent;
            EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
            EventHandler.UpdataTime -= OnGameMinuteEvent;

        }
        private void FixedUpdate()
        {
            Movement();
        }
        private void Update()
        {
            if (!isSceneLoading)
                SetAnimation();

            // 此处用于控制等待时间
            waitTime -= Time.deltaTime;
            canPlayWaitAnimation = waitTime <= 0;

            isEnforceSchedule = npcMoveStack.Count > 0;
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

        /// <summary>
        /// 传入当前时间来匹配日程表的时间
        /// </summary>
        /// <param name="minute"></param>
        /// <param name="hour"></param>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
        {
            // 用于遍历日程表判断时间是否匹配
            int time = (hour * 100) + minute;
            ScheduleDetails matchedSchedule = null;
            foreach (var schedule in scheduleSet)
            {
                if (schedule.Time == time)
                {
                    if (schedule.season != season)
                        continue;
                    if (schedule.day != day)
                        continue;

                    // 按时间找到了Schedule
                    matchedSchedule = schedule;
                }
                else if (schedule.Time > time || schedule.day > day)
                {
                    break;
                }
            }

            if (matchedSchedule != null && isEnforceSchedule == false)
            {
                BuildPath(matchedSchedule);
            }
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

            // HACK:1
            transform.position = new Vector3(currentGridPosition.x + Settings.baseCellSize / 2, currentGridPosition.y + Settings.baseCellSize / 2, 0);

            //targetGridPosition = currentGridPosition;
        }
        /// <summary>
        /// 根据schedule的当前位置和目标位置建立路径
        /// </summary>
        /// <param name="schedule"></param>
        public void BuildPath(ScheduleDetails schedule)
        {
            npcMoveStack.Clear();
            currentSchedule = schedule;
            //targetGridPosition = schedule.targetGridPos;

            stopAnimationClip = schedule.targetAnimation;

            //同场景
            if (schedule.targetScene == currentScene)
            {
                AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, (Vector2Int)schedule.targetGridPos, npcMoveStack);
            }
            //异场景路径创建代码
            else
            {
                SceneRoute sceneRoute = NPC_Manager.Instance.GetSceneRoute(currentScene, schedule.targetScene);
                if (sceneRoute != null)
                {
                    foreach (var r in sceneRoute.SecneRouteList)
                    {
                        Vector2Int fromPos, gotoPos;
                        fromPos = r.fromGridCell;
                        gotoPos = r.gotoGridCell;

                        // 如果pos为9999代表from为当前位置goto要去schedule中的目标位置
                        if (fromPos.x >= Settings.maxGridSize || fromPos.y >= Settings.maxGridSize)
                        {
                            fromPos = (Vector2Int)currentGridPosition;
                        }
                        if (gotoPos.x >= Settings.maxGridSize || gotoPos.y >= Settings.maxGridSize)
                        {
                            gotoPos = (Vector2Int)schedule.targetGridPos;
                        }
                        AStar.Instance.BuildPath(r.sceneName, fromPos, gotoPos, npcMoveStack);
                    }
                }
            }

            // 路径计算完后开始走
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
                    float moveTime = (moveDistance / normalSpeed / Settings.secondThreshold);
                    if (moveTime % 1 > 0.5f)
                    {
                        moveTime++;
                    }

                    TimeSpan moveToNextGridTime = new TimeSpan(0, 0, (int)moveTime);

                    currentTime += moveToNextGridTime;

                    priviousStep = currentStep;
                }

            }
        }
        /// <summary>
        /// 是否走在对角线上
        /// </summary>
        /// <param name="priviousStep"></param>
        /// <param name="currentStep"></param>
        /// <returns></returns>
        private bool IsMoveInDiagonalGrid(MovementStep priviousStep, MovementStep currentStep)
        {
            return (priviousStep.gridCoordinate.x != currentStep.gridCoordinate.x) && (priviousStep.gridCoordinate.y != currentStep.gridCoordinate.y);
        }

        #region 移动相关代码
        /// <summary>
        /// fixupdata中会不断调用Movement函数
        /// 行走时会将isMove变量设置为ture防止行走时播放其他动画
        /// 同时将canPlayWaitAnimation设置为false
        /// </summary>
        private void Movement()
        {
            if (!isSceneLoading && !isMove)
            {
                if (npcMoveStack.Count > 0)
                {
                    resetWaitTime();

                    MovementStep nextStep = npcMoveStack.Pop();

                    // 每走一步都确定下一步的场景名
                    currentScene = nextStep.SceneName;
                    CheckSceneValid();

                    TimeSpan nextStepTimeSpan = new TimeSpan(nextStep.hour, nextStep.minute, nextStep.seconds);
                    Vector3Int nextGridPos = (Vector3Int)nextStep.gridCoordinate;
                    MoveToNextStep(nextStepTimeSpan, nextGridPos);
                }
                else
                {
                    // 使角色面朝下
                    animator.SetFloat("DirX", 0);
                    animator.SetFloat("DirY", -1);
                    animator.SetBool("Exit", false);
                }
            }
        }

        private void MoveToNextStep(TimeSpan nextStepTimeSpan, Vector3Int nextGridPos)
        {
            StartCoroutine(moveCooroutine(nextStepTimeSpan, nextGridPos));
        }
        IEnumerator moveCooroutine(TimeSpan nextStepTimeSpan, Vector3Int nextGridPos)
        {
            isMove = true;
            nextWorldPosition = GridToWorldPosition(nextGridPos);

            if (Vector3.Distance(nextGridPos, currentGridPosition) > 3)
                goto teleport;

            if (nextStepTimeSpan > GameTimeSpan)
            {
                // HACK:1
                float distance = Mathf.Abs(Vector3.Distance(nextWorldPosition, transform.position));
                float totalTime = (float)(nextStepTimeSpan.TotalSeconds - GameTimeSpan.TotalSeconds);
                float speed = Mathf.Max(minSpeed, distance / totalTime / Settings.secondThreshold);
                float normalSpeed = Mathf.Max(minSpeed, distance / totalTime / Settings.secondThreshold);
                // HACK:1
                dir = (nextWorldPosition - transform.position).normalized;
                // HACK:1
                Vector3 startWorldPos = transform.position;
                Vector3 targetWorldPos = GridToWorldPosition(nextGridPos);

                if (speed <= maxSpeed)
                {
                    // HACK:1
                    //开始移动
                    // 记录上次距目标位置的距离，用于判断NPC是否运动过快导致无法退出循环的情况
                    float lastDistance = Vector3.Distance(transform.position, nextWorldPosition);
                    while (Mathf.Abs(Vector3.Distance(nextWorldPosition, transform.position)) > Settings.pixelSize)
                    {
                        float currentDistance = Vector3.Distance(transform.position, nextWorldPosition);
                        if (lastDistance >= currentDistance)
                            lastDistance = currentDistance;
                        else
                            break;
                        Vector2 posOffset = new Vector2(dir.x * normalSpeed * Time.fixedDeltaTime, dir.y * normalSpeed * Time.fixedDeltaTime);
                        while (Mathf.Abs(Vector3.Distance(nextWorldPosition, transform.position)) > Settings.pixelSize)
                        {
                            RB2D.MovePosition(RB2D.position + posOffset);
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }
            }
        teleport:
            RB2D.transform.position = nextWorldPosition;
            isMove = false;
            currentGridPosition = nextGridPos;
            // targetGridPosition = nextGridPos;


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
                animator.SetBool("Exit", true);
            }
            else
            {
                if (canPlayWaitAnimation)
                {
                    // 播放开始等待动画
                    StartCoroutine(playWatiAnimationClip());
                }
            }

        }

        // 就是示例的SetStopAnimation
        private IEnumerator playWatiAnimationClip()
        {
            // 重置等待时间
            resetWaitTime();

            if (stopAnimationClip != null)
            {
                // 通过overridecontroller找到空的片段 再去切换动画片段
                animatorOverrideController[blankAnimationClip] = stopAnimationClip;

                // 若动画片段不为空且当前可以播放动画，则播放动画
                animator.SetBool("EventAnimation", true);
                yield return null;
                animator.SetBool("EventAnimation", false);
            }
            else
            {
                animatorOverrideController[stopAnimationClip] = blankAnimationClip;
                animator.SetBool("EventAnimation", false);
            }
            yield break;
        }
        void resetWaitTime()
        {
            waitTime = Settings.NPCWatiEventTime;
        }
    }
}


