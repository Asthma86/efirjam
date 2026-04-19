using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Header("Настройки времени")]
    public int minutesPerTick = 2;  // Сколько игровых минут прибавляем за раз
    public float tickInterval = 1f; // Раз в сколько реальных секунд тикают часы
    public float startHour = 8f;    // Стартовый час (08:00)

    [Header("Интерфейс")]
    public TextMeshProUGUI clockText;

    public int CurrentTotalMinutes { get; private set; }
    private float timer; // Наш внутренний секундомер

    void Start()
    {
        // Переводим стартовое время в целые минуты
        CurrentTotalMinutes = Mathf.FloorToInt(startHour * 60f);

        // Отрисовываем время сразу при старте игры
        UpdateClockUI();
    }

    void Update()
    {
        // Копим время с прошлого кадра
        timer += Time.deltaTime;

        // Как только накопилась 1 секунда (tickInterval)
        if (timer >= tickInterval)
        {
            timer -= tickInterval; // Скидываем таймер, сохраняя миллисекундную погрешность
            CurrentTotalMinutes += minutesPerTick; // Прибавляем 2 минуты

            UpdateClockUI(); // Обновляем текст на экране
        }
    }

    // Вынес обновление текста в отдельный метод для чистоты кода
    void UpdateClockUI()
    {
        int hours = (CurrentTotalMinutes / 60) % 24;
        int minutes = CurrentTotalMinutes % 60;

        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }
}