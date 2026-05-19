#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Game.Kernel;

namespace Game.Presentation.View
{
    /// <summary>
    /// UI とのポインタ相互作用を検査するユーティリティ。
    /// マウスとタッチ入力に対応。
    ///
    /// 【設計方針】
    ///   ホバー用  : GetPointerOverUI()           ← マウス専用・押下不問。タッチにホバーはないため常に Err
    ///   クリック用: GetPointerOverUIOnPress()     ← プライマリタッチ＋マウス左ボタン押下。重複排除済み
    ///   マルチ用  : GetPointerOverUIMultiTouch()  ← 全タッチ＋マウス。重複排除済み
    ///
    /// 【タッチ専用デバイスの注意】
    ///   GetPointerOverUI() はマウスが存在しない場合 Err を返します。
    ///   タッチ入力での判定には GetPointerOverUIOnPress() を使用してください。
    ///
    /// 【Result の寿命】
    ///   返される List は内部キャッシュの参照です。
    ///   次の呼び出しまでに使い切るか、必要なら ToList() でコピーしてください。
    /// </summary>
    public static class UIPointerChecker
    {
        // -----------------------------------------------------------------------
        // バッファセット（scratch・output・dedup を常にセットで管理）
        // ④ バッファを構造体にまとめ、呼び出し側が個別に意識しなくて済むようにする
        // -----------------------------------------------------------------------
        private readonly struct RaycastBuffer
        {
            public readonly List<RaycastResult> Scratch;
            public readonly List<RaycastResult> Output;
            public readonly HashSet<int>        Dedup;

            public RaycastBuffer(
                List<RaycastResult> scratch,
                List<RaycastResult> output,
                HashSet<int> dedup)
            {
                Scratch = scratch;
                Output  = output;
                Dedup   = dedup;
            }

            public void Clear()
            {
                Output.Clear();
                Dedup.Clear();
                // Scratch は Raycast() 内でクリアするため不要
            }
        }

        // メソッドごとの専用バッファセット（共有による参照破壊を防ぐ）
        private static readonly RaycastBuffer _hoverBuf = new(new(), new(), new());
        private static readonly RaycastBuffer _pressBuf = new(new(), new(), new());
        private static readonly RaycastBuffer _multiBuf = new(new(), new(), new());

        // -----------------------------------------------------------------------
        // EventData 管理
        // -----------------------------------------------------------------------
        private static PointerEventData? _eventData;
        private static EventSystem?      _cachedEventSystem;

        private static PointerEventData? GetOrCreateEventData()
        {
            if (EventSystem.current == null) return null;

            if (_eventData == null || _cachedEventSystem != EventSystem.current)
            {
                _cachedEventSystem = EventSystem.current;
                _eventData = new PointerEventData(_cachedEventSystem);
            }
            return _eventData;
        }

        // -----------------------------------------------------------------------
        // 共通レイキャスト処理
        // -----------------------------------------------------------------------

        /// <summary>
        /// 指定位置でレイキャストして buf.Scratch に書き込む。
        /// EventSystem が null の場合は Scratch を空にして return する（防御的処理）。
        /// </summary>
        private static void Raycast(Vector2 position, List<RaycastResult> scratch)
        {
            scratch.Clear();
            var ed = GetOrCreateEventData();
            if (ed == null) return;

            ed.position = position;
            EventSystem.current!.RaycastAll(ed, scratch);
        }

        /// <summary>
        /// 指定位置でレイキャストし、重複なし結果を buf.Output に追記する。
        /// </summary>
        private static void RaycastAndDedup(Vector2 position, in RaycastBuffer buf)
        {
            Raycast(position, buf.Scratch);
            foreach (var r in buf.Scratch)
            {
                int id = r.gameObject.GetInstanceID();
                if (buf.Dedup.Add(id)) buf.Output.Add(r);
            }
        }

        // -----------------------------------------------------------------------
        // ホバー判定（マウス専用・押下不問）
        // ③ Hover も scratch 経由に統一し、設計を一貫させる
        // -----------------------------------------------------------------------

        /// <summary>
        /// ポインタが UI 要素の上にあるか判定（ホバー用・マウス専用）。
        /// ボタンの押下状態に関係なく、現在のマウス位置で判定する。
        /// タッチデバイスにはホバー概念がないため、Mouse が存在しない場合は Err を返す。
        /// タッチ入力での判定には GetPointerOverUIOnPress() を使用すること。
        /// </summary>
        public static Result<List<RaycastResult>> GetPointerOverUI()
        {
            if (EventSystem.current == null)
                return Result<List<RaycastResult>>.Err("EventSystem not found");
            if (Mouse.current == null)
                return Result<List<RaycastResult>>.Err("Mouse not found. Use GetPointerOverUIOnPress() for touch input.");

            // ③ Hover も scratch 経由に統一（重複排除は不要だが処理パターンを揃える）
            _hoverBuf.Clear();
            RaycastAndDedup(Mouse.current.position.ReadValue(), _hoverBuf);

            return _hoverBuf.Output.Count > 0
                ? Result<List<RaycastResult>>.Ok(_hoverBuf.Output)
                : Result<List<RaycastResult>>.Err("No UI element found under pointer");
        }

