using ElasticSea.Framework.Ui.Interactions;
using ElasticSea.Framework.Util;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIcon : MonoBehaviour
    {
        public int Index;
        public IHideShowAnim FocusTransition;
        public Material Material;
        public SimpleInteractable Interactable;
        public GameObject Backplate;
        public GameObject Frontplate;
        public Rect BackplateRect;
        public Rect FrontplateRect;
        public (Vector2 center, float radius) FrontplateCircle;

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