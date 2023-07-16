using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;
    public static List<Color> Colors;

    [SerializeField] private int _rows, _cols;
    [SerializeField] private float _gridSize = 2f;
    [SerializeField] private SpriteRenderer _bgPrefab;
    [SerializeField] private LevelCell _cellPrefab;
    [SerializeField] private List<Color> _cellColors;
    [SerializeField] private Level _level;

    private LevelCell[,] cells;
    private int currentCellFillValue;

    private void Awake()
    {
        Instance = this;
        Colors = _cellColors;
        currentCellFillValue = -2;
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        SetUpStartLevel();
        SetUpBG();
        SetUpGrid();
        SetUpCamera();
    }

    private void SetUpStartLevel()
    {
        if (_rows == _level.Rows && _cols == _level.Columns) return;

        _level.Columns = _cols;
        _level.Rows = _rows;
        _level.Data = new List<int>();
        _level.NumberPositions = new List<Vector2Int>();
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _cols; j++)
            {
                _level.Data.Add(-2);
            }
        }
        EditorUtility.SetDirty(_level);
    }

    private void SetUpBG()
    {
        //SpawnBG
        SpriteRenderer bg = Instantiate(_bgPrefab);
        bg.size = new Vector2(_level.Columns + 0.05f, _level.Rows + 0.05f) * _gridSize;
        bg.transform.position = new Vector3(_level.Columns, _level.Rows, 0)
            * 0.5f * _gridSize;
        cells = new LevelCell[_level.Rows, _level.Columns];
    }

    private void SetUpGrid()
    {
        //SpawnCells
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
    }

    private void SetUpCamera()
    {
        //SetUpCamera
        Camera.main.orthographicSize = Mathf.Max(_level.Rows, _level.Columns) * _gridSize + 4f;
        Vector3 camPos = Camera.main.transform.position;
        camPos.x = _level.Columns * _gridSize * 0.5f;
        camPos.y = _level.Rows * _gridSize * 0.5f;
        Camera.main.transform.position = camPos;
    }

    private void ResetGrid()
    {
        //SpawnCells
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                bool isNumber = _level.NumberPositions.Contains(new Vector2Int(i, j));
                int colorIndex = _level.Data[i * _level.Columns + j];
                int numberValue = isNumber ? _level.NumberValue(new Vector2Int(i, j)) : 0;
                cells[i, j].Init(isNumber, colorIndex, numberValue);
            }
        }
    }

    private void Update()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = Vector2Int.zero;
        gridPos.x = Mathf.FloorToInt(mousePos.y / _gridSize);
        gridPos.y = Mathf.FloorToInt(mousePos.x / _gridSize);

        if(Input.GetMouseButtonDown(0))
        {
            if (!IsValid(gridPos)) return;

            _level.Data[gridPos.x * _cols + gridPos.y] = currentCellFillValue;
            cells[gridPos.x, gridPos.y].UpdateFill(currentCellFillValue);
            ResetGrid();
            EditorUtility.SetDirty(_level);
        }

        if(Input.GetMouseButtonDown(1))
        {
            if(!IsValid(gridPos)) return;
            if(!_level.NumberPositions.Contains(gridPos))
            {
                _level.NumberPositions.Add(gridPos);
            }
            ResetGrid();
            EditorUtility.SetDirty(_level);
        }
       
    }

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Columns && pos.y < _level.Rows;
    }

    public void SetCurrentCellFillValue(int fill)
    {
        currentCellFillValue = fill;
    }
}
