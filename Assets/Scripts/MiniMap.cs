using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniMap : MonoBehaviourPunCallbacks
{
    // Kaydırma Hızı
    [SerializeField] private float scrollSpeed = 1f;
    // Minumum Zoom Seviyesi
    [SerializeField] private float minZoomValue = 10f;
    // Maximum Zoom Seviyesi
    [SerializeField] private float maxZoomValue = 50f;    
    // Şu anki Zoom Seviyesi
    private float currentZoomValue;

    // Start is called before the first frame update
    void Start()
    {
       if (!photonView.IsMine)
       {
            gameObject.SetActive(false);
       }
    }

    void Update()
    {
        // Fare Tekelğinden gelen verileri okumamı sağlar.
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        // Kaydırmak durumuna göre zoom değişkenini ayarlamak
        if (scrollDelta > 0)
        {
            currentZoomValue += scrollSpeed;
        }
        else if (scrollDelta < 0)
        {
            currentZoomValue -= scrollSpeed;
        }

        // Kaydırma değerinin kodda tanıtılan minimum ve maksimum yakınlaştırma değerleri arasında sınırlandırılması
        // Mevcut yakınlaştırma değeri sınırlayıcı değerler arasında "sıkıştırılır" (Clamp), yani sonsuza kadar yakınlaştırıp uzaklaştıramayız
        currentZoomValue = Mathf.Clamp(currentZoomValue,minZoomValue,maxZoomValue );
        gameObject.GetComponent<Camera>().orthographicSize =  currentZoomValue ;
    }

}
