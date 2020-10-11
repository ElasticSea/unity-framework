using UnityEngine;

namespace Core.Ui
{
    public abstract class OnDragFormatter : MonoBehaviour
    {
        public abstract void OnDragChanged(DragPosition dragPosition);
        public abstract void OnOriginChanged(bool isOrigin);
    }
}