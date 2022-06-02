using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class BulletController : MonoBehaviour
{
    private Vector2Int direction;
    public float speed = 10;
    public bool FireByEnemy;
    private Cell[,] cells;

    public void Initialize(Cell[,] cells, bool fireByEnemy)
    {
        this.cells = cells;
        FireByEnemy = fireByEnemy;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;

        var ourCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        if (cells[ourCell.x, ourCell.y].Space != CellSpace.Empty)
        {
            if (cells[ourCell.x, ourCell.y].TryDestroyCell())
            {
                if (cells[ourCell.x, ourCell.y].fieldObject != null)
                {
                    cells[ourCell.x, ourCell.y].fieldObject.Die();
                }
            }

            Destroy(gameObject);
        }
        if (cells[ourCell.x, ourCell.y].Occupant != null)
        {
            Tank tank = FireByEnemy ? cells[ourCell.x, ourCell.y].Occupant.GetComponent<Player>() : cells[ourCell.x, ourCell.y].Occupant.GetComponent<EnemyAI>();
            if (tank != null)
            {
                tank.Die();
                Destroy(gameObject);
            }
        }
    }

    public void Fire(Vector2Int direction)
    {
        this.direction = direction;
    }
}
