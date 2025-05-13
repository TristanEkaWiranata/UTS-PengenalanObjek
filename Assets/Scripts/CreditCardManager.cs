using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreditCardManager : MonoBehaviour
{
    [System.Serializable]
    public class MemberInfo
    {
        public string nama;
        public string nim;
        public string jobdesk;
    }

    public List<MemberInfo> members = new List<MemberInfo>();

    public GameObject cardPrefab;
    public Transform cardContainer;

    public Button buttonNext;
    public Button buttonPrev;

    private int currentIndex = 0;
    private GameObject currentCard;

    void Start()
    {
        ShowCard(currentIndex);

        buttonNext.onClick.AddListener(() => ShowNext());
        buttonPrev.onClick.AddListener(() => ShowPrevious());
    }

    void ShowCard(int index)
    {
        if (currentCard != null)
            Destroy(currentCard);

        currentCard = Instantiate(cardPrefab, cardContainer);

        Text[] texts = currentCard.GetComponentsInChildren<Text>();
        if (texts.Length >= 3)
        {
            texts[0].text = "Nama  : " + members[index].nama;
            texts[1].text = "NIM   : " + members[index].nim;
            texts[2].text = "Job   : " + members[index].jobdesk;
        }
    }

    void ShowNext()
    {
        currentIndex = (currentIndex + 1) % members.Count;
        ShowCard(currentIndex);
    }

    void ShowPrevious()
    {
        currentIndex = (currentIndex - 1 + members.Count) % members.Count;
        ShowCard(currentIndex);
    }
}
