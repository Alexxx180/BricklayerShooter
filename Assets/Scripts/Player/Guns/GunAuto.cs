using System;
using UnityEngine;
using UnityEngine.UI;

public class GunAuto : MonoBehaviour, IGunBase
{
    public AudioSource source;
    public AudioSource reload;
    public Camera fpsCam;
    // Start is called before the first frame update

    public ushort damage = 10;
    public float range = 100f;
    public float fireRate = 15f;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public float nextFire = 0.5f;
    public string control = "Fire1";

    [SerializeField]
    private readonly ushort _INFINITE = 65535;
    public ushort INFINITE => _INFINITE;
    public ushort currentAmmo = 30;
    [SerializeField]
    private ushort _leftAmmo = 150;
    public ushort leftAmmo
    {
        get { return _leftAmmo; }
        set { _leftAmmo = value; }
    }

    [SerializeField]
    private ushort _maxAmmo = 150;
    public ushort maxAmmo
    {
        get { return _maxAmmo; }
        set { _maxAmmo = value; }
    }
    public ushort division = 30;
    public Text ammo;

    // Update is called once per frame 
    void Update()
    {
        if (Input.GetButtonDown("Reload") && maxAmmo < INFINITE)
            AmmoCheck2();
        if (Input.GetButton(control) && Time.time >= nextFire)
        {
            nextFire = Time.time + 1f / fireRate;
            if (currentAmmo > 0)
                Shoot();
            AmmoCheck();
        }
    }
    public ushort MinAmmo(int min, int left)
    {
        return Convert.ToUInt16(Math.Min(min, left));
    }
    public ushort MaxAmmo(int min, int left)
    {
        return Convert.ToUInt16(Math.Max(min, left));
    }
    private void AmmoCheck()
    {
        if (maxAmmo >= INFINITE)
            return;
        currentAmmo = MaxAmmo(0, currentAmmo - 1);
        ammo.text = currentAmmo + " / " + leftAmmo;
        if (currentAmmo <= 0)
            AmmoCheck2();
    }
    private void AmmoCheck2()
    {
        reload.Play();
        if (leftAmmo <= 0)
        {
            ammo.text = currentAmmo + " / " + leftAmmo;
            return;
        }
        ushort mem = currentAmmo;
        currentAmmo = MinAmmo(leftAmmo, division);
        leftAmmo = MaxAmmo(0, leftAmmo - division + mem);
        RefreshAmmo();
    }
    public void Restore(ushort bonus)
    {
        leftAmmo = MinAmmo(maxAmmo, leftAmmo + bonus);
        RefreshAmmo();
    }
    public void RefreshAmmo()
    {
        ammo.text = currentAmmo + " / " + leftAmmo;
    }
    private void Shoot()
    {
        source.Play();
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Enemy target = hit.transform.GetComponent<Enemy>();
            if (target != null)
                target.TakeDamage(damage);
            StaticEnemy target2 = hit.transform.parent.transform.GetComponent<StaticEnemy>();
            if (target2 != null)
                target2.TakeDamage(damage);
        }

        if (impactEffect == null)
            impactEffect = GameObject.Find("MuzzleFlash");
        GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactGO, 2f);
    }
}
