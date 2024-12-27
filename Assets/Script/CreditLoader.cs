using TMPro;
using UnityEngine;

public class CreditsLoader : MonoBehaviour
{
    [TextArea(5, 20)]
    public string creditsText = @"Titre du jeu : IObelix Adventures

Développement :
- Jules Cleret-Bissoulet
- Eduardo Costa Paulo
- Thomas Chauviré
- Matthieu Deniau
- Mathieu Clément

Game Design :
- Jules Cleret-Bissoulet
- Eduardo Costa Paulo

Graphismes :
- Eduardo Costa Paulo
- Thomas Chauviré

Programmation :
- Matthieu Deniau
- Mathieu Clément

Musique et Effets Sonores :
- Nom du compositeur ou 'Assets libres de droits'

Remerciements :
- EPITA pour le cadre de travail
- OpenAI pour l'assistance technique
- Famille et amis pour leur soutien

Un grand merci à vous d'avoir joué à notre jeu !";

    public TextMeshProUGUI creditsDisplay;

    void Start()
    {
        if (creditsDisplay != null)
        {
            creditsDisplay.text = creditsText;
        }
    }
}