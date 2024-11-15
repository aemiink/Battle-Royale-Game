using UnityEngine;
using Photon.Pun;
//UI Çalışabilmek için gerekli olan kütüphane
using UnityEngine.UI;
public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] protected int health;
    [SerializeField] protected float attackDistance;    
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;
    protected GameObject player;    
    protected Animator anim;
    protected Rigidbody rb;
    protected float distance;
    protected float timer;  
    bool dead;
    protected GameObject[] players;
    [SerializeField] Image healthBar; 
  


    public virtual void Move() 
    {
    }
    public virtual void Attack() 
    {
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }

    [PunRPC]
    public void ChangeHealth(int count)
    {
        health -= count;
        float fillPercent = health / 100f;
        healthBar.fillAmount = fillPercent;
        if(health <= 0)
        {
            dead = true;
            GetComponent<Collider>().enabled = false;
            anim.enabled = true;
            anim.SetBool("Die", true);
        }
    }



    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject; 
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        CheckPlayers();
    }

    private void Update()
    {
        //Uzaklığı depolayacak bir değişken tanıtıyoruz
        //Mathf.Infinity - pozitif sonsuz
        float closestDistance = Mathf.Infinity;
        //Oyuncu listesinin her öğresi için tek tek çalışacak kod
        foreach (GameObject closestPlayer in players)
        {
            //Düşman ve oyuncu arasındaki uzaklığın hesaplanması
            float checkDistance = Vector3.Distance(closestPlayer.transform.position, transform.position);
            //Eğer bu oyuncuya olan mesafe bir önceki oyuncuya olan mesafeden daha az ise
            if (checkDistance < closestDistance)
            {
                //Önceki oyuncu hayattaysa
                if(closestPlayer.GetComponent<PlayerController>().dead == false)
                {
                    //Mevcut oyuncuyu en yakın oyuncu olarak kaydetme 
                    player = closestPlayer;
                    //closestDistance değerini bu oyuncuya olan uzaklık olarak değiştirme
                    closestDistance = checkDistance;
                }
            }
        }
        //player değişkeninin içinde bir oyuncu olup olmadığını kontrol etme
        //Bu kontrol hataları önlememize yardımcı olacaktır
        if (player != null)
        {
            //Kodun geri kalanı değişmedi
            distance = Vector3.Distance(transform.position, player.transform.position);
            if (!dead)
            {
                Attack();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!dead)
        {
            Move();
        }
    }

    void CheckPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Invoke("CheckPlayers", 3f);
    }
}
