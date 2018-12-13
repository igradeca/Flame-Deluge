using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Enemy : MovingObject {

    public int playerDamage;
    public int hp = 4;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    protected override void Start () {

        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();        

        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
        	
	} 

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.y > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        int enemyAction = Random.Range(0, 100);

        if(enemyAction > 50 && gameObject.tag == "Raider")
        {
            ScanForPlayer(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), 4f);
        }
        else
            base.AttemptMove<T>(xDir, yDir);        

        skipMove = true;
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("EnemyChop");

        hitPlayer.LoseHealth(playerDamage);

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    protected void ScanForPlayer(Vector2 center, float radius)
    {
        if (gameObject.activeSelf)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

            foreach (var item in hitColliders)
            {
                if (item.tag == "Player")
                {
                    Vector2 playerTarget = new Vector2(item.transform.position.x, item.transform.position.y);
                    AimForPlayer(playerTarget);
                }
            }
        }

    }

    private void AimForPlayer(Vector2 playerTarget)
    {
        Vector2 start = transform.position;

        GetComponent<BoxCollider2D>().enabled = false;

        Effect(playerTarget - start);

        GetComponent<BoxCollider2D>().enabled = true;

    }

    public void DamageEnemy(int loss)
    {

        animator.SetTrigger("EnemyDamaged");
        hp -= loss;

        if (hp <= 0)
            gameObject.SetActive(false);
    }

}
