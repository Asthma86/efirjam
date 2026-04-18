using UnityEngine;

public class FloatingCloud : MonoBehaviour
{
    [Header("Настройки")]
    public float speed = 1f; // Скорость конкретного облака
    public float leftBound = -10f; // Координата X, где облако исчезает
    public float rightBound = 10f; // Координата X, где появляется снова

    // Опционально: случайный разброс по высоте при респавне
    public bool randomizeHeight = true;
    public float minY = 2f;
    public float maxY = 5f;

    void Update()
    {
        // Двигаем облако влево
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Если облако улетело за левый край
        if (transform.position.x > rightBound)
        {
            // Вычисляем новую высоту (случайную или оставляем текущую)
            float newY = randomizeHeight ? Random.Range(minY, maxY) : transform.position.y;

            // Телепортируем на правый край
            transform.position = new Vector3(leftBound, newY, transform.position.z);
        }
    }
}
