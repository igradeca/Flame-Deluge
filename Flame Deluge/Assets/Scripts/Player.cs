using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MovingObject {

    public int maxHP = 120;
    public int maxFood = 120;
    public int maxAmmo = 120;
    public int wallDamage = 1;
    public int pointsPerAmmo = 1;
    public int pointsPerHealth = 10;
    public int pointsPerFood = 10;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public Text healthText;
    public Text ammoText;
    public Button fireButton;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int hp;
    private int food;
    private int ammo;

    public GameObject shootingMark;

    private Vector2 touchOrigin = -Vector2.one;

    private bool moveAllowed;

	// Use this for initialization
	protected override void Start () {

        animator = GetComponent<Animator>();

        hp = GameManager.instance.playerHealth;
        food = GameManager.instance.playerFoodPoints;
        ammo = GameManager.instance.playerAmmo;

        foodText.text = food.ToString();
        ammoText.text = ammo.ToString();
        healthText.text = hp.ToString();

        moveAllowed = true;

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.playerHealth = hp;
        GameManager.instance.playerFoodPoints = food;
        GameManager.instance.playerAmmo = ammo;
    }
	
	// Update is called once per frame
	void Update () {

        if (!GameManager.instance.playersTurn) return;
        if (!moveAllowed) return;

        int horizontal = 0;
        int vertical = 0;
        /*UNITY_EDITOR || */
/*#if UNITY_STANDALONE*/

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        
        if (horizontal != 0)
            vertical = 0;

//#else
        
        if (Input.touchCount > 0 && !aimingIsActive)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }

            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;

                if(Mathf.Abs(x) > 80 || Mathf.Abs(y) > 80)
                {
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        horizontal = x > 0 ? 1 : -1;
                    else
                        vertical = y > 0 ? 1 : -1;
                }
                
            }

        }

        else if (Input.touchCount == 1 && aimingIsActive)
        {
            Aim(Camera.main.ScreenToWorldPoint(Input.touches[0].position));
        }

//#endif

        if ((horizontal != 0 || vertical != 0) && !aimingIsActive)
            AttemptMove<Wall>(horizontal, vertical);

    }

    private void Aim(Vector3 target)
    {
        Collider2D[] foundedMarks = Physics2D.OverlapCircleAll(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 5);

        foreach (var item in foundedMarks)
        {
            if (item.tag == "Mark" && item.transform.position.x == Mathf.RoundToInt(target.x) && item.transform.position.y == Mathf.RoundToInt(target.y))
            {
                Vector2 start = transform.position;
                Vector2 end = new Vector2(item.transform.position.x, item.transform.position.y);

                GetComponent<BoxCollider2D>().enabled = false;

                Effect(end - start);
                shotIsActive = true;

                GetComponent<BoxCollider2D>().enabled = true;

                if (shotIsActive)
                {
                    ammo--;
                    ammoText.text = ammo.ToString();

                    RemoveShootingMarks();
                    
                    StartCoroutine(WaitForMoveToEnd(0.2f));
                }

            }
        }

    }

    protected void ScanForShootableObjects(Vector2 center, float radius)
    {

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (var item in hitColliders)
        {
            if (item.tag == "Destroyable" || item.tag == "Ghoul" || item.tag == "Raider")
            {
                aimingIsActive = true;
                Vector3 markLocation = new Vector3(item.transform.position.x, item.transform.position.y, 0);
                Instantiate(shootingMark, markLocation, Quaternion.identity);
            }            
        }

        if (aimingIsActive)
        {
            animator.SetTrigger("playerAim");
        }

    }

    protected void RemoveShootingMarks()
    {
        GameObject[] marks = GameObject.FindGameObjectsWithTag("Mark");

        foreach (var item in marks)
        {
            Destroy(item, 0);
        }

        aimingIsActive = false;
        animator.SetTrigger("playerStopAim");
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {        
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if(Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            food--;
        }

        foodText.text = food.ToString();
        healthText.text = hp.ToString();
        ammoText.text = ammo.ToString();

        CheckIfGameOver();

        StartCoroutine(WaitForMoveToEnd(0));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }

        else if(other.tag == "Food")
        {
            food += pointsPerFood;

            if (food > maxFood)
                food = 100;

            foodText.text = food + "+" + pointsPerFood;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Ammo")
        {
            ammo += pointsPerAmmo;

            if (ammo > maxAmmo)
                ammo = 10;

            ammoText.text = ammo + "+" + pointsPerAmmo;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Health")
        {
            hp += pointsPerHealth;

            if (hp > maxHP)
                hp = 120;

            healthText.text = hp + "+" + pointsPerHealth;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }

    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);            

        food--;
        animator.SetTrigger("playerChop");
    }    

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseHealth(int loss)
    {
        animator.SetTrigger("playerHit");
        hp -= loss;
        healthText.text = hp + "-" + loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (hp <= 0 || food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    private IEnumerator WaitForMoveToEnd(float seconds)
    {
        moveAllowed = false;
        yield return new WaitForSeconds(seconds);
        GameManager.instance.playersTurn = false;
        moveAllowed = true;
        shotIsActive = false;
    }    

    public void FireButtonDown()
    {
        if (aimingIsActive)
        {
            RemoveShootingMarks();            
        }

        else if (!aimingIsActive && ammo > 0)
        {
            ScanForShootableObjects(new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y), 5);            
        }
    }

}
