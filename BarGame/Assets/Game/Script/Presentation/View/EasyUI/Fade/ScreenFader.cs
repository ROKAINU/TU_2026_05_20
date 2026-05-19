using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Game.Presentation.View
{
    public class ScreenFader : MonoBehaviour
    {
        private Image fadeImage;
        private Canvas fadeCanvas;
        private Coroutine currentRoutine;

        void Awake()
        {
            CreateFadeCanvas();
        }

        private void CreateFadeCanvas()
        {
            if (fadeCanvas != null)
                return;

            // Canvas生成
            GameObject canvasObj = new GameObject("FadeCanvas");
            fadeCanvas = canvasObj.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            DontDestroyOnLoad(canvasObj);

            // Image生成（真っ黒）
            GameObject imgObj = new GameObject("FadeImage");
            imgObj.transform.SetParent(canvasObj.transform, false);

            fadeImage = imgObj.AddComponent<Image>();
            fadeImage.color = new Color(0, 0, 0, 0);  // 最初は透明
            fadeImage.raycastTarget = false;

            // 全画面にフィット
            RectTransform rect = fadeImage.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        // フェードイン（黒 → 透明）
        public async UniTask FadeIn(float duration)
        {
            await StartFade(1f, 0f, duration);
        }

        // フェードアウト（透明 → 黒）
        public async UniTask FadeOut(float duration)
        {
            await StartFade(0f, 1f, duration);
        }

        private async UniTask StartFade(float from, float to, float duration)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            await FadeRoutine(from, to, duration);
        }

        private async UniTask FadeRoutine(float from, float to, float duration)
        {
            float t = 0f;
            Color c = fadeImage.color;
            fadeImage.raycastTarget = true;

            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(from, to, t / duration);
                fadeImage.color = new Color(c.r, c.g, c.b, alpha);

                await UniTask.Yield();
            }

            fadeImage.color = new Color(c.r, c.g, c.b, to);
            fadeImage.raycastTarget = false;
        }

        /// <summary>
        /// 即座にアルファ値を変更する。
        /// </summary>
        /// <param name="alpha">設定する透明度</param>
        public void SetImmediateAlpha(float alpha)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            Color c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            fadeImage.raycastTarget = alpha > 0f;
        }
    }
}