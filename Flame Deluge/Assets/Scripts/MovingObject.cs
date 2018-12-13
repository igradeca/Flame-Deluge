using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    public bool aimingIsActive = false;
    public GameObject laserTrailPrefab;
    public bool shotIsActive = false;

	// Use this for initialization
	protected virtual void Start () {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 5f / moveTime;
	
	}

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end)      // IEnumetator se koristi za npr. male pomake da nam animacija bude vidljiva, inace bi lokacija lika odma se promjenila bez da vidimo to
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        if (gameObject.activeSelf)
        {
            RaycastHit2D hit;
            bool canMove = Move(xDir, yDir, out hit);

            if (hit.transform == null)      // ako se pomaknul - kraj metode
                return;

            T hitComponent = hit.transform.GetComponent<T>();

            if (!canMove && hitComponent != null)
                OnCantMove(hitComponent);
        }        

    }

    protected abstract void OnCantMove <T> (T component)
        where T : Component;    

    public void Effect(Vector2 target)
    {
        target.Normalize();
        float rotZ = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        GameObject trail = Instantiate(laserTrailPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0f, 0f, rotZ)) as GameObject;

        if(gameObject.tag == "Player")
        {
            trail.GetComponent<SpriteRenderer>().sprite = trail.GetComponent<MoveTrail>().laserSprites[0];            
        }
        else
        {
            trail.GetComponent<SpriteRenderer>().sprite = trail.GetComponent<MoveTrail>().laserSprites[1];
            trail.GetComponent<MoveTrail>().damage = gameObject.GetComponent<Enemy>().playerDamage;
            //Debug.Log("Damage to send:" + gameObject.GetComponent<Enemy>().playerDamage);
        }
            
    }

}
