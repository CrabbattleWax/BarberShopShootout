using UnityEngine;
using System.Collections;

public class CloudScript : MonoBehaviour {
	public float speed = 5;
	public float xMin = -12;
	public float xMax = 12;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector2.left * speed * Time.deltaTime);
		if(transform.position.x < xMin){
			float currentX = transform.position.x;
			currentX = xMax;
			transform.position = new Vector2(currentX, transform.position.y);
		}
	}
}
