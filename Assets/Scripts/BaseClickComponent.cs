using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [field : SerializeField]
        public Material WhiteMaterial { get; set; }
        
        [field : SerializeField]
        public Material BlackMaterial { get; set; }

        public Coordinate Coordinate;
        
        private Material _defaultMaterial;
        
        private MeshRenderer _meshRenderer;
        
        public ColorType Color { get; set; }
        public BaseClickComponent Pair { get; set; }

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetDefaultMaterial(Material material)
        {
            _defaultMaterial = material;
            SetMaterial(material);
        }
        
        public void SetMaterial(Material material = null)
        {
            _meshRenderer.sharedMaterial = material ? material : _defaultMaterial;
        }
        
        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler Clicked;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;


        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter(PointerEventData eventData);

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit(PointerEventData eventData);
        
        public abstract IEnumerator Move(BaseClickComponent cell);

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick(PointerEventData eventData)
		{
            Clicked?.Invoke(this);
        }

        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
		}
    }

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
    
    public readonly struct Coordinate
    {
        public readonly int X;
        public readonly int Y;

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}