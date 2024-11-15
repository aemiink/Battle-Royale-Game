using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Photon Kütüphanesi
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

// kalıtım değişti
public class GameManager : MonoBehaviourPunCallbacks
{

    [SerializeField] List <Transform> spawnsTurret = new List <Transform>();
    [SerializeField] List <Transform> spawnsWalk = new List <Transform>();
    [SerializeField] List <Transform> spawns = new List <Transform>();
    int randSpawn;
    // Yazı elemanına bir referans
    [SerializeField] public TMP_Text playersText;
    // Oyuncuları depolayacak bir array
    GameObject[] players;
    // Aktif oyuncuların kullanıcı adlarını depolayacak bir liste
    List<string> activePlayers = new List<string>();
    int checkPlayers = 0;
    private int previousPlayerCount;

    // Start is called before the first frame update
    void Start()
    {
        // Rastgele Bir Sayı Seçmek
        randSpawn = Random.Range(0, spawns.Count);
        PhotonNetwork.Instantiate("Player", spawns[randSpawn].position, spawns[randSpawn].rotation);
        Invoke("SpawnEnemy", 5f);
        previousPlayerCount = PhotonNetwork.PlayerList.Length;

    }

    void Update()
    {
        if (PhotonNetwork.PlayerList.Length < previousPlayerCount)
        {
            ChangePlayersList();
        }
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }

    public void SpawnEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnsWalk.Count; i++)
            {
                PhotonNetwork.Instantiate("WalkEnemy", spawnsWalk[i].position, spawnsWalk[i].rotation); 
            }
            
            for (int i = 0; i < spawnsTurret.Count; i++)
            {
                PhotonNetwork.Instantiate("Turret", spawnsTurret[i].position, spawnsTurret[i].rotation); 
            }
        }

    }

    public void ChangePlayersList()
    {
        photonView.RPC("PlayerList", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers.Clear();
        foreach(GameObject player in players)
        {
            // Eğer oyuncu hayattaysa
            if (player.GetComponent<PlayerController>().dead == false)
            {
                // activePlayers listesine kullanıcı adının eklenmesi
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);
            }
        }
        //
        playersText.text = "Players in game : " + activePlayers.Count.ToString();

        if (activePlayers.Count <= 1 && checkPlayers > 0)
        {
            PlayerPrefs.SetString("Winner", activePlayers[0]);
            // Oyundaki bütün düşmanları arayıp bir listede tutmak
            var enemies = GameObject.FindGameObjectsWithTag("enemy");
            // Listedeki düşmanlara sırasıyla bakmak
            foreach (GameObject enemy in enemies)
            {
                // Listedeki her düşmana 100 zarar vermek. Eğer 100'den fazla canı olan düşmanlar varsa bu sayıyı ona göre düzenleyin!
                enemy.GetComponent<Enemy>().ChangeHealth(100);
            }
            Invoke("EndGame", 5f);
        }
        checkPlayers++;
    }

    void EndGame()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //Menü Sahnesninin yüklenmesi
        SceneManager.LoadScene(0);
        // Oyuncu listesinin güncellenmesi
        ChangePlayersList();
    }
}
