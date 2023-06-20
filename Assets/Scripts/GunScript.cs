using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {
	public string gunType = "Bazooka";
	public float reloadTime = 1.0f;
	public float timer;
	public GameObject ammunition;
	public GameObject player;
	public int ammo = 3;
	private Rigidbody2D rBody;
	public Sprite pPistol;
	public Sprite pPistolLeft;
	public Sprite flashing;
	public float recoil = 0.2f;
	private LineRenderer lineRenderer;
	public float recoilSpeed = 10f;
	private PlayerScript pScript;
	public Transform socket;
	public GameObject explosion;
	public Transform gunSprite;
	// Use this for initialization
	void Start () {
		rBody = GetComponent<Rigidbody2D>();
		if (gunType == "Pistol") {
			gunSprite = transform.Find("Pistol2");
		}
		if(gunType =="Pistol" || gunType == "Rifle"){
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.enabled = false;
		}

	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if(timer <=0){
			timer = 0;
		}
		if(ammo <=0){
			Destroy(gameObject);
		}
		if(gunType == "Pistol" || gunType == "Rifle"){
			if(timer <= reloadTime - 0.1f){
				lineRenderer.enabled = false;
			}
		}
		if (gunSprite) {
			gunSprite.localRotation = Quaternion.Lerp (gunSprite.localRotation, new Quaternion (0, 0, 0, 1), Time.deltaTime * 5);
		}
	}
	//RaycastHit2D hit;
	public void Shoot(){
		
		if(gunType =="Bazooka"){
			GameObject bullet = Instantiate(ammunition,transform.position,transform.rotation)as GameObject;
			Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
		}
		if(gunType =="Pistol"|| gunType == "Rifle"){
			recoil = 1.9f;

			lineRenderer.enabled = true;
			RaycastHit2D hit;
			if(player.GetComponent<PlayerScript>().facing == "Right"){
				hit = Physics2D.Raycast(new Vector2(socket.position.x +1 ,socket.position.y), socket.TransformDirection(Vector2.right), 200f);
				print (socket.eulerAngles);
			}else{
				hit = Physics2D.Raycast(new Vector2(socket.position.x -1,socket.position.y), socket.TransformDirection(Vector2.right), 200f);
			}
			if (hit.collider != null && hit.collider.gameObject.tag == "Player"){
				print (hit.collider.name);
				Destroy (hit.transform.gameObject);
				lineRenderer.SetPosition(0, socket.position);
				lineRenderer.SetPosition(1,hit.point);
			}else{
				Ray ray = new Ray(new Vector2(transform.position.x,transform.position.y), transform.TransformDirection(Vector3.right));
				lineRenderer.SetPosition(0, socket.position);
				lineRenderer.SetPosition(1,ray.GetPoint(20f));
			}
			Recoil (5);
		}
		if(gunType == "Grenade"){
			Drop ();
			player.GetComponent<PlayerScript>().gun = null;
			rBody.AddRelativeForce(Vector3.right * 500);
			rBody.gravityScale = 0.5f;
			StartCoroutine(Explode());
		}else{

		ammo --;
		}
		timer = reloadTime;

	}

	void Recoil(float power){
		if (player.GetComponent<PlayerScript> ().facing == "Right") {
			gunSprite.localRotation = Quaternion.Lerp (gunSprite.localRotation, new Quaternion (0, 0, gunSprite.localRotation.z + power, 0), Time.deltaTime * power);
		}
		if (player.GetComponent<PlayerScript> ().facing == "Left") {
			gunSprite.localRotation = Quaternion.Lerp (gunSprite.localRotation, new Quaternion (0, 0, gunSprite.localRotation.z - power, 0), Time.deltaTime * power);
		}
	}
	public void Drop(){
		//rBody.gravityScale = 1;
		gameObject.layer = 9;
		rBody.isKinematic = false;
		gameObject.GetComponent<Collider2D>().enabled = true;
		gameObject.GetComponent<Collider2D>().isTrigger = true;
		rBody.bodyType = RigidbodyType2D.Kinematic;
	}
	void OnCollisionStay2D(Collision2D collision){
		foreach(ContactPoint2D contact in collision.contacts){
			if(contact.normal == Vector2.up){
				if(gunType !="Grenade"){
					rBody.gravityScale = 0;
					gameObject.layer = 0;
					gameObject.GetComponent<Collider2D>().enabled = true;
					rBody.isKinematic = true;
					gameObject.GetComponent<Collider2D>().isTrigger = true;
				}
			}
		}
	}
	IEnumerator Explode(){
		yield return new WaitForSeconds(1f);
		gameObject.GetComponent<SpriteRenderer>().sprite = flashing;
		yield return new WaitForSeconds(0.8f);
		gameObject.GetComponent<SpriteRenderer>().sprite = pPistol;
		yield return new WaitForSeconds(0.7f);
		gameObject.GetComponent<SpriteRenderer>().sprite = flashing;
		yield return new WaitForSeconds(0.6f);
		gameObject.GetComponent<SpriteRenderer>().sprite = pPistol;
		yield return new WaitForSeconds(0.5f);
		gameObject.GetComponent<SpriteRenderer>().sprite = flashing;
		yield return new WaitForSeconds(0.4f);
		gameObject.GetComponent<SpriteRenderer>().sprite = pPistol;
		yield return new WaitForSeconds(0.3f);
		gameObject.GetComponent<SpriteRenderer>().sprite = flashing;
		yield return new WaitForSeconds(0.2f);
		gameObject.GetComponent<SpriteRenderer>().sprite = pPistol;
		rBody.freezeRotation = true;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		ExplosionDamage(transform.position,2);
		GameObject ex = Instantiate(explosion,transform.position,transform.rotation) as GameObject;
		ex.gameObject.transform.SetParent(gameObject.transform);
		rBody.gravityScale = 0;
		rBody.velocity = Vector2.zero;

		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
	void ExplosionDamage(Vector2 center, float radius) {
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
		int i = 0;
		while (i < hitColliders.Length) {
			hitColliders[i].SendMessage("Destroy",SendMessageOptions.DontRequireReceiver);
			i++;
		}
	}
	void OnDrawGizmos() {
		//if(rBody.isKinematic == true){
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, 2);
		//}
	}
}
