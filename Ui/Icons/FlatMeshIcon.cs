using ElasticSea.Framework.Interactions;
using ElasticSea.Framework.Scripts.Util;
using ElasticSea.Framework.Util;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIcon : MonoBehaviour
    {
        public int Index;
        public string Name;
        public IHideShowAnim FocusTransition;
        public Material Material;
        public SimpleInteractable Interactable;
        public GameObject Backplate;
        public GameObject Frontplate;
        public Collider Collider;
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