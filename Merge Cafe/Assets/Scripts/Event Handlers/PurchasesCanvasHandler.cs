using Gameplay.DecorationMode;
using Gameplay.Tutorial;
using UnityEngine;

namespace EventHandlers
{
    public class PurchasesCanvasHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            TutorialSystem.GetPurchaseButtonTransform += GetFirstPurchaseButtonTransform;
        }

        private void OnDisable()
        {
            TutorialSystem.GetPurchaseButtonTransform -= GetFirstPurchaseButtonTransform;
        }

        private Transform GetFirstPurchaseButtonTransform() => transform.GetChild(0);
    }
}
