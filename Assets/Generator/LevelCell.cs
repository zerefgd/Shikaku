using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelCell : MonoBehaviour
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
        cellColors = LevelGenerator.Colors;
        Id = colorValue;
        IsNumber = isNumber;
        _cellRenderer.color = cellColors[Id + 2];
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
    }
    
    public void UpdateFill(int fill)
    {
        Id = fill;
        _cellRenderer.color = cellColors[Id + 2];
    }
}
