using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    bool isSton = false;
    [SerializeField]
    bool isWhite = false;
    
    GroundManager manager;

    public ushort x, y;



    public bool IsWhite 
    {
        set
        {
            if (isWhite == value)
                Instantiate(manager.whiteSton, new Vector3(transform.position.x , transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
            else
                Instantiate(manager.blackSton, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
            
            isSton = true; 
        }
    }


    private void Awake()
    {
        manager = GetComponentInParent<GroundManager>();
        manager.AddGround(this);


    }

}
