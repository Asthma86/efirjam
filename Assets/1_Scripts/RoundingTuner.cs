using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragRotate : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Vector2 _startDragPos;
    private float _startAngle;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _startDragPos);
        _startAngle = GetAngleToPoint(_startDragPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 currentDragPos);
        float currentAngle = GetAngleToPoint(currentDragPos);
        float delta = Mathf.DeltaAngle(_startAngle, currentAngle);
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + delta);
        _startAngle = currentAngle; // для непрерывного вращения без рывков
    }

    private float GetAngleToPoint(Vector2 point)
    {
        Vector2 dir = point - (Vector2)transform.localPosition;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}