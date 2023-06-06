using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Checkers
{
    public class PathCreator
    {
        private readonly CellComponent[,] _cells;
        private readonly PlayableSide _playableSide;

        private BaseClickComponent _currentCell;

        public List<BaseClickComponent> DestroyCandidate { get; private set; } = new ();
        
        public PathCreator(CellComponent[,] cells, PlayableSide playableSide)
        {
            _cells = cells;
            _playableSide = playableSide;
        }

        [ItemCanBeNull]
        public List<CellComponent> FindFreeCells(BaseClickComponent cell)
        {
            _currentCell = cell;

            var pairs = new List<CellComponent>();

            if (_playableSide.CurrentSide == ColorType.Black)
            {
                MoveChip(pairs, 1, 1);
            }
            else
            {
                MoveChip(pairs, -1, 1);
            }

            return pairs;
        }

        private void MoveChip(List<CellComponent> pairs, int rowIndex, int colIndex)
        {
            DestroyCandidate.Clear();
            
            for (int i = 0; i < 2; i++, colIndex = -colIndex)
            {
                var nextRow = _currentCell.Coordinate.Y + rowIndex;
                var nextColumn = _currentCell.Coordinate.X + colIndex;

                if (CheckFreeSpaceOver(nextRow, nextColumn))
                {
                    pairs.Add(_cells[nextColumn, nextRow]);
                }
                else if (CheckBorders(nextRow, nextColumn) &&
                         _cells[nextColumn, nextRow].Pair.Color != _playableSide.CurrentSide)
                {
                    DestroyCandidate.Add(_cells[nextColumn, nextRow]);
                    
                    if (_playableSide.CurrentSide == ColorType.Black)
                    {
                        nextRow++;
                    }
                    else
                    {
                        nextRow--;
                    }

                    if (colIndex > 0)
                    {
                        nextColumn++;
                    }
                    else
                    {
                        nextColumn--;
                    }
                    
                    if (CheckFreeSpaceOver(nextRow, nextColumn))
                    {
                        pairs.Add(_cells[nextColumn, nextRow]);
                    }
                }
            }
        }

        private bool CheckFreeSpaceOver(int nextRow, int nextColumn)
        {
            if (!CheckBorders(nextRow, nextColumn))
            {
                return false;
            }

            return _cells[nextColumn, nextRow].Pair == null;
        }

        private bool CheckBorders(int nextRow, int nextColumn)
        {
            if (nextRow < 0 || nextRow >= _cells.GetUpperBound(0) + 1)
            {
                return false;
            }

            return nextColumn < _cells.GetUpperBound(1) + 1 &&
                   nextColumn >= 0;
        }
    }
}