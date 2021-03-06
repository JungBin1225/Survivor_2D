using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter player;

    public List<int> met_enemy_list;
    public List<int> enemy_kill_list;

    public GameObject playerUI;
    public GameObject farmingUI;

    public int enemyType;
    public float Hp;
    public float Mt;
    public float totalAir;
    public float Air;

    public int power;
    public int AtkDamage;
    public int defense;
    public int accuracy;
    public int avoid;
    public float critical;
    public int speed;

    public bool isDead;
    public bool isRun;
    public bool isAttack;
    public bool isCritical;
    public bool canMove;
    public bool isBattle;
    public bool isHome;
    public bool isReward;

    public bool endBattle;

    public int exp;

    public string booty_item;
    

    public Image HpBar;
    public Image FarmingBar;

    public Text normalDamageText;

    public Text criticalDamageText;
    public Text missText;

    public Text causeDeathText;

    Rigidbody2D rigid;

    CapsuleCollider2D capsuleCol;
    public EnemyCharacter enemy;

    
    public bool isWin;
    public bool killenemy;
    public int enemyCount;

    public int killCount;

    public Vector2 playerPos;
    // Start is called before the first frame update

    //playerMove

    public float maxSpeed;
    public bool isFarming;
    public bool isFarmDone;
    public bool isblocked;
    private int farmingTimer;
    //public GameObject farm;
    //public GameObject farmEnd;
    //public GameObject AIR;

    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake() 
    {
        if (player == null)
            player = this;

        else if (player != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        rigid = GetComponent<Rigidbody2D>();
        capsuleCol=GetComponent<CapsuleCollider2D>();
        AtkDamage = power;
        canMove = true;
        isAttack = true;

        //playerMove
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //farmEnd.gameObject.SetActive(false);

        maxSpeed = 1.0f;

        isFarming = false;
        isblocked = false;
        isFarmDone = false;
        farmingTimer = 0;
        

        //??????...
        
        
    }
    void Start()
    {
        
    }
    public void getVictoryReward()
    {
        exp+=enemy.exp;
        booty_item=enemy.booty_item;
        killCount++;
        killenemy=true;
        isWin=true;
        enemy_kill_list.Add(1);
        isReward=true;
        // Debug.Log("?????????");
        // Debug.Log(exp);
        // Debug.Log("?????????");
        // Debug.Log(booty_item);
    }

    void getRunPenalty()
    {
        Mt=Mt-5;
        enemy_kill_list.Add(0);
    }

    void decreaseAir()
    {
        Air -= Time.deltaTime;
    }
    void checkDead()
    {
        if (Hp <= 0)
        {
            
            //player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            Invisible(0.4f*Time.deltaTime);
           
           
            isDead = true;
            //Time.timeScale=0;
            StartCoroutine("showDeathText","??????");
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        else if(Mt<=0){
            //player.transform.rotation = new Quaternion(0, 0, 45, 0);
            Invisible(0.2f);
            isDead =true;
            StartCoroutine("showDeathText","??????");
        }
        else if(Air<=0){
            //player.transform.rotation = new Quaternion(0, 0, 45, 0);
            Invisible(0.2f);
            isDead =true;
            
            StartCoroutine("showDeathText","??????");
            
            
        }
    }
    void checkAttack()
    {
        float check=1;
        check = (accuracy - enemy.avoid + 30) / 100 * ((int)Air / totalAir)*100; //%????????? ?????? *100
        
        
        int tmp=Random.Range(1,101);  //1~100

        if(tmp<=check){
            isAttack=true;
        }
        else if(tmp>check){
            isAttack=false;
        }
    }

    void checkCritical()
    {
        float check;
        check = (critical / 100)*(totalAir / Air)*100;  //%????????? ?????? *100
        //Debug.Log((int)check); 10%

        int tmp=Random.Range(1,101);  //1~100

        if(tmp<=check){
            isCritical=true;
        }
        else if(tmp>check){
            isCritical=false;
        }
        
    }
    IEnumerator textDamage(int damage)
    {

        normalDamageText.gameObject.SetActive(true);
        normalDamageText.text = "-" + damage.ToString() + "\n";
        yield return new WaitForSeconds(1.0f);
        normalDamageText.gameObject.SetActive(false);   
    }
    IEnumerator textMiss()
    {
        missText.gameObject.SetActive(true);
        missText.text="Miss";
        yield return new WaitForSeconds(1.0f);
        missText.gameObject.SetActive(false);
    }

    IEnumerator textCritical(int damage)
    {
        criticalDamageText.gameObject.SetActive(true);
        criticalDamageText.text="-"+damage.ToString()+"\n";
        yield return new WaitForSeconds(1.0f);
        criticalDamageText.gameObject.SetActive(false);
    }


    IEnumerator showDeathText(string cause)
    {
        causeDeathText.gameObject.SetActive(true);
        causeDeathText.text=cause+"??? ?????? ?????????????????????!....";
        canMove=false;
        //isTrigger ???????????? ??? ?????? ??????
        capsuleCol.isTrigger=true;
        rigid.constraints = RigidbodyConstraints2D.FreezePositionY; 
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX; 
        gameObject.GetComponent<PlayerMove>().enabled=false;

        yield return new WaitForSeconds(4.0f);
        causeDeathText.gameObject.SetActive(false);
        
        gameObject.SetActive(false);
        
    }

    void GetDamage()
    {
        if (enemy.isAttack == true)
        {
            if (enemy.isCritical == true)
            {
                if((enemy.AtkDamage - defense) <= 0)
                {
                    //????????? ?????? ????????????????????? ???????????? ?????? ?????????0??? ??????
                    int damage=0;
                    HpBar.fillAmount = (float)Hp / 100;
                    /*rigid.AddForce(new Vector2(0,enemy.speed * 20));*/
                    StartCoroutine("textDamage",damage);
                }
                else
                {
                    //????????? ?????? ???????????? ????????? ?????? ??????
                    //(enemy.AtkDamage - defense)
                    int damage=enemy.AtkDamage-defense;
                    Hp = Hp - damage;
                    HpBar.fillAmount = (float)Hp / 100;
                    StartCoroutine("textCritical",damage);
                }
               
            }
            else
            {
                //????????? ?????? ???????????? ???????????? ???????????? ???????????? ?????? ?????????0??? ??????
                if ((enemy.AtkDamage - defense) <= 0)
                {
                    //Hp = Hp;
                    int damage=0;
                    HpBar.fillAmount = (float)Hp / 100;
                    StartCoroutine("textDamage",damage);
                }
                else
                {
                    //????????? ?????? ???????????? ???????????? ?????? ??????
                    int damage=enemy.AtkDamage-defense;
                    Hp = Hp - damage;
                    HpBar.fillAmount = (float)Hp / 100;
                    StartCoroutine("textDamage",damage);
                }
            }
        }
        else 
        //????????? ????????? ??????
        {
            StartCoroutine("textMiss");

        }

        Nulkback();

    }

    void PlayerAvoid()
    {

    }

    void Invisible(float i)
    {
        player.spriteRenderer.color = new Color(255, 255, 255, player.spriteRenderer.color.a - i);
    }

    IEnumerator StopMove()
    {
        canMove = false;
        yield return new WaitForSeconds(0.8f);
        //Debug.Log(1);
        canMove = true;
    }
    void Nulkback()
    {
        rigid.AddForce(new Vector2(-enemy.speed * 20, enemy.speed * 50));

        StartCoroutine("StopMove");

    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "trap")
        {
            isblocked = true;
        }


        if (other.gameObject.tag == "Enemy")
        {
            checkAttack();
            checkCritical();
            if (isAttack == true)
            {
                if (isCritical == true)
                {
                    AtkDamage = (int)(power * 1.5);
                    
                }
                else
                {
                    AtkDamage = power;
                   
                }
            }

            GetDamage();
        }
        else if (other.gameObject.tag == "RunTrigger")
        {
            isRun = true;
            isWin=false;
            killCount=0;
            getRunPenalty();
        }

        else if (other.gameObject.tag=="EnemyType_1")
        {
            
            enemyType=1;
            
            enemyCount++;

            isBattle=true;
            met_enemy_list.Add(1);

            
        }

        else if (other.gameObject.tag=="EnemyType_2")
        {
            //this.spriteRenderer.sprite=rightSprite;
            enemyType=2;
            
            enemyCount++;

            isBattle=true;
            met_enemy_list.Add(2);

            
        }
        

        else if(other.gameObject.tag=="EnemyBlock")
        {
            endBattle=true;
            killCount=0;
            //isWin=true;
            
        }

        else if(other.gameObject.tag=="Home")
        {
            isHome=true;

            
        }

        else if(other.gameObject.tag=="trap")
        {
            isblocked=true;
        }




    }
    
    void Move()
    {
        if (canMove == true)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }     
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
            }

            //???????????? ?????????????????? ????????? ???????????? 
            if ((isBattle==false)&(SceneManager.GetActiveScene().name=="Outside"))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    transform.Translate(Vector3.up * speed * Time.deltaTime);
                    //anim.SetBool("isWalking", true);

                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime);
                    //anim.SetBool("isWalking", true);

                }    
            }
            
        }
    }

    public void SavePlayerPos(float xpos, float ypos)
    {
        playerPos=new Vector2(player.transform.position.x+xpos,player.transform.position.y+ypos);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        checkDead();
        Farming();
        //decreaseAir();
        

        if(SceneManager.GetActiveScene().name=="Battle"){
            gameObject.GetComponent<PlayerMove>().enabled=false;
            if(isDead==true)
            {
                rigid.gravityScale = -1.5f;
                playerUI.SetActive(false);
            }
            else
            {
                rigid.gravityScale = 1;
                playerUI.SetActive(true);
            }
            
            if(GameObject.FindGameObjectWithTag("Enemy")!=null){
                enemy=GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyCharacter>();
            }
            decreaseAir();
            
            
        }


        if(SceneManager.GetActiveScene().name=="Outside"){
            gameObject.GetComponent<PlayerMove>().enabled=true;
            playerUI.SetActive(false);
            decreaseAir();
            rigid.gravityScale=0;
            
        }

        if(SceneManager.GetActiveScene().name=="Home"){
            playerUI.SetActive(false);
            
            rigid.gravityScale=0;
        }
        
        if (isblocked)
        {
            getBack();
            isblocked = false;
        }
        if (isFarming)
        {
            if (farmingTimer > 3000)
            {
                Invoke("getItem", 3);
                isFarming = false;
                isFarmDone = true;
                farmingTimer = 0;
                Debug.Log("????????? ??????");

            }

            else
            {
                farmingTimer++;
                //Debug.Log(farmingTimer);
            }
        }


    }

    //playerMove
    void getItem()
    {
        int item = 3;
        //farmEnd.gameObject.SetActive(true);
        //farm.gameObject.SetActive(false);
        Debug.Log("item????????" + item);
    }
    void getBack()
    {
        rigid.position = new Vector2(rigid.position.x - 2.5f, rigid.position.y);
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "farm")
        {
            if (!isFarmDone) isFarming = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "farm")
        {
            isFarming = false;
            farmingTimer = 0;
        }
    }

  
    
    public void Farming()
    {
        if(isFarming==true)
        {
            //StartCoroutine("FarmingUI");
            farmingUI.SetActive(true);
            FarmingBar.fillAmount = FarmingBar.fillAmount + Time.deltaTime * 0.5f;
        }
        else
        {
            farmingUI.SetActive(false);
            FarmingBar.fillAmount = 0;
        }
    }
}
