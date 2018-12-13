using UnityEngine;
using System.Collections;

public class MoveTrail : MonoBehaviour {

    public int moveSpeed = 15;
    public int damage;
    public Sprite[] laserSprites;

	void Update () {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);        

        Destroy(gameObject, 1);
	}

    void OnTriggerEnter2D(Collider2D hit)
    {
        if(GetComponent<SpriteRenderer>().sprite.name == "Green laser")
        {
            if (hit.tag == "Destroyable")
            {
                Wall target = hit.GetComponent<Wall>();
                target.DamageWall(1);
                Destroy(gameObject, 0);
            }

            else if (hit.tag == "Ghoul" || hit.tag == "Raider")
            {
                Enemy target = hit.GetComponent<Enemy>();
                target.DamageEnemy(1);
                Destroy(gameObject, 0);
            }
        }

        else
        {
            if (hit.tag == "Destroyable")
            {
                Wall target = hit.GetComponent<Wall>();
                target.DamageWall(1);
                Destroy(gameObject, 0);
                //Debug.Log("Wall hit! " + hit.name);
            }

            else if (hit.tag == "Player")
            {
                Debug.Log("Damage done: " + damage);
                Player target = hit.GetComponent<Player>();
                target.LoseHealth(damage);
                Destroy(gameObject, 0);
            }
        }

    }

}
