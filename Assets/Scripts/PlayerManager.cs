using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; 

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text infoText;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            infoText.text = "Waiting for players...";
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            infoText.text = newPlayer.NickName + " joined the game!";
            photonView.RPC("UpdateInfoText", RpcTarget.All, infoText.text);
        }
    }

    [PunRPC]
    private void UpdateInfoText(string text)
    {
        infoText.text = text;
    }
}