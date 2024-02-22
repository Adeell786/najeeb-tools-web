using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movebullet : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    bool run = true;
    public Vector3 Direction;
    public float Speed = 1;
    public float startTimeValue = 0.005f;
    public float endTimeValue = 0.5f;
    public float timeLerpSpeed;
    public GameObject RotateBulletMesh,BulletFlare,Cam1,Cam2;
    public GameObject ThirdPersonMesh;
    public int TPSweapon;
    ThirdPersonWeapons thirdweapons;
    int weapon;
    private void Awake()
    {
        HudMenuManager.instance.Croshair.SetActive(false);
        
            FPSPlayer.instance.CameraControlComponent.thirdPersonActive = true;
         thirdweapons = FindObjectOfType<ThirdPersonWeapons>();
        FPSPlayer.instance.GetComponent<Rigidbody>().isKinematic = true;
        //weapon  = PlayerWeapons.instance.currentWeapon.GetHashCode();
        run = true;
      //  ThirdPersonMesh = GameObject.Find("Player Character Model(Clone)");
        this.transform.SetParent(null);
        transform.LookAt(MConstants.LastBulletTarget.transform);
        string name = PlayerWeapons.instance.weaponOrder[PlayerWeapons.instance.currentWeapon].gameObject.name;
        
        for (int i = 0; i <= thirdweapons.thirdPersonWeaponModels.Count - 1; i++)
        {
            
            if (name == thirdweapons.thirdPersonWeaponModels[i].weaponObject.gameObject.name)
            {
                thirdweapons.thirdPersonWeaponModels[i].weaponObject.gameObject.SetActive(true);
                TPSweapon = i;
            }
        }


    }
    void Start()
    {
      //  ThirdPersonMesh.SetActive(false);
        Speed = 30;
      //  Direction = (MConstants.LastBulletTarget.transform.transform.position - transform.position).normalized;
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 0.05f;
     // transform.LookAt(MConstants.LastBulletTarget.transform);
    }
    private void OnEnable()
    {
        //this.transform.rotation = Quaternion.Euler(0f, this.gameObject.transform.rotation.y, 0f);
        //HudMenuManager.Instance.HideGamePlay();ad
        //Time.timeScale = startTimeValue;
    }
        // Update is called once per frame
        private void Update()
    {
        if (run)
        {
            transform.LookAt(MConstants.LastBulletTarget.transform);
            //  transform.position += Direction * Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, MConstants.LastBulletTarget.transform.position, Time.deltaTime * Speed);
            // rb.AddForce(this.transform.forward * 1000);
            float dist = Vector3.Distance(MConstants.LastBulletTarget.transform.position, transform.position);
            float dis2 = Vector3.Distance(FPSPlayer.instance.transform.position, MConstants.LastBulletTarget.transform.position);
            float valu2 = 70 / 100 * dist;

            if (dis2 >= 4)
            {
                if (dist <= 5f && !Cam2.activeSelf)
                {
                    Time.timeScale = 0.07f;
                    // Cam1.transform.position = Cam2.transform.position;
                    Cam1.SetActive(false);
                    Cam2.transform.SetParent(null);
                    Cam2.SetActive(true);

                }
                if (dist <= 2)
                {
                    Time.timeScale = 1;
                }
            }
            if(dis2 < 4)
            {
                Time.timeScale = 0.07f;
                Cam1.SetActive(false);
                Cam2.transform.SetParent(null);
                Cam2.SetActive(true);
            }
            if (dist <= 0.1f)
            {
                Time.timeScale = 1;
                if (MConstants.LastBulletTarget.gameObject.transform.parent.GetComponent<FOVDetection>())
                {
                   
                    MConstants.LastBulletTarget.gameObject.transform.parent.GetComponent<FOVDetection>().EnemyDie();
                }
                else if (MConstants.LastBulletTarget.gameObject.GetComponent<SocideBoomber>())
                {
                    MConstants.LastBulletTarget.gameObject.GetComponent<SocideBoomber>().DamageApplie(100);
                }
                else
                {
                    if (MConstants.LastBulletTarget.transform.gameObject.GetComponent<LocationDamage>().damageMultiplier > 15f)
                    {
                        HudMenuManager.instance.ShowHeadShort(MConstants.LastBulletTarget.gameObject.transform);

                    }
                    MConstants.LastBulletTarget.gameObject.GetComponent<LocationDamage>().ApplyDamage(500, Vector3.forward, WeaponBehavior.instance.mainCamTransform.position, WeaponBehavior.instance.myTransform, true, false);
                    WeaponBehavior.instance.FPSPlayerComponent.UpdateHitTime();//used for hitmarker}
                }
                run = false;
                rb.isKinematic = true;
                RotateBulletMesh.SetActive(false);
                BulletFlare.SetActive(false);
                this.GetComponent<MeshRenderer>().enabled = false;
                
                Invoke("DestroyThis", 2f);
                
            }
            
        }
    }

    private void DestroyThis()
    {
        FPSPlayer.instance.CameraControlComponent.thirdPersonActive = false;
        FPSPlayer.instance.GetComponent<Rigidbody>().isKinematic = false;
        for (int i = 0; i <= thirdweapons.thirdPersonWeaponModels.Count - 1; i++)
        {
            thirdweapons.thirdPersonWeaponModels[i].weaponObject.gameObject.SetActive(false);
        }
        HudMenuManager.instance.Croshair.SetActive(true);
        //FPSPlayer.instance.CameraControlComponent.thirdPersonActive = false;
        FPSPlayer.instance.painFadeObj.GetComponent<Image>().enabled = true;
        HudMenuManager.instance.FpsCamera.farClipPlane = 12000f;
      //  ThirdPersonMesh.SetActive(true);
        MConstants.IslastBullet = false;
        Destroy(Cam2.gameObject);
        Destroy(this.gameObject);
    }
  
}
