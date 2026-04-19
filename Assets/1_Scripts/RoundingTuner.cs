using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragRotate : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("Target for horizontal movement")]
    [SerializeField] private RectTransform targetObject;   // Второй UI-элемент, который двигается вправо/влево
    [SerializeField] private float sensitivity = 1f;       // Чувствительность: пикселей позиции на градус поворота
    [SerializeField] private float minX = -200f;           // Левая граница (по X)
    [SerializeField] private float maxX = 200f;            // Правая граница (по X)

    [Header("Text display")]
    [SerializeField] private Text displayText;             // UI Text для отображения значения
    [SerializeField] private float minValue = 90f;         // Значение при minX
    [SerializeField] private float maxValue = 150f;        // Значение при maxX
    [SerializeField] private string numberFormat = "F1";   // Формат: F1 = одна десятая (например, 120.5)

    private Vector2 _startDragPos;
    private float _startAngle;
    private float _currentTargetX;   // Текущая позиция targetObject по X

    private void Start()
    {
        if (targetObject != null)
        {
            // Сохраняем начальную позицию второго элемента
            _currentTargetX = targetObject.anchoredPosition.x;
            // Принудительно обновляем текст в соответствии с начальной позицией
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

        // Вращаем текущий элемент (без ограничений)
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + deltaAngle);

        // Двигаем второй элемент на величину, пропорциональную изменению угла
        float deltaPos = deltaAngle * sensitivity;
        float newX = _currentTargetX + deltaPos;

        // Ограничиваем позицию границами
        newX = Mathf.Clamp(newX, minX, maxX);

        // Применяем новую позицию к целевому объекту
        Vector2 anchoredPos = targetObject.anchoredPosition;
        anchoredPos.x = newX;
        targetObject.anchoredPosition = anchoredPos;

        // Сохраняем новую позицию для следующего шага
        _currentTargetX = newX;

        // Обновляем текст на основе новой позиции
        UpdateTextFromPosition(newX);

        // Обновляем начальный угол для плавного продолжения вращения
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

        // Линейная интерполяция: значение = minValue + (currentX - minX) / (maxX - minX) * (maxValue - minValue)
        float t = Mathf.InverseLerp(minX, maxX, currentX);
        float value = Mathf.Lerp(minValue, maxValue, t);

        // Отображаем с заданным форматом (например, "123.4")
        displayText.text = value.ToString(numberFormat);
    }
}