#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Domain;

namespace Game.Infrastructure
{
    [CreateAssetMenu(menuName = "Game/Scene Catalog", fileName = "SceneCatalog")]
    public sealed class SceneCatalog : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public SceneId sceneId;
            [Tooltip("Addressablesのシーンアドレス")]
            public string address = "";
        }

        [SerializeField] private List<Entry> _entries = new();

        private Dictionary<SceneId, string>? _map;

        private void OnEnable() => _map = null;

#if UNITY_EDITOR
        private void OnValidate() => _map = null;
#endif

        private Dictionary<SceneId, string> GetMap()
        {
            if (_map != null) return _map;

            _map = new Dictionary<SceneId, string>(_entries.Count);
            foreach (var e in _entries)
            {
                if (e == null || string.IsNullOrWhiteSpace(e.address))
                    continue;

                if (!_map.TryAdd(e.sceneId, e.address))
                    throw new InvalidOperationException(
                        $"[SceneCatalog] Duplicate SceneId '{e.sceneId}' found. " +
                        $"First: '{_map[e.sceneId]}', Duplicate: '{e.address}'"
                    );
            }
            return _map;
        }

        public bool TryGetAddress(SceneId id, out string address)
        {
            if (GetMap().TryGetValue(id, out var found))
            {
                address = found;
                return true;
            }
            address = string.Empty;
            return false;
        }

        public string GetAddressOrThrow(SceneId id)
        {
            if (!TryGetAddress(id, out var address))
                throw new KeyNotFoundException($"[SceneCatalog] Address not found for SceneId: {id}");
            return address;
        }
    }
}