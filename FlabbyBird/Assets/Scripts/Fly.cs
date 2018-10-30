using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour {

    public float flapTime = .15f;
    public float flapPower = 800;
    public float glidePercentage = .6f;
    public float dragCof = .001f;
    public float restMass = 3;
    public float fatMass = 2;

    private Rigidbody rb;
    private BoxCollider col;
    private Material mat;
    private float endFlapTime = 0;
    private bool isDead = false;
    private float energyPerStep = 0f;
    private float energyExpendedStep = 0f;
    private float expendModifier = .002f;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        mat = GetComponent<Renderer>().material;

        GameManager.instance.mass = restMass + fatMass;
        UpdateMass();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        UpdateSpeedPos();

        if (!isDead)
        {
            transform.up = CalculateAngleAxis();

            if (Input.GetKey("mouse 0"))
            {
                if (Input.GetKeyDown("mouse 0"))
                {
                    StartFlap();
                }
                else if (Time.time < endFlapTime)
                {
                    Flap();
                }
                else
                {
                    Glide();
                }
            }

            ExpendMass();
        }

        CalcDrag();
    }

    private void ExpendMass()
    {
        if (GameManager.instance.mass - restMass < fatMass)
        {
            energyExpendedStep *= .5f;
        }

        GameManager.instance.mass -= (energyPerStep + energyExpendedStep) * expendModifier;
        energyExpendedStep = 0;

        UpdateMass();

        //Debug.Log("mass: " + GameManager.instance.mass);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (GameManager.instance.speed > 10)
            {
                isDead = true;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Bug")
        {
            ConsumeBug(collision.gameObject);
        }
    }

    private void ConsumeBug(GameObject bug)
    {
        if (!isDead)
        {
            // bug.eaten();
            // bird.mass += bug.mass();
            Destroy(bug);
            GameManager.instance.mass += .3f;

            UpdateMass();
        }
    }

    private void UpdateMass()
    {
        rb.mass = GameManager.instance.mass;

        if (GameManager.instance.mass < restMass)
        {
            isDead = true;
        }

        float t = GameManager.instance.mass - restMass;

        if (t <= 2)
        {
            mat.color = Color.Lerp(Color.red, Color.green, t/fatMass);
        }
        else
        {
            mat.color = Color.Lerp(Color.green, Color.blue, (t-fatMass)*.2f);
        }

    }

    private void UpdateSpeedPos()
    {
        GameManager.instance.speed = rb.velocity.x;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        //Debug.Log("VELOCITY: " + rb.velocity.x);
    }

    private void UpdateVelocity()
    {
        Vector3 v = rb.velocity;
        v.x = GameManager.instance.speed;
        rb.velocity = v;
    }

    // doing our own drag model helps control what speed the player can fly at (not linear scaling - fast == no)
    private void CalcDrag()
    {
        if (GameManager.instance.speed != 0)
        {
            float fatDrag = Mathf.Clamp(((GameManager.instance.mass - (restMass + fatMass)) / 2000), 0, 1);
            float speedMag = Mathf.Pow(Mathf.Abs(GameManager.instance.speed), 2f);
            speedMag *= Mathf.Abs(GameManager.instance.speed) / GameManager.instance.speed;
            GameManager.instance.speed -= speedMag * (dragCof + fatDrag) * Time.deltaTime *.3f;

            UpdateVelocity();
        }
    }

    private void StartFlap()
    {
        endFlapTime = Time.time + flapTime;
        Flap();
    }

    private void Flap()
    {
        float downVelocity = Mathf.Clamp(rb.velocity.y, -25, 25) * -1;
        Vector3 angle = CalculateAngleAxis();

        //Debug.Log("Speed: " + downVelocity);
        //Debug.Log("FlapPower: " + (downVelocity * 15 + flapPower));

        rb.AddForce(angle * ((flapPower + downVelocity * 12) / flapTime) * Time.deltaTime * 5);
        // TODO animate flap

        ChargeEnergy(5f);
    }

    private void Glide()
    {
        // Calculates counteractive force of gravity - glidePercentage of 1 should be zero grav
        float scalar = (float) (Time.deltaTime * 9.81f * 59.85 * rb.mass * glidePercentage);
        //Debug.Log("glidePower: " + scalar);

        rb.AddForce(Vector3.up * scalar);
        ChargeEnergy(1f);
    }
    
    private void ChargeEnergy(float energy)
    {
        energyExpendedStep += energy;
    }

    private Vector3 CalculateAngleAxis()
    {
        float t = (Input.mousePosition.x / Screen.width);
        t -= .5f;
        t *= 1.75f;
        t += .5f;
        float minAngle = -35;
        float maxAngle = 45;
        float angle = Mathf.Lerp(minAngle, maxAngle, t);

        //Debug.Log("pos: " + xPos);
        //Debug.Log("width: " + Screen.width);
        //Debug.Log("angle: " + angle);

        return Quaternion.AngleAxis(angle, Vector3.back) * Vector3.up;
    }
}
