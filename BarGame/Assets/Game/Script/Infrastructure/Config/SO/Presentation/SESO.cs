using System.Collections.Generic;
using UnityEngine;

namespace Game.Infrastructure
{
    [CreateAssetMenu(fileName = "SESO", menuName = "Game/AudioSO/SESO", order = 1)]
    public class SESO : ScriptableObject
    {
        public List<AudioClip> seClips = new List<AudioClip>();
    }
}