using Gizmoes.Interactables;
using UnityEngine;
using Util;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIcon : MonoBehaviour
    {
        public int Index;
        public HideShowAnimation FocusTransition;
        public Material Material;
        public SimpleInteractable Interactable;

        public void Focus()
        {
            FocusTransition.Show();
        }

        public void Unfocus()
        {
            FocusTransition.Hide();
        }
    }
}