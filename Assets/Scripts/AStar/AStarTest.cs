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

        public Vector2Int startPos;
        public Vector2Int targetPos;

        private AStar astar;

        public bool displayStartAndTargetTile;
        public bool displayPath;

        private Stack<MovementStep> moveStack;

        private void Awake()
        {
            astar = GetComponent<AStar>();
            moveStack = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();
        }

        private void ShowPathOnGridMap()
        {
            if (tilemap != null&& displayPathTile != null)
            {
                if (displayStartAndTargetTile)
                {
                    tilemap.SetTile((Vector3Int)startPos, displayPathTile);
                    tilemap.SetTile((Vector3Int)targetPos, displayPathTile);
                }
                else
                {
                    tilemap.SetTile((Vector3Int)startPos, null);
                    tilemap.SetTile((Vector3Int)targetPos, null);
                }

                if (displayPath)
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    if (astar.BuildPath(sceneName, startPos, targetPos, moveStack))
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