#nullable enable

using Cysharp.Threading.Tasks;
using LitMotion;
using TriInspector;
using UnityEngine;

namespace Game.Presentation.View
{
    public class UIShakerTween : MonoBehaviour
    {
        /// <summary> 揺らす対象のUI </summary>
        [SerializeField] private RectTransform targetUI = null!;

        /// <summary> 揺れる時間 </summary>
        [Header("Shake Settings")]
        [SerializeField] private float duration = 0.5f;
        /// <summary> 揺れの振幅 </summary>
        [SerializeField] private float strength = 20f;
        /// <summary> 振動回数 </summary>
        [SerializeField] private int vibrato = 10;

        /// <summary> 左右に揺らすか </summary>
        [Header("Axis")]
        [SerializeField] private bool shakeX = true;
        /// <summary> 上下に揺らすか </summary>
        [SerializeField] private bool shakeY = true;

        /// <summary> Y軸揺れの強さ倍率 </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float yStrengthMultiplier = 0.3f;

        private MotionHandle _shakeHandle;

        private void Awake()
        {
            if (targetUI == null)
                targetUI = GetComponent<RectTransform>();
        }

        [Button]
        public async UniTask Shake()
        {
            _shakeHandle.TryCancel();

            Vector3 originalPos =
                targetUI.anchoredPosition3D;

            _shakeHandle = LMotion
                .Create(0f, 1f, duration)
                .WithEase(Ease.Linear)
                .Bind(t =>
                {
                    float decay = 1f - t;

                    float angle =
                        t * vibrato * Mathf.PI * 2f;

                    float x = 0f;
                    float y = 0f;

                    if (shakeX)
                    {
                        x =
                            Mathf.Sin(angle) *
                            strength *
                            decay;
                    }

                    if (shakeY)
                    {
                        y =
                            Mathf.Cos(angle * 1.37f) *
                            strength *
                            yStrengthMultiplier *
                            decay;
                    }

                    targetUI.anchoredPosition3D =
                        originalPos +
                        new Vector3(x, y, 0f);
                })
                .AddTo(this);

            await _shakeHandle.ToUniTask();

            if (targetUI != null)
                targetUI.anchoredPosition3D = originalPos;
        }
    }
}