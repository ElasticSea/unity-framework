using ElasticSea.Framework.Ui.Interactions;
using UnityEngine;

namespace ElasticSea.Framework.Ui.VisionOs
{
    public interface IVisionOsInteractableCallback
    {
        void Run(GameObject target, IInteractable interactable);
    }
}