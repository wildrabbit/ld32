using UnityEngine;
using System.Collections;

public class PlayerCharacterControl : MonoBehaviour 
{
    public int baseHp = 100;
    public int baseEloquency = 100;
    public float baseSpeed = 3.0f;
    //------------------
    private int m_hp = 100;
    private int m_eloquency = 100;

    private Rigidbody2D m_bodyRef = null;
    private Collider2D m_colliderRef = null;


    public void Initialize (Vector3 position)
    {
        m_hp = baseHp;
        m_eloquency = baseEloquency;
    }

	// Use this for initialization
	void Start () 
    {
        m_bodyRef = GetComponent<Rigidbody2D>();
        m_colliderRef = GetComponent<Collider2D>();     
	}
	
	// Update is called once per frame
	void Update () 
    {
        float xValue = Input.GetAxis("Horizontal");
        float yValue = Input.GetAxis("Vertical");
        Vector3 movement = Vector3.zero;
        movement.x = xValue;
        movement.y = yValue;
        movement.Normalize();
        movement *= baseSpeed;
        GetComponent<Rigidbody2D>().velocity = movement;
	}

    public void UnloadLevel ()
    {
        GameObject.Destroy(gameObject);
    }
}
