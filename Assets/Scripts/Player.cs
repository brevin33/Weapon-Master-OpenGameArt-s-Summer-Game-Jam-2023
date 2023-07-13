using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.CapsuleBoundsHandle;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.Mathematics;

public class Player : MonoBehaviour
{

    [SerializeField]
    private Camera camera;

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 300f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 1f)]
    float bounciness = 0.5f;

    [SerializeField]
    Rect allowedArea = new Rect(-2.5f, -2.93f, 5f, 5.9f);

    [SerializeField]
    int HP;

    [SerializeField]
    LayerMask mousePosHits;

    Vector3 velocity;

    public Vector3 mousePos;

    [SerializeField]
    GameObject[] weapons;

    bool attacking;

    int attackingWeapon;

    float nextWeapon;

    Vector2 playerInput;

    bool tryAttacking;

    public static int combo = 1;

    public void hitCombo()
    {
        combo += 1;
    }

    public void dropCombo()
    {
        combo = 1;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
    }

    void Update()
    {
        getInput();

        WASDMovment();

        attack();

        faceMouse();
    }

    void attack()
    {
        if (attacking)
        {
            return;
        }
        if (tryAttacking)
        {
            attacking = true;
            StartCoroutine(doAttacks());
        }
    }

    void getInput()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        if (Input.GetButtonDown("Fire1"))
        {
            tryAttacking = true;
        }
        else
        {
            tryAttacking= false;
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, mousePosHits))
        {
            mousePos = new Vector3( raycastHit.point.x, 0.5f, raycastHit.point.z);
        }
    }


    void faceMouse()
    {
        Vector3 facingDir = mousePos - transform.localPosition;
        // -----------------------------------------------------------    Not Implimented --------------------------------
    }

    void WASDMovment()
    {

        Vector3 desiredVelocity =
            new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;
        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }
        if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }
        transform.localPosition = newPosition;
    }

    public void createHitbox(float distFromPlayer, GameObject prefab)
    {
        Vector3 facingDir = mousePos - transform.position;
        facingDir.Normalize();
        Vector3 spawnPos = transform.position + facingDir * distFromPlayer;
        quaternion rotation = transform.rotation;
        rotation *= Quaternion.LookRotation(facingDir);
        rotation *= Quaternion.Euler(90, 270, 0);
        GameObject hitBox = Instantiate(prefab, spawnPos, rotation);
    }

    IEnumerator doAttacks()
    {
        combo = 1;
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject weapon = weapons[i];
            Stats stats = weapon.GetComponent<Stats>();
            createHitbox(stats.distFromPlayer, weapon);
            yield return new WaitForSeconds(stats.cooldown);
        }
        attacking = false;
    }

}
