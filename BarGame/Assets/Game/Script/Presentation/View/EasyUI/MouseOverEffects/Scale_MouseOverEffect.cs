using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Presentation.View
{
    public class Scale_MouseOverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float scaleUp = 1.2f;
        [SerializeField] private float scaleSpeed = 10f;

        private Vector3 originalScale;
        private Vector3 targetScale;

        void Awake()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }

        void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * scaleUp;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
        }
    }
}