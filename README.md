using UnityEngine;

public class coin : MonoBehaviour
{
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 1, 0);
    }
}
(Enemy.cs)
C#
کپی
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;
    public int speed = 2;
    void Start() {
        player = GameObject.Find("Player");
    }
    void Update()
    {
        var playerposition = player.transform.position;
        var enemyposition = transform.position;
        transform.position = Vector3.MoveTowards(enemyposition, playerposition, speed * Time.deltaTime);
        transform.LookAt(playerposition);
    }
}
(player.cs)
C#
کپی
using UnityEngine;

public class player : MonoBehaviour
{
    public int speed = 2;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.back * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * speed * Time.deltaTime;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("e"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("c"))
        {
            Destroy(other.gameObject);
        }
    }
}

(Gun.cs)
C#
کپی
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Rigidbody bulletPrefab;
    public float speed = 10f;
    public float power = 2000f;

    void Update()
    {
        var h = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var v = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(h, v, 0); 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody i = Instantiate(bulletPrefab, transform.position, transform.rotation);
            i.AddForce(transform.forward * power);
        }
    }
}

C#
کپی
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // مفصل چرخ جلو
    public WheelJoint2D tf;
    // مفصل چرخ عقب
    public WheelJoint2D tr;

    // گشتاور یا قدرت موتور
    public float t = 500f;

    // متغیر برای نگهداری تنظیمات موتور
    private JointMotor2D m;

    void Update()
    {
        float s = Input.GetAxis("Horizontal");

        if (s != 0)
        {
            m.motorSpeed = -s * 1000f; 
            m.maxMotorTorque = t;      

            tf.useMotor = true;
            tr.useMotor = true;

            tf.motor = m;
            tr.motor = m;
        }
        else
        {
            tf.useMotor = false;
            tr.useMotor = false;
        }
    }
}
ball.cs)
C#
کپی
using UnityEngine;

public class ball : MonoBehaviour
{
    private float movement = 0;
    public float speed = 5f;
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);
    }
}
۲. اسکریپت پرش روی پلتفرم (ground.cs)
C#
کپی
using UnityEngine;

public class ground : MonoBehaviour
{
    public float jump = 11f;
    Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0f) 
        {
            rb = collision.gameObject.GetComponent<Rigidbody2D>(); 
            if (rb != null) 
            {
                rb.velocity = new Vector2(rb.velocity.x, jump); 
            }
        }
    }
}
۳. اسکریپت ایجاد پلتفرم‌ها (NewBehaviourScript.cs)
C#
کپی
using UnityEngine;

public class NewBehaviourScript: MonoBehaviour
{
    public GameObject plane;
    public float maxdis = 3f;
    public float mindis = 2f;
    public int xx = 200;
    public float xdis = 5f;

    void Start()
    {
        f();
    }

    void f()
    {
        Vector2 pos = new Vector2();
        for(int i=0; i<=xx;i++)
        {
            float ypos = Random.Range(mindis, maxdis);
            float xpos = Random.Range(-xdis, xdis);
            pos = new Vector2(xpos, pos.y+ ypos);
            Instantiate(plane, pos, Quaternion.identity);
        }
    }
}
۴. اسکریپت دوربین (کامل) (cameraqaa.cs)
C#
کپی
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraqaa: MonoBehaviour
{
    public Transform target;
    public float follow = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Update()
    {
        if (target.position.y > transform.position.y) 
        {
            Vector3 targerpostion = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targerpostion, follow * Time.deltaTime);
        }
    }
}
