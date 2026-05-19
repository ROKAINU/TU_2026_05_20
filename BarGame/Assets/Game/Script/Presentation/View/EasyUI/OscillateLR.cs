using UnityEngine;

namespace Game.Presentation.View
{
    public class OscillateLR : MonoBehaviour
    {
        [SerializeField] private float amplitude = 3f;     // 移動幅
        [SerializeField] private float speed = 1f;         // 往復速度
        [SerializeField] private float phaseOffset = 0f;   // 位相オフセット（開始位置調整）
        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            float x = startPos.x + Mathf.Sin(Time.time * speed + phaseOffset) * amplitude;
            transform.position = new Vector3(x, startPos.y, startPos.z);
        }
    }
}