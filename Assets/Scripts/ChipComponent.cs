using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] private float _timeToMove;
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, false);
        }

        public override IEnumerator Move(BaseClickComponent cell)
        {
            while (Vector3.Distance(transform.position, cell.transform.position) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, cell.transform.position,
                    _timeToMove * Time.deltaTime);
                yield return null;
            }
        }
    }
}