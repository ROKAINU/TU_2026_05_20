using System.Collections.Generic;
using UnityEngine;

namespace Game.Presentation
{
    [CreateAssetMenu(fileName = "SESO", menuName = "Game/AudioSO/SESO", order = 1)]
    public class SESO : ScriptableObject
    {
        public List<AudioClip> seClips = new List<AudioClip>();
    }
}