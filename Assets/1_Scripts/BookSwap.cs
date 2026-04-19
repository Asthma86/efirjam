using UnityEngine;
using UnityEngine.EventSystems;

public class BookSwap : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject closedBook;
    [SerializeField] private GameObject openedBook;

    public void OnPointerClick(PointerEventData eventData)
    {
        // ≈сли закрыта€ книга активна Ц открываем
        if (closedBook.activeSelf)
        {
            closedBook.SetActive(false);
            openedBook.SetActive(true);
        }
        // »наче если открыта€ активна Ц закрываем
        else if (openedBook.activeSelf)
        {
            openedBook.SetActive(false);
            closedBook.SetActive(true);
        }
    }
}