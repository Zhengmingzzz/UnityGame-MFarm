using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace MFarm.N_AStar
{
    public class AStarTest : MonoBehaviour
    {
        public Tilemap tilemap;

        public TileBase displayPathTile;

        public Vector2Int startWorldPos;
        public Vector2Int targetWorldPos;

        private AStar astar;

        public bool displayStartAndTargetTile;
        public bool displayPath;

        private Stack<MovementStep> moveStack;


        [Header("NPC“∆∂Ø≤‚ ‘")]
        public ScheduleDetails schedule;
        public bool isMove;
        public MFarm.NPC.NPC_Movement NPCMovement;

        private void Awake()
        {
            astar = GetComponent<AStar>();
            moveStack = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();
            if (isMove)
            {
                isMove = false;
                NPCMovement.BuildPath(schedule);
            }
        }

        private void ShowPathOnGridMap()
        {
            if (tilemap != null&& displayPathTile != null)
            {
                if (displayStartAndTargetTile)
                {
                    tilemap.SetTile((Vector3Int)startWorldPos, displayPathTile);
                    tilemap.SetTile((Vector3Int)targetWorldPos, displayPathTile);
                }
                else
                {
                    tilemap.SetTile((Vector3Int)startWorldPos, null);
                    tilemap.SetTile((Vector3Int)targetWorldPos, null);
                }

                if (displayPath)
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    if (astar.BuildPath(sceneName, startWorldPos, targetWorldPos, moveStack))
                    {
                        foreach (var m in moveStack)
                        {
                            tilemap.SetTile((Vector3Int)m.gridCoordinate, displayPathTile);
                        }
                    }
                    else
                    {
                        Debug.Log("FindLoadError");
                    }
                    
                }
                else
                {
                    if (moveStack.Count > 0)
                    {
                        foreach (var m in moveStack)
                        {
                            tilemap.SetTile((Vector3Int)m.gridCoordinate, null);
                        }
                            moveStack.Clear();
                    }
                }




            }
        }

    }
}