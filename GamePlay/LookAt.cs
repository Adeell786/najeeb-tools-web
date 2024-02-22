using UnityEngine;

public class LookAt : MonoBehaviour
{
    public GameObject Lastbullet;
    private void Awake()
    {
        if (HudMenuManager.instance.UI[1].activeSelf)
        {

            HudMenuManager.instance.zoomOut();
        }
        
        FPSPlayer.instance.zoomed = false;
        transform.LookAt(MConstants.LastBulletTarget.transform);
    }
    private void Start()
    {
        Lastbullet.SetActive(true);
        Destroy(this.gameObject, 1f);
    }
    // [SerializeField]
   // public Transform lookObject;

    //public void SetLookObject(Transform obj)
    //{
    //    lookObject = obj;
    //}

    void Update()
    {
        //if(lookObject)
        //{
      //  transform.LookAt(MConstants.LastBulletTarget.transform);
        //}
    }
}
