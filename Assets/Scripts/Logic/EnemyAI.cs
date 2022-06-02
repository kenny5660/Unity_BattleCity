using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{

    internal enum EnemyAiType
    {
        FlagHunter,
        PlayerHunter
    }
    internal class EnemyAI : Tank
    {
        Vector2Int flagPos;
        public EnemyAiType aiType;
        List<Vector2Int> whereToMove;
        static int counter = 0;

        public override void Initialize(float moveSpeed, FieldController fieldController)
        {
            base.Initialize(moveSpeed, fieldController);
            isEnemy = true;


        }

        public void InitializeAI(Vector2Int flagPos)
        {
            this.flagPos = flagPos;
            whereToMove = new List<Vector2Int>()
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            aiType = counter % 2 == 0 ? EnemyAiType.FlagHunter : EnemyAiType.PlayerHunter;
            counter++;

        }
        public Vector2Int GetPlayerPos()
        {
            Vector2Int p = default;
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Occupant != null && !cells[x, y].Occupant.isEnemy)
                    {
                        p = new Vector2Int(x, y);
                    }
                }
            }
            return p;
        }
        public List<Vector2Int> bfs_to_pos(Vector2Int targetPos)
        {
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);
            bool[,] usedCell = new bool[width, height];
            Vector2Int[,] prevCell = new Vector2Int[width, height];
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Vector2Int curCoords = new Vector2Int(-1, -1);
            List<Vector2Int> path = new List<Vector2Int>();
            List<Vector2Int> pathDirs = new List<Vector2Int>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    prevCell[i, j] = new Vector2Int(-1, -1);
                }
            }
            curCoords = GetCoords();
            queue.Enqueue(curCoords);
            usedCell[curCoords.x, curCoords.y] = true;
            while (queue.Count != 0)
            {
                curCoords = queue.Peek();
                queue.Dequeue();
                //Debug.Log("Cell " + curCoords.x + " " + curCoords.y);
                if (curCoords == targetPos)
                {
                    //Debug.Log("Find pos " + curCoords.x + " " + curCoords.y);
                    break;
                }
                foreach (var dir in whereToMove)
                {
                    var neiborCoords = curCoords + dir;
                    if (neiborCoords.x < width && neiborCoords.x >= 0 &&
                        neiborCoords.y < height && neiborCoords.y >= 0 &&
                        cells[neiborCoords.x, neiborCoords.y].Space != CellSpace.Bedrock &&
                        !usedCell[neiborCoords.x, neiborCoords.y])
                    {
                        queue.Enqueue(new Vector2Int(neiborCoords.x, neiborCoords.y));
                        usedCell[neiborCoords.x, neiborCoords.y] = true;
                        prevCell[neiborCoords.x, neiborCoords.y] = curCoords;
                    }
                }

            }
            var t = curCoords;
            while (t.x > 0 || t.y > 0)
            {
                pathDirs.Add(t - prevCell[t.x, t.y]);
                path.Add(t);
                t = prevCell[t.x, t.y];
            }
            pathDirs.RemoveAt(pathDirs.Count - 1);
            pathDirs.Reverse();

            path.Reverse();
            return pathDirs;
        }

        public IEnumerator Think()
        {

            while (true)
            {

                if (UnityEngine.Random.value < 0.4)
                {
                    Fire();
                }
                else
                {
                    if (UnityEngine.Random.value > 0.8)
                    {
                        yield return TryMove(whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)]);
                    }
                    else
                    {
                        var next_step = aiType == EnemyAiType.PlayerHunter ? bfs_to_pos(GetPlayerPos())[0] : bfs_to_pos(flagPos)[0];
                        var next_coor = GetCoords() + next_step;
                        if (cells[next_coor.x, next_coor.y].Space != CellSpace.Empty)
                        {

                            if (Fire())
                            {
                                Debug.Log("Fire");
                            }
                            else
                            {
                                yield return TryMove(whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)]);
                            }
                        }
                        else
                        {
                            yield return TryMove(next_step);
                        }

                    }

                }

                yield return new WaitForSeconds(0.5f);
            }
        }
        public override void Die()
        {
            fieldController.EnemyDie();
            base.Die();
        }
    }
}
