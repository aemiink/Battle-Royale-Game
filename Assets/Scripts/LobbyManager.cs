using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text ChatText;
    [SerializeField] TMP_InputField InputText;
    [SerializeField] TMP_Text PlayersText;
    // Oluşturudğum Start Butonunu Depolayacak Değişken
    [SerializeField] GameObject startButton; 

    public void Send()
    {

        if (string.IsNullOrWhiteSpace(InputText.text))
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            photonView.RPC("Log", RpcTarget.All, PhotonNetwork.NickName + ":" + InputText.text);
            InputText.text = string.Empty;
        }
    }


    [PunRPC]
    void Log(string message)
    {
        //Alt satıra geçme
        ChatText.text += "\n";
        ChatText.text += message;
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // Belirli bir kullanıcı adına sahip bir oyuncunun odadan ayrıldığını bildiren bir mesaj çıktısı alma
        Log(otherPlayer.NickName + " left the room");
        RefreshPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Belirli bir kullanıcı adına sahip bir oyuncunun odaya girdiğini bildiren bir mesaj çıktısı
        Log(newPlayer.NickName + " entered the room");
        RefreshPlayers();
    }

     void RefreshPlayers()
    {
        // Çağrı yalnızca Ana İstemci (sunucuyu oluşturan oyuncu) tarafından yapılabilir
        if (PhotonNetwork.IsMasterClient)
        {
            // Lobideki tüm oyuncular için ShowPlayers yöntemini çağırma
            photonView.RPC("ShowPlayers", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ShowPlayers()
    {
        // Oyuncu listesini temizleme, sadece 'Players:' satırını bırakma
        PlayersText.text = "Players: ";
        // Sunucudaki tüm oyuncuların için çalışacak bir döngü başlatma
        foreach (Photon.Realtime.Player otherPlayer in PhotonNetwork.PlayerList)
        {
            // Sonraki satıra geçiş
            PlayersText.text += "\n";
            // Kullanıcı adını çıktı vermek
            PlayersText.text += otherPlayer.NickName;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshPlayers();
        // Eğer Photon Ağında Master Client Değilse
        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(false);
        }

        // PlayerPrefs'te "Winner" anahtarı altında kaydedilmiş bir değerimiz varsa ve oyuncu master clientsa
        if (PlayerPrefs.HasKey("Winner") && PhotonNetwork.IsMasterClient)
        {
            // Kazananın takma adını saklayacak geçici bir değişken oluşturma
            string winner = PlayerPrefs.GetString("Winner");
            // Son maçı kazanan oyuncunun adını görüntülemek için sohbet mesajı fonksiyonunu çağırma
            photonView.RPC("ShowMessage", RpcTarget.All, "The last match was won by: " + winner);
            // Aynı mesajın tekrarlanmaması için PlayerPrefs'ten her şeyi silme
            PlayerPrefs.DeleteAll();
        }

    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
