using UnityEngine;

[System.Serializable] // Чтобы было видно в инспекторе, если понадобится
public class RadioSignal
{
    public string Sender;
    public string Receiver;
    public string Frequency;
    public int SentTimeInMinutes;
    public string AuthCode;
    public string MessageText;

    // Свойство для красивого вывода времени отправки в UI (формат 00:00)
    public string FormattedSentTime => string.Format("{0:00}:{1:00}", (SentTimeInMinutes / 60) % 24, SentTimeInMinutes % 60);
}

[System.Serializable]
public class SignalTemplate
{
    public string Sender;
    public string Receiver;
    [TextArea(2, 5)] public string MessageText;
}