using Gameplay.ItemGenerators;
using Gameplay.Tutorial;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(TrashCan))]
    public class TrashCanHandler : MonoBehaviour
    {
        private TrashCan _trashCan;

        private void Awake()
        {
            _trashCan = GetComponent<TrashCan>();
        }

        private void OnEnable()
        {
            TutorialSystem.GetTrashCanTransform += GetTransform;
        }

        private void OnDisable()
        {
            TutorialSystem.GetTrashCanTransform -= GetTransform;
        }

        private Transform GetTransform() => transform;
    }
}