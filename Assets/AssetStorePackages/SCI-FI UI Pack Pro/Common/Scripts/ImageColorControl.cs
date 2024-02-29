
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class ImageColorControl : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Color normalColor;
    public Color highlightColor;

    public Image theImage;

    public void Start()
    {
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        theImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theImage.color = normalColor ;
    }

 
}
