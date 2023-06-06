using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    [RequireComponent(typeof(ClickHandler))]
    public class DeskGenerator : MonoBehaviour
    {
        [SerializeField] private int _rows;
        [SerializeField] private int _cols;

        [SerializeField] private CellComponent _cellPrefab;
        [SerializeField] private ChipComponent _chipPrefab;

        private readonly List<ChipComponent> _chips = new ();
        
        private ClickHandler _clickHandler;
        
        private void Awake()
        {
            _clickHandler = GetComponent<ClickHandler>();

            var cells = new CellComponent[_rows, _cols];
            var previousColor = ColorType.White;
            
            for (int i = 0; i < _rows; i++)
            {
                var previousPosition = 0f;
                previousColor = previousColor == ColorType.White ? ColorType.Black : ColorType.White;

                for (int j = 0; j < _cols; j++)
                {
                    var cell = Instantiate(_cellPrefab, new Vector3(i, 0, previousPosition), Quaternion.identity, transform);

                    previousPosition += cell.transform.localScale.x;
                    previousColor = previousColor == ColorType.White ? ColorType.Black : ColorType.White;
                    cell.SetDefaultMaterial(previousColor == ColorType.Black ? cell.BlackMaterial : cell.WhiteMaterial);

                    cells[i, j] = cell;
                    cell.Coordinate = new Coordinate(i, j);

                    if (previousColor != ColorType.Black)
                    {
                        continue;
                    }
                    
                    switch (j)
                    {
                        case < 3:
                        {
                            CreateChip(cell, ColorType.Black);
                            break;
                        }

                        case > 4:
                        {
                            CreateChip(cell, ColorType.White);
                            break;
                        }
                    }
                }
            }
            
            _clickHandler.Init(cells, _chips);
        }

        private void CreateChip(CellComponent cell, ColorType color)
        {
            var chip = Instantiate(_chipPrefab, cell.transform.position, Quaternion.identity, transform);
            chip.SetDefaultMaterial(color == ColorType.Black ? chip.BlackMaterial : chip.WhiteMaterial);
            chip.Pair = cell;
            cell.Pair = chip;
            chip.Color = color;
            chip.Coordinate = cell.Coordinate;
            _chips.Add(chip);
        }
    }
}