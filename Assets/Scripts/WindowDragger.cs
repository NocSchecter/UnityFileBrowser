using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.DreamOS
{
    public class WindowDragger : UIBehaviour, IBeginDragHandler, IDragHandler
    {
        [Header("Resources")]
        public RectTransform dragArea;
        public RectTransform dragObject;

        private Vector2 originalLocalPointerPosition;
        private Vector3 originalPanelLocalPosition;

        public new void Start()
        {
            if (dragArea == null)
            {
                try
                {
                    dragArea = GetComponent<RectTransform>();
                }

                catch { Debug.LogWarning("<b>[Window Dragger]</b> Drag Area is missing.", this); }
            }
        }


        public void OnBeginDrag(PointerEventData data)
        {
            originalPanelLocalPosition = dragObject.localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, data.position, data.pressEventCamera, out originalLocalPointerPosition);
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 localPointerPosition;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, data.position, data.pressEventCamera, out localPointerPosition))
            {
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                dragObject.localPosition = originalPanelLocalPosition + offsetToOriginal;
            }

            ClampToArea();
        }

        public void ClampToArea()
        {
            Vector3 pos = dragObject.localPosition;
            Vector3 minPosition = dragArea.rect.min - dragObject.rect.min;
            Vector3 maxPosition = dragArea.rect.max - dragObject.rect.max;
            pos.x = Mathf.Clamp(dragObject.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(dragObject.localPosition.y, minPosition.y, maxPosition.y);
            dragObject.localPosition = pos;
        }
    }
}