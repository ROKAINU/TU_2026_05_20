using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Presentation.SceneTransition;

namespace Game.Presentation.View
{
    public class SceneTransitionUI : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0f;
        [SerializeField] private SceneId sceneId;
        private SceneTransitioner _sceneTransitioner;
        
        [Inject]
        internal void Construct(SceneTransitioner sceneTransitioner)
        {
            _sceneTransitioner = sceneTransitioner;
        }

        public void Transition()
        {
            _sceneTransitioner.TransitionToAsync(sceneId, new SceneTransitioner.TransitionOptions(fadeDuration: fadeDuration)).Forget();
        }
    }
}