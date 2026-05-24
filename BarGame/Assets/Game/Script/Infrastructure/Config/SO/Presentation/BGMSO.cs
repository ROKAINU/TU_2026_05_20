using System.Collections.Generic;
using UnityEngine;

namespace Game.Infrastructure
{
    [CreateAssetMenu(fileName = "BGMSO", menuName = "Game/AudioSO/BGMSO", order = 0)]
    public class BGMSO : ScriptableObject
    {
        public List<AudioClip> bgmClips = new List<AudioClip>();
    }
}