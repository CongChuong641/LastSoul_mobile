using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
public class Gun : MonoBehaviour
{
    private float rotateOffSet = 180f;
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;
    [SerializeField] private int maxAmmo=50;
    public int currentAmmo;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    // Update is called once per frame
    void Update()
    {
        RotateGun();
        Shoot();
        ReLoad();
    }
    void RotateGun()
    {
        Vector2 aimScreenPosition = GameInput.GetAimScreenPosition();

        if (aimScreenPosition.x < 0 || aimScreenPosition.x > Screen.width || aimScreenPosition.y < 0 || aimScreenPosition.y > Screen.height)
        {
            return; 
        }
        Vector3 displacement = transform.position - Camera.main.ScreenToWorldPoint(aimScreenPosition);
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffSet);
        if (angle < -90 || angle > 90)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }else
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
    }
    void Shoot()
    {
        if (GameInput.ConsumeFirePressed() && currentAmmo > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;
            Instantiate(bulletPrefabs, firePos.position, firePos.rotation);
            currentAmmo--;
            UpdateAmmoText();
            audioManager.PlayShootSound();
        }
        
    }
    void ReLoad()
    {
        if (GameInput.ConsumeReloadPressed() && currentAmmo < maxAmmo)
        { 
            currentAmmo = maxAmmo;
            UpdateAmmoText();
            audioManager.PlayReLoadSound();
        }
    }
    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            if (currentAmmo > 0)
            {
                ammoText.text = currentAmmo.ToString();
            }
            else
            {
                ammoText.text = "Empty";
            }
        }
    }
}
