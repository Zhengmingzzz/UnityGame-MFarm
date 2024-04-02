using MFarm.Map;
using System.Collections.Generic;
using UnityEngine;


namespace MFarm.N_AStar
{
    class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;

        // 整个地图的宽和高
        private int gridWidth;
        private int gridHeight;

        private Node startNode;
        private Node targetNode;

        private int originX;
        private int originY;

        // 选中的Node周围的八个Node
        private List<Node> openList;
        private HashSet<Node> closeList;
        public Stack<MovementStep> pathStack;

        private bool isFindPath;

        /// <summary>
        /// 生成GridNodes
        /// </summary>
        /// <param name="SceneName"></param>
        /// <returns>判断始末位置是否为障碍并且建立GridNodes二维数组</returns>
        private bool generateGridNodes(string SceneName, Vector2Int startGridPos, Vector2Int targetGridPos)
        {
            if (GridMapManager.Instance.getGridDimensions(SceneName, out Vector2Int gridDimension, out Vector2Int gridOrigin))
            {
                // 根据宽度和高度创建一个GridNodes类 包含该地图网格宽度高度和Node类的二维数组
                gridNodes = new GridNodes(gridDimension.x, gridDimension.y);

                gridWidth = gridDimension.x;
                gridHeight = gridDimension.y;

                originX = gridOrigin.x;
                originY = gridOrigin.y;
            }
            else
            {
                return false;
            }

            // startNode|targetNode都不是实际上的坐标位置
            startNode = gridNodes.getGridNode(startGridPos.x - originX, startGridPos.y - originY);
            targetNode = gridNodes.getGridNode(targetGridPos.x - originX, targetGridPos.y - originY);

             for (int x = 0; x < gridWidth; x++)
             {
                for (int y = 0; y < gridHeight; y++)
                {

                    // 根据实际坐标拿到对应的瓦片信息
                    TileDetail tile = GridMapManager.Instance.getTileDetailByPos(SceneName, new Vector3Int(x + originX, y + originY, 0));
                    if (tile != null)
                    {
                        // 将瓦片是否为障碍物的信息填入新建的二维数组GridNodes
                        gridNodes.gridNodesArray[x, y].isObstacle = tile.NPC_Obstacle;

                    }
 
                }
             }
            if (startNode.isObstacle || targetNode.isObstacle)
            {
                Debug.Log("startNode||targetNode is Obstacle");
                return false;
            }

            return true;
        }

        public bool BuildPath(string SceneName, Vector2Int startGridPos, Vector2Int targetGridPos, Stack<MovementStep> movementSteps)
        {
            isFindPath = false;


            if (generateGridNodes(SceneName, startGridPos, targetGridPos))
            {
                openList = new List<Node>();
                closeList = new HashSet<Node>();


                if (FindShortestPath())
                {
                    //找到路径并填充到Stack中
                    GetMovementStepToStack(SceneName, movementSteps);
                }

            }
            return isFindPath;
        }

        private bool FindShortestPath()
        {
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                openList.Sort();

                Node currentNode = openList[0];

                openList.RemoveAt(0);

                closeList.Add(currentNode);

                if (currentNode == targetNode)
                {
                    isFindPath = true;
                    break;
                }

                //评估周围八个点补充到openList
                GetNeighbourhoodNode(currentNode);


            }
            return isFindPath;
        }


        private void GetNeighbourhoodNode(Node currentNode)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    Node validNeighbourNode = GetValidNeighbourNode(currentNode.NodeX + x, currentNode.NodeY + y);
                    if (validNeighbourNode != null)
                    {
                        if (!openList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = GetNeighbourNodeDistance(startNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetNeighbourNodeDistance(targetNode, validNeighbourNode);

                            validNeighbourNode.parentNode = currentNode;

                            openList.Add(validNeighbourNode);
                        }
                        //else
                        //{
                        //    if (validNeighbourNode.fCost < currentNode.fCost)
                        //    {

                        //    }
                        //    else if (validNeighbourNode.fCost == currentNode.fCost)
                        //    {

                        //    }

                        //}
                    }
                }
            }
        }

        private Node GetValidNeighbourNode(int x, int y)
        {
            Node neighbourhoodNode = null;
            if (!(x < 0 || x > gridWidth || y < 0 || y > gridHeight))
            {
                Node node = gridNodes.gridNodesArray[x, y];
                if (!closeList.Contains(node))
                {
                    if (!node.isObstacle)
                    {
                        neighbourhoodNode = node;
                    }

                }
            }
            return neighbourhoodNode;
        }

        private int GetNeighbourNodeDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.NodeX - nodeB.NodeX);
            int yDistance = Mathf.Abs(nodeA.NodeY - nodeB.NodeY);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            else if (xDistance == yDistance)
            {
                return 14 * xDistance;
            }
            else
            {
                return 14 * xDistance + 10 * (yDistance - xDistance);
            }
        }

        private void GetMovementStepToStack(string sceneName, Stack<MovementStep> movementSteps)
        {
            Node nextNode = targetNode;


            while (nextNode != null)
            {
                MovementStep MS = new MovementStep();
                MS.SceneName = sceneName;
                MS.gridCoordinate = new Vector2Int(nextNode.NodeX + originX, nextNode.NodeY + originY);

                movementSteps.Push(MS);
                nextNode = nextNode.parentNode;
            }
        }



    }
}