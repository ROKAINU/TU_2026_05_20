using UnityEngine;

namespace Game.Presentation.View
{
    public class SetActiveGameObjectOnClick : MonoBehaviour
    {
        [SerializeField] private GameObject _targetObject;

        public void SetActive(bool isActive)
        {
            if (_targetObject != null)
                _targetObject.SetActive(isActive);
        }
    }
}