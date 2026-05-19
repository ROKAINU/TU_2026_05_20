using UnityEngine;
using UnityEngine.UI;

namespace Game.Presentation.View
{
    public class BlinkingLight : MonoBehaviour
    {
        public Color baseColor = Color.white;   // 元の色
        public float blinkIntensity = 2f;       // 明るさの増幅値
        public float blinkSpeed = 2f;           // 点滅周期（1秒で何回点滅するか）

        [SerializeField] private Image objectMaterial;

        void Start()
        {
            if (objectMaterial == null)
                objectMaterial = GetComponent<Image>();
            if (objectMaterial == null)
            {
                Debug.LogError("BlinkingLight: Imageが見つかりません。");
            }

            objectMaterial.color = baseColor; // 初期色を設定
        }

        void Update()
        {
            if (objectMaterial == null) return;

            // サイン波で点滅（0~1）
            float emission = (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI * 2f) + 1f) / 2f;

            // 明るさを調整してEmissionに反映
            Color finalColor = baseColor * Mathf.Lerp(1f, blinkIntensity, emission);
            objectMaterial.color = finalColor;
        }
    }
}
