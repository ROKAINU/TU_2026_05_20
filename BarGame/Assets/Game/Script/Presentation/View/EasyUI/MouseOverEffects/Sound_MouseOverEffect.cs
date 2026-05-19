using UnityEngine;
using VContainer;
using UnityEngine.EventSystems;
using Game.Presentation;

namespace Game.Presentation.View
{
    public class Sound_MouseOverEffect : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip _audioClip = null!;
        private SEPlayer _sePlayer = null!;

        [Inject]
        internal void Construct(SEPlayer sePlayer)
        {
            _sePlayer = sePlayer;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _sePlayer.PlaySE(_audioClip);
        }
    }
}