using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("Max Values")]
    public int maxHp;
    public int maxJumps;
    public float curAttackTime;
    public float slowTime;
    public float maxSpeed;
    public float max_charge_dmg;

    [Header("Current Values")]
    public int curHp;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public float lastHit;
    public float lastHitIce;
    public bool isSlowed;
    public float charge_dmg;
    public float charge_rate;
    public bool ischarging;

    [Header("Attacking")]
    public PlayerController curAttacker;
    public float attackDmg;
    public float attackSpeed;
    public float iceAttackSpeed;
    public float attackRate;
    public float lastAttackTime;
    public GameObject[] attackPrefabs;


    [Header("Mods")]
    public float moveSpeed;
    public float jumpForce;


    [Header("Audio clips")]
    //jump and 0
    // land snd 1
    // taunt 1 2
    public AudioClip[] playerfx_list;

    internal void takeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource audio;
    private Transform muzzle;
    private GameManager gameManager;
    private PlayerContainerUI playerUI;
    public GameObject deatheffectprefab;


    // unity life cycle methods
    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        muzzle = GetComponentInChildren<Muzzle>().GetComponent<Transform>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        curHp = maxHp;
        curJumps = maxJumps;
        score = 0;
        moveSpeed = maxSpeed;
        
    }

    private void FixedUpdate()
    {
        move();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10 || curHp <= 0)
        {
            die();
        }

        if (curAttacker)
        {
            if (Time.time - lastHit > curAttackTime)
            {
                curAttacker = null;
            }
        }


        if (isSlowed)
        {
            if (Time.time - lastHitIce > slowTime)
            {
                isSlowed = false;
                moveSpeed = maxSpeed;
            }



        }

        if (ischarging)
        {
            charge_dmg += charge_rate;
            if(charge_dmg > max_charge_dmg)
            {
                charge_dmg = max_charge_dmg;
            }
            playerUI.updateChargeBar(charge_dmg, max_charge_dmg);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //resetting curJumps when I hit the ground
        foreach (ContactPoint2D hit in collision.contacts)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                if (hit.point.y < transform.position.y)
                {
                    audio.PlayOneShot(playerfx_list[2]);
                    curJumps = maxJumps;
                }
            }
            if (hit.point.x > transform.position.x || hit.point.x < transform.position.x && hit.point.y < transform.position.y)
            {
                if(maxJumps == 0)
                {
                    curJumps++;
                }
                
            }

        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
     
    }

    //action methods

    


    private void move()
    {
        
        // sets the velocity on rig x to what ever the cur move input is and multiply by move speed
        rig.velocity = new Vector2(curMoveInput * moveSpeed, rig.velocity.y);

        // player direction
        if(curMoveInput != 0)
        {
            if (curMoveInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            


            //transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }

        if (ischarging)
        {
            isSlowed = true;
            charge_dmg += charge_rate;
            if (charge_dmg > max_charge_dmg)
            {

            }
        }
    }

    private void jump()
    {
        // play jump sound
        audio.PlayOneShot(playerfx_list[0]);
        rig.velocity = new Vector2(rig.velocity.x, 0);
        //add force up
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void die()
    {
        Destroy (Instantiate(deatheffectprefab, transform.position, Quaternion.identity), 1f);
        print("player has died");
        // play die sound
        audio.PlayOneShot(playerfx_list[4]);
        if(curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;

            if(score < 0)
            {
                score = 0;
            }
        }
  
        respawn();
    }


    public void drop_out()
    {
        Destroy(playerUI.gameObject);
        Destroy(gameObject);
    }
    public void addScore()
    {
        score++;
        playerUI.updateScoreText(score);
    }

    public void takeDamage (int ammount, PlayerController attacker)
    {
        curHp -= ammount;
        curAttacker = attacker;
        lastHit = Time.time;
        if (ischarging)
        {
            charge_dmg /= 2;

        }
        playerUI.updateHealthBar(curHp, maxHp);

    }

    //over load method to take float
    public void takeDamage(float ammount,PlayerController attacker)
    {
        curHp -= (int)ammount;
        curAttacker = attacker;
        lastHit = Time.time;
        if(ischarging)
        {
            charge_dmg /= 2;

        }
        playerUI.updateHealthBar(curHp, maxHp);
    }

    public void takeIceDamage(float ammount, PlayerController attacker)
    {
        curHp -= (int)ammount;
        curAttacker = attacker;
        lastHit = Time.time;
        isSlowed = true;
        lastHit = Time.time;
        lastHitIce = Time.time;
        moveSpeed /= 2;
        if (ischarging)
        {
            charge_dmg /= 2;

        }
        playerUI.updateHealthBar(curHp, maxHp);

    }

    private void respawn()
    {
        curHp = maxHp;
        curJumps = maxJumps;
        curAttacker = null;
        rig.velocity = Vector2.zero;
        transform.position = gameManager.spawn_points[Random.Range(0, gameManager.spawn_points.Length)].position;
    }


    // input action map methods
    //move input methods
    public void onMoveInput(InputAction.CallbackContext context)
    {
        float x = context.ReadValue<float>();
        if (x > 0)
        {
            curMoveInput = 1;
        }
        else if(x <0 )
        {
            curMoveInput = -1;
        }
        else
        {
            curMoveInput = 0;
        }


    }


    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (curJumps > 0)
            {
                curJumps--;
                jump();
            }
        }


    }

    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block button");
        }



    }

    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause button");
        }
    }

    public void onStdAtachInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed&& Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawn_std_attack(attackDmg,attackSpeed);

        }

        if (ischarging)
        {
            ischarging = false;
            charge_dmg = 0;
        }
    }

    public void spawn_std_attack(float dmg,float speed)
    {
        print("standard attack");
        GameObject fireBall = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireBall.GetComponent<Projectile>().onSpawn(attackDmg, attackSpeed, this, transform.localScale.x);
    }

    public void spawn_ice_attack()
    {
        GameObject iceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceBall.GetComponent<Projectile>().onSpawn(attackDmg, iceAttackSpeed, this, transform.localScale.x);
    }

    public void spawn_charge_attack()
    {
        GameObject chargeBall = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        chargeBall.GetComponent<Projectile>().onSpawn(charge_dmg, attackSpeed, this, transform.localScale.x);
    }

    public void setUI(PlayerContainerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void onChrAtachInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // togglee on charging bool
            ischarging = true;
            moveSpeed /= 2;
            
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            // toggle off charging bool
            ischarging = false;
         
            // spawn attack
            spawn_charge_attack();
            // set dmg value back to 0
            charge_dmg = 0;
            moveSpeed = maxSpeed;
            playerUI.updateChargeBar(charge_dmg, max_charge_dmg);

        }



    }

    public void onAttackIceInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate*2)
        {
            lastAttackTime = Time.time;
            spawn_ice_attack();
        }
        if (ischarging)
        {
            ischarging = false;
            charge_dmg = 0;
        }
    }

    public void onTaunt1Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            audio.PlayOneShot(playerfx_list[1]);
        }



    }

    public void onTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt 2button");
        }



    }


    public void onTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt 3 button");
        }



    }


    public void onTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt 4 button");
        }



    }
}
