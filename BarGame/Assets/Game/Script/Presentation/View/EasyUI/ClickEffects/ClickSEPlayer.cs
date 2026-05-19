using UnityEngine;
using VContainer;
using Game.Presentation;

namespace Game.Presentation.View
{
    public class ClickSEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioClip = null!;
        private SEPlayer _sePlayer = null!;

        [Inject]
        internal void Construct(SEPlayer sePlayer)
        {
            _sePlayer = sePlayer;
        }

        public void PlaySE()
        {
            _sePlayer.PlaySE(_audioClip);
        }
    }
}