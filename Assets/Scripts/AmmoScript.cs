using UnityEngine;
using System.Collections;

public class AmmoScript : MonoBehaviour {
	public string ammoType = "Bazooka";
	public float speed = 5;
	private Rigidbody2D rBody;
	public GameObject explosion;
	public float timeTilExplode = 10;
	void Start () {
		//Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
		rBody = GetComponent<Rigidbody2D>();
		rBody.AddRelativeForce(Vector2.right*speed,0);
	}
	
	// Update is called once per frame
	void Update () {
		if(rBody.isKinematic == false){
			transform.right = rBody.velocity;
		}else{
			ExplosionDamage(transform.position,0.8f);

		}
		timeTilExplode -= Time.deltaTime;
		if(timeTilExplode <=0){
			StartCoroutine(Explode());
		}
	}
	void OnTriggerEnter2D(Collider2D other){
		StartCoroutine(Explode());
	}
	void ExplosionDamage(Vector2 center, float radius) {
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
		int i = 0;
		while (i < hitColliders.Length) {
			hitColliders[i].SendMessage("Destroy",SendMessageOptions.DontRequireReceiver);
			i++;
		}
	}
	IEnumerator Explode(){
		rBody.bodyType = RigidbodyType2D.Kinematic;
		rBody.gravityScale = 0;
		rBody.velocity = Vector2.zero;
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		gameObject.GetComponentInChildren<ParticleSystem>().Stop();
		GameObject ex = Instantiate(explosion,transform.position,transform.rotation) as GameObject;
		ex.gameObject.transform.SetParent(gameObject.transform);
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}

}
