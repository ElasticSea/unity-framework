using ElasticSea.Framework.Ui.Interactions;
using ElasticSea.Framework.Util;
using Gizmoes.Interactables;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIcon : MonoBehaviour
    {
        public int Index;
        public IHideShowAnim FocusTransition;
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