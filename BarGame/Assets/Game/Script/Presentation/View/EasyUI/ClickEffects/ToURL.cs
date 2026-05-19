using UnityEngine;

namespace Game.Presentation.View
{
    public class ToURL : MonoBehaviour
    {
        [SerializeField] private string url;

        public void OpenURL()
        {
            UnityEngine.Application.OpenURL(url);
        }
    }
}