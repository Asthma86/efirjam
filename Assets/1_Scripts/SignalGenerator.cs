using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignalGenerator : MonoBehaviour
{
    public TimeManager timeManager;

    [Header("UI Монитора")]
    public TextMeshProUGUI infoTextUI;
    public TextMeshProUGUI messageTextUI;

    [Header("Справочник сигналов (Связки: Кто-Кому-Текст)")]
    public List<SignalTemplate> signalTemplates; // Наш новый фиксированный список

    [Header("Прогрессия (Коды и Частоты)")]
    public List<string> baseCodes = new List<string> { "3228", "3964", "666", "777", "I<3U" };
    public List<string> phase2Frequencies = new List<string> { "97Hz", "103Hz", "115Hz" };
    public string phase3Frequency = "63Hz";
    public List<string> forbiddenWords = new List<string> { "Мятеж", "Подкуп", "Авария в шахте" };

    // Состояние: текущий сигнал, который висит на мониторе (если null - экран пуст)
    public RadioSignal currentSignal = null;

    // Метод для отрисовки сигнала на мониторе
    private void UpdateMonitorUI()
    {
        if (currentSignal == null) return;

        Debug.Log("Начинаю перенос текста на экран...");

        if (infoTextUI == null || messageTextUI == null)
        {
            Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: Поля UI пустые! Ты забыл перетащить их в Инспекторе в GameController.");
            return;
        }

        string info = $"ОТ: {currentSignal.Sender}\n" +
                      $"КОМУ: {currentSignal.Receiver}\n" +
                      $"ВРЕМЯ: {currentSignal.FormattedSentTime}\n" +
                      $"ЧАСТОТА: {currentSignal.Frequency}\n" +
                      $"КОД: {currentSignal.AuthCode}";

        infoTextUI.text = info;
        messageTextUI.text = currentSignal.MessageText;

        Debug.Log("Текст успешно отправлен в UI!");
    }

    // Метод для очистки монитора
    private void ClearMonitorUI()
    {
        infoTextUI.text = "ОЖИДАНИЕ СИГНАЛА...";
        messageTextUI.text = "";
    }

    void Start()
    {
        // Принудительно убиваем "призрачный" сигнал, который Юнити могла создать при старте
        currentSignal = null;

        ClearMonitorUI();
    }

    void Update()
    {
        // Временно для тестов: по пробелу ПЫТАЕМСЯ сгенерировать сигнал
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryGenerateSignal();
        }

        // Временно для тестов: по Enter ИМИТИРУЕМ обработку сигнала игроком (нажатие кнопки)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResolveSignal();
        }
    }

    // Метод-контроллер, который решает, можно ли выдать новый сигнал
    public void TryGenerateSignal()
    {
        // Если сигнал уже висит на экране, блокируем создание нового
        if (currentSignal != null)
        {
            Debug.LogWarning("Сигнал уже на экране! Ждем решения оператора.");
            return;
        }

        if (signalTemplates.Count == 0)
        {
            Debug.LogError("Список шаблонов пуст! Добавь их в Инспекторе.");
            return;
        }

        currentSignal = CreateSignalLogic();
        Debug.Log($"НОВЫЙ СИГНАЛ: От={currentSignal.Sender}, Кому={currentSignal.Receiver}, Время={currentSignal.FormattedSentTime}, Текст={currentSignal.MessageText}");
        
        UpdateMonitorUI();
    }

    // Метод, который мы будем вызывать при нажатии кнопок "Разрешить/Запретить"
    public void ResolveSignal()
    {
        if (currentSignal != null)
        {
            Debug.Log("Сигнал обработан. Эфир чист.");
            currentSignal = null; // Очищаем состояние, разрешая новый сигнал
            ClearMonitorUI();
        }
    }

    // Сама математика создания (скрыта от внешних скриптов)
    private RadioSignal CreateSignalLogic()
    {
        RadioSignal signal = new RadioSignal();

        // 1. Берем случайную готовую связку из нашего списка
        SignalTemplate template = signalTemplates[Random.Range(0, signalTemplates.Count)];
        signal.Sender = template.Sender;
        signal.Receiver = template.Receiver;
        string rawText = template.MessageText;

        // 2. Логика времени (75% / 25%)
        int currentTime = timeManager.CurrentTotalMinutes;
        int chance = Random.Range(0, 100);

        if (chance < 75)
        {
            int minTime = Mathf.Max(0, currentTime - 60);
            signal.SentTimeInMinutes = Random.Range(minTime, currentTime + 1);
        }
        else
        {
            int maxTime = Mathf.Max(0, currentTime - 61);
            int minTime = Mathf.Max(0, currentTime - 240);
            signal.SentTimeInMinutes = Random.Range(minTime, maxTime + 1);
        }

        // 3. Прогрессия сложности по часам
        int startMinutes = Mathf.FloorToInt(timeManager.startHour * 60);
        int hoursPassed = (currentTime - startMinutes) / 60;

        signal.AuthCode = baseCodes[Random.Range(0, baseCodes.Count)];
        signal.Frequency = "100Hz";

        if (hoursPassed >= 2)
        {
            signal.Frequency = phase2Frequencies[Random.Range(0, phase2Frequencies.Count)];
        }
        if (hoursPassed >= 4)
        {
            if (Random.Range(0f, 1f) > 0.5f) signal.Frequency = phase3Frequency;

            // Подмешиваем запрещенное слово в конец текста с шансом 30%
            if (Random.Range(0, 100) < 30)
            {
                string badWord = forbiddenWords[Random.Range(0, forbiddenWords.Count)];
                rawText = rawText + " " + badWord + ".";
            }
        }

        signal.MessageText = rawText;
        return signal;
    }
}