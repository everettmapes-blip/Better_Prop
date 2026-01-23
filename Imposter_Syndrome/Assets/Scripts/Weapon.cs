using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifetime = 3f;
    public Camera playerCamera;
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f; // NOTE: This controls time between shots AND time between burst shots
    public int bulletsPerBurst = 3;
    public int burstBulletsleft;
    public float spreadIntensity; 

    public GameObject muzzleEffect;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;
    
    private void Awake()
    {
        readyToShoot = true;
        burstBulletsleft = bulletsPerBurst;
    }

    // Update is called once per frame
    void Update()
    {
        // FIX 1: Removed the stray "if(Input.GetKeyDown...)" that was here.
        // It was preventing "isShooting" from updating back to false.

        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if ( readyToShoot && isShooting)
        {
            burstBulletsleft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {

        if (muzzleEffect != null)
        {
            ParticleSystem ps = muzzleEffect.GetComponent<ParticleSystem>();
            
            // 1. Stop the parent AND all children (true), and clear any old particles
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            // 2. Play the parent AND all children (true)
            ps.Play(true);
        }
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        
        // FIX 2: Removed the first AddForce. You were adding force twice, 
        // which makes the gun shoot unnaturally fast and ignore spread.
        bullet.transform.forward = shootingDirection;
        bullet.transform.Rotate(0, 90, 0);
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));
        
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        
        if ( currentShootingMode == ShootingMode.Burst && burstBulletsleft > 1 )
        {
            burstBulletsleft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
                targetPoint = hit.point;
        }
        else 
        {
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x,y,0);
    }
    
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        // FIX 3: Added the actual destroy command
        Destroy(bullet);
    }
}