using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIDragRotate : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("Target for horizontal movement")]
    [SerializeField] private RectTransform targetObject;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minX = -200f;
    [SerializeField] private float maxX = 200f;

    [Header("Text display")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private float minValue = 90f;
    [SerializeField] private float maxValue = 150f;
    [SerializeField] private string numberFormat = "F1";
    [SerializeField] private string textSuffix = " Hz";   // ← суффикс

    private Vector2 _startDragPos;
    private float _startAngle;
    private float _currentTargetX;

    private void Start()
    {
        if (targetObject != null)
        {
            _currentTargetX = targetObject.anchoredPosition.x;
            UpdateTextFromPosition(_currentTargetX);
        }
    }

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
        if (targetObject == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 currentDragPos);
        float currentAngle = GetAngleToPoint(currentDragPos);
        float deltaAngle = Mathf.DeltaAngle(_startAngle, currentAngle);

        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + deltaAngle);

        float deltaPos = deltaAngle * sensitivity;
        float newX = _currentTargetX + deltaPos;
        newX = Mathf.Clamp(newX, minX, maxX);

        Vector2 anchoredPos = targetObject.anchoredPosition;
        anchoredPos.x = newX;
        targetObject.anchoredPosition = anchoredPos;

        _currentTargetX = newX;
        UpdateTextFromPosition(newX);
        _startAngle = currentAngle;
    }

    private float GetAngleToPoint(Vector2 point)
    {
        Vector2 dir = point - (Vector2)transform.localPosition;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void UpdateTextFromPosition(float currentX)
    {
        if (displayText == null) return;

        float t = Mathf.InverseLerp(minX, maxX, currentX);
        float value = Mathf.Lerp(minValue, maxValue, t);
        displayText.text = value.ToString(numberFormat) + textSuffix;
    }
}