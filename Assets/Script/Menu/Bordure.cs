using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextBoxHighlighter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image panelImage;
    public Color defaultBorderColor = new Color(0, 0, 0, 0); // Transparent par défaut
    public Color clickedBorderColor = Color.black; // Noir quand on clique

    void Start()
    {
        panelImage = GetComponent<Image>();

        // Assurez-vous que le panel a un Sprite défini (comme "UI Sprite" dans Unity)
        if (panelImage != null)
        {
            panelImage.color = defaultBorderColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (panelImage != null)
        {
            panelImage.color = clickedBorderColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (panelImage != null)
        {
            panelImage.color = defaultBorderColor;
        }
    }
}





/*using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TextOutlineChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private TMP_Text tmpText;

    public Color outlineColor = Color.black; // Couleur du contour lorsqu'on appuie
    private float defaultOutlineWidth = 0f; // Pas de contour au départ
    private float pressedOutlineWidth = 0.2f; // Épaisseur du contour quand on appuie

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();

        // Désactiver complètement l'outline au début
        tmpText.outlineWidth = defaultOutlineWidth;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        tmpText.outlineWidth = pressedOutlineWidth;
        tmpText.outlineColor = outlineColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        tmpText.outlineWidth = defaultOutlineWidth; // Enlever l'outline après le clic
    }
}*/
