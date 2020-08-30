using UnityEngine;

namespace _Framework.Ui.TreeViews
{
    public abstract class OnDragFormatter : MonoBehaviour
    {
        public abstract void OnDragChanged(DragPosition dragPosition);
        public abstract void OnOriginChanged(bool isOrigin);
    }
}