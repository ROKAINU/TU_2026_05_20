using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Presentation.View
{
    public class Alpha_MouseOverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float baseAlpha = 1f;
        [SerializeField] private float targetAlpha = 0.9f;
        [SerializeField] private Image img;

        void Awake()
        {
            if(img == null)
                img = GetComponent<Image>();
            SetBaseAlpha(baseAlpha);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBaseAlpha(targetAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetBaseAlpha(baseAlpha);
        }

        public void SetBaseAlpha(float alpha)
        {
            var color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
}