using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Presentation.View
{
    public class OtherScale_MouseOverEffect : MonoBehaviour
    {
        [SerializeField] private float scaleUp = 1.2f;
        [SerializeField] private float scaleSpeed = 10f;
        [SerializeField] private GameObject otherObject;

        private Vector3 originalScale;
        private Vector3 targetScale;

        void Awake()
        {
            originalScale = otherObject.transform.localScale;
            targetScale = originalScale;
        }

        void Update()
        {
            otherObject.transform.localScale = Vector3.Lerp(otherObject.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
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
