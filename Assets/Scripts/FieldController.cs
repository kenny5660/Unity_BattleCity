using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class FieldController : MonoBehaviour
{
    public GroundChessBoard groundChessVoxel;
    public FieldObject bedrockVoxel;
    public FieldObject grassVoxel;
    public FieldObject flagBasePrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Camera mainCamera;
    private bool isGameEnd;

    public float playerSpeed = 3;
    public float enemySpeed = 2;
    public int enemyCount = 5;
    public int enemySpawnSeconds = 5;

    public Cell[,] cells;
    private int width;
    private int height;
    private Player player;
    private int enemyCountDie = 0;
    private Vector2Int posPlayerSpawn;
    private Vector2Int flagPos;
    private List<Vector2Int> posEnemySpawn;

    private MenuController menuController;

    private string[] map = new[] {
        ".E....E....E...",
        "..@.@..@..@....",
        "@.@..@..@...@..",
        ".##.###@@.##...",
        ".@....#@.......",
        ".@.@..#..@..@..",
        ".@@@#....#@@...",
        ".#.P.......#...",
        ".#..@@@@@..#...",
        "....@@F@@......"
    };

    void Start()
    {
        posEnemySpawn = new List<Vector2Int>();
        menuController = GameObject.FindObjectOfType<MenuController>();
        height = map.Length;
        width = map[0].Length;

        cells = new Cell[width + 2, height + 2];

        for (int i = 0; i < height; i++)
        {
            if (map[i].Length != width)
            {
                throw new System.Exception("Invalid map");
            }
            for (int j = 0; j < width; j++)
            {
                cells[j + 1, i + 1] = new Cell(map[i][j] == '#' ? CellSpace.Bedrock :
                                               map[i][j] == '@' ? CellSpace.Grass :
                                               map[i][j] == 'F' ? CellSpace.Flag :
                                               CellSpace.Empty);
                if (map[i][j] == 'P')
                {
                    posPlayerSpawn = new Vector2Int(j + 1, i + 1);
                    PlayerSpawn();
                }
                if (map[i][j] == 'E')
                {
                    posEnemySpawn.Add(new Vector2Int(j + 1, i + 1));
                }
            }
        }

        for (int i = 0; i < width + 2; i++)
        {
            cells[i, 0] = new Cell(CellSpace.Bedrock);
            cells[i, height + 1] = new Cell(CellSpace.Bedrock);
        }

        for (int i = 0; i < height + 2; i++)
        {
            cells[0, i] = new Cell(CellSpace.Bedrock);
            cells[width + 1, i] = new Cell(CellSpace.Bedrock);
        }

        for (var x = 0; x < width + 2; x++)
        {
            for (var y = 0; y < height + 2; y++)
            {
                var c = Instantiate(groundChessVoxel, new Vector3(x, 0, y), Quaternion.identity, transform);
                c.SetColor((x + y) % 2 == 0);

                if (cells[x, y].Space == CellSpace.Bedrock)
                {
                    cells[x, y].fieldObject = Instantiate(bedrockVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                }
                if (cells[x, y].Space == CellSpace.Grass)
                {
                    cells[x, y].fieldObject = Instantiate(grassVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                }
                if (cells[x, y].Space == CellSpace.Flag)
                {
                    flagPos = new Vector2Int(x, y);
                    cells[x, y].fieldObject = Instantiate(flagBasePrefab, new Vector3(x, 1, y), Quaternion.identity, transform);
                }

            }
        }
        if (mainCamera.orthographic)
        {
            mainCamera.transform.position = new Vector3((width + 1) / 2.0f, 10, (height + 1) / 2.0f);
            mainCamera.orthographicSize = (height + 1) / 2 + 1;
        }
        else
        {
            mainCamera.transform.position = new Vector3((width + 1) / 2.0f, (Mathf.Max(width, height)) / Mathf.Tan(mainCamera.fieldOfView * Mathf.Deg2Rad / 2) / 2, (height + 1) / 2.0f);
        }
        mainCamera.transform.eulerAngles = new Vector3(90, 0, 0);
        StartCoroutine(EnemySpawner());

    }
    private IEnumerator EnemySpawner()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemySpawn();
            yield return new WaitForSeconds(enemySpawnSeconds);
        }
    }
    // Update is called once per frame
    public void PlayerSpawn()
    {
        var playerGO = Instantiate(playerPrefab, new Vector3(posPlayerSpawn.x, 1, posPlayerSpawn.y), Quaternion.identity, transform);
        player = playerGO.GetComponent<Player>();
        player.Initialize(playerSpeed, this);
        cells[posPlayerSpawn.x, posPlayerSpawn.y].Occupy(player);
    }
    public void EnemySpawn()
    {
        Vector2Int pos = posEnemySpawn[UnityEngine.Random.Range(0, posEnemySpawn.Count)];
        var enemyGO = Instantiate(enemyPrefab, new Vector3(pos.x, 1, pos.y), Quaternion.identity, transform);
        var e = enemyGO.GetComponent<EnemyAI>();
        e.Initialize(enemySpeed, this);
        e.InitializeAI(flagPos);
        cells[pos.x, pos.y].Occupy(e);
        StartCoroutine(e.Think());

    }
    public void PlayerDie()
    {
        PlayerSpawn();
    }

    public void EnemyDie()
    {
        enemyCountDie++;
        if (enemyCountDie >= enemyCount)
        {
            EndGame(true);
        }
    }
    public void EndGame(bool isWin)
    {
        if (!isWin)
        {
            player.Die();
        }
        isGameEnd = true;
        menuController.ShowEndGameMenu(isWin);
    }
    void Update()
    {
        if (!isGameEnd)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(player.TryMove(Vector2Int.right));
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(player.TryMove(Vector2Int.left));
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(player.TryMove(Vector2Int.up));
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(player.TryMove(Vector2Int.down));
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.Fire();
            }
        }

    }
}
