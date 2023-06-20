using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerScript : MonoBehaviour {
	public float speed = 10f;
	private Rigidbody2D rBody;
	public float jumpSpeed = 12f;
	public float gravity= 20.0f;
	private float vSpeed;
	public bool grounded;
	public float angleValue = 60f;
	public Transform gun;
	private GunScript gScript;
	[HideInInspector]
	public string facing = "Right";
	public Transform canvas;
	public Text ammoText;
	public GameObject gunToPickup;
    void Start () {
		rBody = GetComponent<Rigidbody2D>();
		if(gun){
			gScript = gun.GetComponent<GunScript>();
		}
	}
	void Update(){
		//ScreenWrap();
		if(gun){
			if(facing == "Right"){
				gun.transform.localPosition = new Vector3(transform.localPosition.x + 0.1f,transform.localPosition.y,0);
			}else{
				gun.transform.localPosition = new Vector3(transform.localPosition.x - 0.1f,transform.localPosition.y,0);
			}
		}
		if(canvas){
			canvas.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y + 1.0f,0);
		}
		if(ammoText){
			if(gun && gScript){
				ammoText.text = gScript.ammo.ToString();
			}else{
				ammoText.text = "";
			}
		}
		if (gunToPickup)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				//gun.gameObject = null;
				if (gun)
				{
					gScript.Drop();
				}
				gun = gunToPickup.transform;
				gun.GetComponent<Collider2D>().enabled = false;
				
				if (gun)
				{
					gScript = gun.GetComponent<GunScript>();
					gScript.player = this.gameObject;

				}
			}
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
		//ScreenWrap();
		if (Input.GetButton ("Jump")&& grounded) {
			vSpeed = jumpSpeed;
		}
		if(!grounded){
			vSpeed -= gravity * Time.deltaTime;
		}
		rBody.velocity = new Vector2(Input.GetAxis("Horizontal") * speed,vSpeed);


		if(gun && gScript){
			Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(gun.transform.position);
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			gun.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			if(Input.GetMouseButton(0)&& gScript.timer <= 0){
				gScript.Shoot();
			}
		}


		Vector3 v3 = Input.mousePosition;
		v3 = Camera.main.ScreenToWorldPoint(v3);
		if( v3.x < transform.position.x /*&& facing == "Right"*/){
			//transform.localScale = new Vector2(-localX, transform.localScale.y);
			transform.localScale = new Vector2(-0.2f, transform.localScale.y);
			if(gun){
				if( gScript.gunType == "Rifle" || gScript.gunType == "Grenade"|| gScript.gunType == "Pistol")
				{
					gun.GetComponentInChildren<SpriteRenderer>().sprite = gScript.pPistolLeft;
				}
				
			}
			facing = "Left";
		}else if(v3.x > transform.position.x/* && facing == "Left"*/){
			transform.localScale = new Vector2(0.2f, transform.localScale.y);
			if(gun){
				if( gScript.gunType == "Rifle"|| gScript.gunType == "Grenade"|| gScript.gunType == "Pistol")
				{
					gun.GetComponentInChildren<SpriteRenderer>().sprite = gScript.pPistol;
				}
				
			}
			facing = "Right";
		}
		//print (CheckRenderer());
	}
	void OnCollisionStay2D(Collision2D collision){
		foreach(ContactPoint2D contact in collision.contacts){
			if(contact.normal == Vector2.down){
				vSpeed -= Time.deltaTime *10;
			}
			if(contact.normal == Vector2.up){
				grounded = true;
				vSpeed = 0;
			}
		}
	}
	void OnTriggerStay2D(Collider2D collision){
		if(collision.transform.tag =="Gun"|| collision.transform.tag == "Grenade")
		{
			gunToPickup = collision.transform.gameObject;
		}
	}
	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.transform.tag == "Gun" || collision.transform.tag == "Grenade")
		{
			gunToPickup = null;
		}
	}
	void OnCollisionExit2D(){
		grounded = false;
	}
	
}
