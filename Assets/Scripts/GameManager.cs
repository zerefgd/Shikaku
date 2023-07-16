using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static List<Color> Colors;

    [SerializeField] private float _gridSize = 2f;
    [SerializeField] private SpriteRenderer _bgPrefab;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private List<Color> _cellColors;
    [SerializeField] private Level _level;

    private bool hasGameFinished;
    private Cell[,] cells;
    private Vector2Int startGrid, endGrid;
    private int currentResetId;

    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        Colors = _cellColors;
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        //SpawnBG
        SpriteRenderer bg = Instantiate(_bgPrefab);
        bg.size = new Vector2(_level.Columns + 0.05f, _level.Rows + 0.05f) * _gridSize;
        bg.transform.position = new Vector3(_level.Columns, _level.Rows, 0)
            * 0.5f * _gridSize;

        //SpawnCells
        cells = new Cell[_level.Rows, _level.Columns];
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                cells[i, j] = Instantiate(_cellPrefab);
                bool isNumber = _level.NumberPositions.Contains(new Vector2Int(i, j));
                int colorIndex = _level.Data[i * _level.Columns + j];
                int numberValue = isNumber ? _level.NumberValue(new Vector2Int(i, j)) : 0;
                cells[i, j].Init(isNumber, colorIndex, numberValue);
                cells[i, j].transform.position = new Vector3(j + 0.5f, i + 0.5f, 0) * _gridSize;
            }
        }

        //SetUpCamera
        Camera.main.orthographicSize = Mathf.Max(_level.Rows, _level.Columns) * _gridSize + 4f;
        Vector3 camPos = Camera.main.transform.position;
        camPos.x = bg.transform.position.x;
        camPos.y = bg.transform.position.y;
        Camera.main.transform.position = camPos;
    }

    private void Update()
    {
        if (hasGameFinished) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = Vector2Int.zero;
        gridPos.x = Mathf.FloorToInt(mousePos.y / _gridSize);
        gridPos.y = Mathf.FloorToInt(mousePos.x / _gridSize);

        if (Input.GetMouseButtonDown(0))
        {
            startGrid = gridPos;
            endGrid = gridPos;
            if (!IsValid(startGrid)) return;
            HighLight();
        }
        else if (Input.GetMouseButton(0))
        {
            CleanGrid(currentResetId);
            endGrid = gridPos;
            if (!IsValid(endGrid) || !IsValid(startGrid)) return;
            HighLight();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startGrid = Vector2Int.zero;
            endGrid = Vector2Int.zero;
            CleanGrid(-1);
            CheckWin();
        }
    }

    private void HighLight()
    {
        Vector2Int start, end;
        start = new Vector2Int(startGrid.x < endGrid.x ? startGrid.x : endGrid.x,
            startGrid.y < endGrid.y ? startGrid.y : endGrid.y);
        end = new Vector2Int(startGrid.x > endGrid.x ? startGrid.x : endGrid.x,
            startGrid.y > endGrid.y ? startGrid.y : endGrid.y);

        int numberCells = 0;
        Vector2Int numberPos = Vector2Int.zero;
        for (int i = start.x; i <= end.x; i++)
        {
            for (int j = start.y; j <= end.y; j++)
            {
                numberCells += cells[i, j].IsNumber ? 1 : 0;
                if (cells[i, j].IsNumber)
                {
                    numberPos = new Vector2Int(i, j);
                }
            }
        }

        if (numberCells == 0 || numberCells > 1)
        {
            for (int i = start.x; i <= end.x; i++)
            {
                for (int j = start.y; j <= end.y; j++)
                {
                    if (cells[i, j].Id >= 0)
                    {
                        CleanGrid(cells[i, j].Id);
                    }

                    cells[i, j].HighLightEmpty();
                }
            }

            currentResetId = -1;
        }

        if (numberCells == 1)
        {
            for (int i = start.x; i <= end.x; i++)
            {
                for (int j = start.y; j <= end.y; j++)
                {
                    if (cells[i, j].Id >= 0 &&
                        cells[i, j].Id != cells[numberPos.x, numberPos.y].Id)
                    {
                        CleanGrid(cells[i, j].Id);
                    }

                    cells[i, j].HighLightFilled(cells[numberPos.x, numberPos.y].Id);
                }
            }

            currentResetId = cells[numberPos.x,numberPos.y].Id;
        }

        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                if (cells[i, j].IsNumber)
                {
                    cells[i, j].UpdateNumber(IsValidCell(i, j));
                }
            }
        }
    }


    private void CleanGrid(int id)
    {
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                cells[i, j].CleanGrid(id);
            }
        }
    }

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Columns && pos.y < _level.Rows;
    }
    private bool IsValidCell(int i, int j)
    {
        int numOfCells = 0;
        int id = cells[i, j].Id;
        for (int row = 0; row < _level.Rows; row++)
        {
            for (int col = 0; col < _level.Columns; col++)
            {
                numOfCells += cells[row, col].Id == id ? 1 : 0;
            }
        }
        return numOfCells == cells[i, j].Value;
    }

    private void CheckWin()
    {
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                if (cells[i, j].Id < 0)
                    return;

                if (cells[i, j].IsNumber && !IsValidCell(i, j))
                    return;
            }
        }

        hasGameFinished = true;
        StartCoroutine(GameWin());
    }

    private IEnumerator GameWin()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
