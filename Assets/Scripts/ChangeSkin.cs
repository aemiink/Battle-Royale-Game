using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeSkin : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject male;
    [SerializeField] GameObject female;
    [SerializeField] bool isMale;
    // Start is called before the first frame update
    void Start()
    {
        Replace(isMale);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isMale = !isMale;
                Replace(isMale);
            }
        }

    }


    public void Replace(bool value)
    {
        male.SetActive(value);  
        female.SetActive(!value);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isMale);
        }
        else
        {   
            isMale = (bool)stream.ReceiveNext();  
            Replace(isMale);      
        }
    }
}
