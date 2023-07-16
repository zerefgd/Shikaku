using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector] public int Id;
    [HideInInspector] public bool IsNumber;
    [HideInInspector] public int Value;

    [SerializeField] private SpriteRenderer _cellRenderer;
    [SerializeField] private TMP_Text _numbersText;
    [SerializeField] private Color _unsolvedColor;
    [SerializeField] private Color _solvedColor;

    private List<Color> cellColors;

    public void Init(bool isNumber, int colorValue, int numberValue)
    {
        cellColors = GameManager.Colors;
        IsNumber = isNumber;
        if (IsNumber)
        {
            Id = colorValue;
            Value = numberValue;
            _numbersText.gameObject.SetActive(true);
            _numbersText.text = Value.ToString();
            _numbersText.color = _unsolvedColor;
        }
        else
        {
            Id = -2;
            Value = 0;
            _numbersText.gameObject.SetActive(false);
        }
        _cellRenderer.color = cellColors[Id + 2];
    }

    public void CleanGrid(int id)
    {
        if (IsNumber || Id != id)
        {
            return;
        }

        Id = -2;
        _cellRenderer.color = cellColors[Id + 2];
    }

    public void HighLightEmpty()
    {
        if (IsNumber) return;
        Id = -1;
        _cellRenderer.color = cellColors[Id + 2];
    }

    public void HighLightFilled(int id)
    {
        Id = id;
        _cellRenderer.color = cellColors[Id + 2];
    }

    public void UpdateNumber(bool correct)
    {
        _numbersText.color = correct ? _solvedColor : _unsolvedColor;
    }
}
