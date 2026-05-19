using UnityEngine;
using TMPro;

namespace Game.Presentation.View
{
    public class LicenseText : MonoBehaviour
    {
        [SerializeField] private TextAsset licenseTextAsset;
        [SerializeField] private TextMeshProUGUI licenseText;
        [SerializeField] private RectTransform textContainer;

        void Start()
        {
            SetupLicenseText();
        }

        void SetupLicenseText()
        {
            if (licenseTextAsset == null || licenseText == null || textContainer == null)
            {
                Debug.LogError("LicenseText: Please assign all references in the inspector.");
                return;
            }

            licenseText.text = licenseTextAsset.text;

            Canvas.ForceUpdateCanvases();
            licenseText.ForceMeshUpdate();

            Vector2 sizeDelta = textContainer.sizeDelta;
            float preferredHeight = licenseText.preferredHeight;
            sizeDelta.y = preferredHeight;
            textContainer.sizeDelta = sizeDelta;
        }
    }
}