using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomCodeInput; // Champ pour entrer le code de la salle
    public TMP_Text statusText; // Texte optionnel pour afficher les statuts (facultatif)

    public void JoinOrCreateRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "Connexion à Photon en cours...";
            Debug.LogWarning("Le client n'est pas encore prêt. Veuillez attendre la connexion au Master Server.");
            return;
        }

        string roomCode = roomCodeInput.text;

        if (!string.IsNullOrEmpty(roomCode))
        {
            statusText.text = "Tentative de connexion à la salle...";
            PhotonNetwork.JoinRoom(roomCode); // Tente de rejoindre la salle
        }
        else
        {
            statusText.text = "Le code de la salle est vide !";
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au Master Server !");
        statusText.text = "Connecté. Prêt à rejoindre ou créer une salle.";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Rejoint la salle avec succès !");
        statusText.text = "Rejoint la salle : " + PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.LoadLevel("Map"); // Charge la scène de jeu
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Salle introuvable, création d'une nouvelle salle : {message}");
        statusText.text = "Salle introuvable. Création d'une nouvelle salle...";

        // Si la salle n'existe pas, crée une nouvelle salle
        string roomCode = roomCodeInput.text;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 }; // Configurer les options de la salle
        PhotonNetwork.CreateRoom(roomCode, roomOptions); // Crée la salle
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Salle créée avec succès !");
        statusText.text = "Salle créée avec succès ! En attente des autres joueurs...";
    }
}