        // -----------------------------------------------------------------------
        // クリック／押下判定
        // -----------------------------------------------------------------------

        /// <summary>
        /// ポインタが UI 要素の上にあるか判定（クリック／押下用）。
        /// プライマリタッチ中またはマウス左ボタン押下中のときだけ判定する。
        /// 両方が同時にヒットした場合でも重複を排除して返す。
        /// </summary>
        public static Result<List<RaycastResult>> GetPointerOverUIOnPress()
        {
            if (EventSystem.current == null)
                return Result<List<RaycastResult>>.Err("EventSystem not found");

            _pressBuf.Clear();

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                RaycastAndDedup(Touchscreen.current.primaryTouch.position.ReadValue(), _pressBuf);

            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                RaycastAndDedup(Mouse.current.position.ReadValue(), _pressBuf);

            return _pressBuf.Output.Count > 0
                ? Result<List<RaycastResult>>.Ok(_pressBuf.Output)
                : Result<List<RaycastResult>>.Err("No UI element found under pointer");
        }

        // -----------------------------------------------------------------------
        // マルチタッチ版
        // -----------------------------------------------------------------------

        /// <summary>
        /// すべてのアクティブなタッチ＋マウスを対象にした判定。
        /// タブレット等でタッチとマウスが同時に有効な環境でも UI 要素の重複を排除する。
        /// </summary>
        public static Result<List<RaycastResult>> GetPointerOverUIMultiTouch()
        {
            if (EventSystem.current == null)
                return Result<List<RaycastResult>>.Err("EventSystem not found");

            _multiBuf.Clear();

            if (Touchscreen.current != null)
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (!touch.press.isPressed) continue;
                    RaycastAndDedup(touch.position.ReadValue(), _multiBuf);
                }

            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                RaycastAndDedup(Mouse.current.position.ReadValue(), _multiBuf);

            return _multiBuf.Output.Count > 0
                ? Result<List<RaycastResult>>.Ok(_multiBuf.Output)
                : Result<List<RaycastResult>>.Err("No UI elements found");
        }

        // -----------------------------------------------------------------------
        // bool 版ショートカット
        // -----------------------------------------------------------------------

        /// <summary>ポインタが UI 上にあるか（ホバー用・bool 版・マウス専用）。</summary>
        public static bool IsPointerOverUI() => GetPointerOverUI().IsOk;

        /// <summary>ポインタが UI 上にあるか（押下用・bool 版）。</summary>
        public static bool IsPointerOverUIOnPress() => GetPointerOverUIOnPress().IsOk;

        // -----------------------------------------------------------------------
        // 派生ユーティリティ
        // -----------------------------------------------------------------------

        public enum PointerMode
        {
            Hover,
            Press,
            MultiTouch,
        }

        /// <summary>
        /// ① 未知の PointerMode が渡された場合は例外を投げる（silent fallback 禁止）。
        /// </summary>
        private static Result<List<RaycastResult>> GetByMode(PointerMode mode) => mode switch
        {
            PointerMode.Hover      => GetPointerOverUI(),
            PointerMode.Press      => GetPointerOverUIOnPress(),
            PointerMode.MultiTouch => GetPointerOverUIMultiTouch(),
            _                      => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unhandled PointerMode"),
        };

        /// <summary>
        /// ポインタが特定のキャンバス上にあるか判定。
        /// mode で Hover / Press / MultiTouch を選択できる。
        /// </summary>
        public static Result<RaycastResult> GetPointerOverCanvas(
            Canvas canvas,
            PointerMode mode = PointerMode.Hover)
        {
            if (canvas == null)
                return Result<RaycastResult>.Err("Canvas is null");

            return GetByMode(mode).Match(
                ok: results =>
                {
                    foreach (var result in results)
                        if (result.gameObject.transform.IsChildOf(canvas.transform))
                            return Result<RaycastResult>.Ok(result);

                    return Result<RaycastResult>.Err($"No UI element found on canvas '{canvas.name}'");
                },
                err: error => Result<RaycastResult>.Err(error)
            );
        }

        /// <summary>
        /// 最初にヒットした UI 要素を取得。
        /// mode で Hover / Press / MultiTouch を選択できる。
        /// </summary>
        public static Result<RaycastResult> GetTopUIElement(PointerMode mode = PointerMode.Hover)
        {
            return GetByMode(mode).Match(
                ok: results => results.Count > 0
                    ? Result<RaycastResult>.Ok(results[0])
                    : Result<RaycastResult>.Err("No UI element found"),
                err: error => Result<RaycastResult>.Err(error)
            );
        }
    }
}