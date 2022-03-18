using UnityEngine;
using UnityEngine.UI;

namespace ElasticSea.Framework.Extensions
{
    public static class RectTransformExtensions
    {
        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }

        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }
        
        public static Canvas GetParentCanvas(this RectTransform rt)
        {
            var parent = rt;
            var parentCanvas = rt.GetComponent<Canvas>();

            var SearchIndex = 0;
            while (parentCanvas == null || SearchIndex > 50)
            {
                parentCanvas = rt.GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    parent = parent.parent.GetComponent<RectTransform>();
                    SearchIndex++;
                }
            }
            return parentCanvas;
        }

        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }

        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }

        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetXPosition(this RectTransform trans, float x)
        {
            trans.anchoredPosition = new Vector2(x, trans.anchoredPosition.y);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width),
                newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width),
                newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width),
                newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width),
                newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }
        
        public static void SetSize(this RectTransform trans, float width, float heigh)
        {
            trans.SetSize(new Vector2(width, heigh));
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            var oldSize = trans.rect.size;
            var deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax +
                              new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }

        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }

        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }

        public static void SetAnchorX(this RectTransform transform, float x)
        {
            transform.anchoredPosition = new Vector2(x, transform.anchoredPosition.y);
        }

        public static void SetAnchorY(this RectTransform transform, float y)
        {
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, y);
        }

        public static void SetAnchorMinX(this RectTransform transform, float x)
        {
            transform.anchorMin = new Vector2(x, transform.anchorMin.y);
        }

        public static void SetAnchorMinY(this RectTransform transform, float y)
        {
            transform.anchorMin = new Vector2(transform.anchorMin.x, y);
        }

        public static void SetAnchorMaxX(this RectTransform transform, float x)
        {
            transform.anchorMax = new Vector2(x, transform.anchorMax.y);
        }

        public static void SetAnchorMaxY(this RectTransform transform, float y)
        {
            transform.anchorMax = new Vector2(transform.anchorMax.x, y);
        }

        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            var rect = rectTransform.rect;
            var worldMin = rectTransform.TransformPoint(new Vector3(rect.xMin, rect.yMin, 0));
            var worldMax = rectTransform.TransformPoint(new Vector3(rect.xMax, rect.yMax, 0));
            return Rect.MinMaxRect(
                worldMin.x,
                worldMin.y,
                worldMax.x,
                worldMax.y
            );
        }
        
        public static Vector2 FitIntoCanvas(this RectTransform rt)
        {
            var worldRect = rt.GetWorldRect();
            var canvas = rt.GetComponentInParent<Canvas>();
            var canvasScale = canvas.transform.localScale;
            var canvasRect = canvas.pixelRect;
            var rightOffset = worldRect.xMax - canvasRect.width;
            var leftOffset = canvasRect.x - worldRect.xMin;
            var upOffset = worldRect.yMax - canvasRect.height;
            var bottomOffset = canvasRect.y - worldRect.yMin;

            var offset = new Vector2();
            if (rightOffset > 0) offset -= new Vector2(rightOffset / canvasScale.x, 0);
            if (leftOffset > 0) offset += new Vector2(leftOffset / canvasScale.x, 0);
            if (upOffset > 0) offset -= new Vector2(0, upOffset / canvasScale.y);
            if (bottomOffset > 0) offset += new Vector2(0, bottomOffset / canvasScale.y);

            return offset;
        }

        public static RectTransform CopyFrom(this RectTransform destination, RectTransform source)
        {
            destination.localPosition = source.localPosition;
            destination.localRotation = source.localRotation;
            destination.localScale = source.localScale;
            destination.sizeDelta = source.sizeDelta;
            destination.anchorMin = source.anchorMin;
            destination.anchorMax = source.anchorMax;
            destination.offsetMin = source.offsetMin;
            destination.offsetMax = source.offsetMax;
            destination.pivot = source.pivot;
            return destination;
        }
        
        public static (int columns, int rows) GetColumnsAndRows(this GridLayoutGroup glg)
        {
            var column = 0;
            var row = 0;

            if (glg.transform.childCount == 0)
                return (0, 0);

            //Column and row are now 1
            column = 1;
            row = 1;

            //Get the first child GameObject of the GridLayoutGroup
            var firstChildObj = glg.transform.
                GetChild(0).GetComponent<RectTransform>();

            var firstChildPos = firstChildObj.anchoredPosition;
            bool stopCountingRow = false;

            //Loop through the rest of the child object
            for (int i = 1; i < glg.transform.childCount; i++)
            {
                //Get the next child
                var currentChildObj = glg.transform.
                    GetChild(i).GetComponent<RectTransform>();

                var currentChildPos = currentChildObj.anchoredPosition;

                //if first child.x == otherchild.x, it is a column, ele it's a row
                if (firstChildPos.x == currentChildPos.x)
                {
                    column++;
                    //Stop couting row once we find column
                    stopCountingRow = true;
                }
                else
                {
                    if (!stopCountingRow)
                        row++;
                }
            }

            return (column, row);
        }
    }
}