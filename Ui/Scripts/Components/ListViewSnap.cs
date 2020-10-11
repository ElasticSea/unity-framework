using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Components
{
    public class ListViewSnap : MonoBehaviour
    {
        [SerializeField] private ListView listview;
        [SerializeField] private ScrollRect scrollRect;
        private RectTransform lastTOggle;

        private void Start()
        {
            listview.group.OnActiveTogglesChanged += Apply;
            Apply(listview.group.Active);
        }

        private void Apply(IEnumerable<IToggle> toggles)
        {
            var toggle = (toggles.FirstOrDefault() as Component)?.transform as RectTransform;
            if (toggle)
            {
                StartCoroutine(SnapAtTheEndOfTheFrame(toggle));
            }
        }

        private IEnumerator SnapAtTheEndOfTheFrame(RectTransform toggle)
        {
            yield return new WaitForEndOfFrame();
            
            var relativeScrollPos = (Vector2) scrollRect.transform.InverseTransformPoint(scrollRect.content.position);
            var relativePos = ((Vector2) (scrollRect.transform.InverseTransformPoint(toggle.position)));
            var vpSize = scrollRect.viewport.GetSize();
            var itemSize = toggle.GetSize();

            var achoredBase = relativeScrollPos - relativePos;

            if (relativePos.y < -vpSize.y / 2)
            {
                scrollRect.content.SetAnchorY(achoredBase.y - vpSize.y + itemSize.y * toggle.pivot.y);
            }
            else if (relativePos.y > vpSize.y / 2)
            {
                scrollRect.content.SetAnchorY(achoredBase.y - itemSize.y * (1 - toggle.pivot.y));
            }

            if (relativePos.x < -vpSize.x / 2)
            {
                scrollRect.content.SetAnchorX(achoredBase.x - vpSize.x + itemSize.x * toggle.pivot.x);
            }
            else if (relativePos.x > vpSize.x / 2)
            {
                scrollRect.content.SetAnchorX(achoredBase.x - itemSize.x * (1 - toggle.pivot.x));
            }
        }
    }
}