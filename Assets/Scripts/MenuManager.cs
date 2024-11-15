using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// Photon hizmetlerinin Fonksiyonları
using Photon.Pun;

// Kalıtım Değiştirme
public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField]TMP_Text logText;
    [SerializeField] TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        // Oyuncuya rastgele sayı etiketli bir kullanıcı adı verelim
        PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);
        // Log yazı alanında bu kullanıcı adını gösterelim
        Log("Player Name: " + PhotonNetwork.NickName);
        // Oyun ayarlarını yapalım
        PhotonNetwork.AutomaticallySyncScene = true; // Pencereler arasındaki otomatik geçiş
        PhotonNetwork.GameVersion = "1"; // Oyun versiyonunu ayarlama
        PhotonNetwork.ConnectUsingSettings(); // Photon sunucusuna bağlanma
    }

    void Log(string message)
    {
        //Alt satıra geçme
        logText.text += "\n";
        logText.text += message;
    }

    public void ChangeName()
    {
        //InputField alanına yazılan yazıyı okumak
        PhotonNetwork.NickName = inputField.text;
        //Yeni kullanıcı adını çıktı vermek:
        Log("New Player name: " + PhotonNetwork.NickName);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 15 });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnConnectedToMaster()
    {
        Log("Connected to the server");
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the lobby");
        PhotonNetwork.LoadLevel("Lobby");
    }
}
