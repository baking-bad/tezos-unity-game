using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMTextColorControl : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Color normalColor;
    public Color highlightColor;

    public void Start()
    {
        normalColor = GetComponent<TextMeshProUGUI>().color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<TextMeshProUGUI>().color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       GetComponent<TextMeshProUGUI>().color = normalColor ;
    }

 
}
