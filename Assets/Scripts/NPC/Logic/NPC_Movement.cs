using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.N_AStar;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace MFarm.NPC {
    public class NPC_Movement : MonoBehaviour
    {
        public string currentScene;
        private string targetScene;
        public string StartScene { set => currentScene = value; }

        private Vector3Int currentGridPosition;
        private Vector3Int targetGridPosition;

        public float speed = 2f;
        public float minSpeed = 1f;
        public float maxSpeed = 3f;

        private int DirX;
        private int DirY;
        public bool isMove;


        [Header("NPC组件")]
        private SpriteRenderer NPC_SpriteRenderer;
        private Rigidbody2D RB2D;
        private BoxCollider2D boxCollider2D;
        private Animator animator;


        private AStar astar;
        private Stack<MovementStep> npcMoveStack;

        private Grid curretnGrid;


        private void Awake()
        {
            NPC_SpriteRenderer = GetComponent<SpriteRenderer>();
            RB2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            InitNPC();
        }

        private void OnEnable()
        {
            EventHandler.AfterLoadSceneEvent += OnAftarSceneLoadEvent;
        }
        private void OnDisable()
        {
            EventHandler.AfterLoadSceneEvent -= OnAftarSceneLoadEvent;

        }

        private void OnAftarSceneLoadEvent()
        {
            curretnGrid = FindObjectOfType<Grid>();
            CheckSceneValid();

        }

        #region 对NPC的场景检测
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
            currentGridPosition = curretnGrid.WorldToCell(transform.position);

            transform.position = new Vector3(currentGridPosition.x + Settings.baseCellSize / 2, currentGridPosition.y + Settings.baseCellSize / 2, 0) ;
            Debug.Log(transform.position);

            targetGridPosition = currentGridPosition;
        }
    }
}