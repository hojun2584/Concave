using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ground : MonoBehaviour
{
    [SerializeField]
    bool isSton = false;
    bool isWhite = false;
    
    GroundManager manager;

    public bool check = false;
    public ushort x, y;


    public bool IsWhite 
    {
        set
        {
            if (isWhite == value)
                Instantiate(manager.whiteSton, new Vector3(transform.position.x , transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
            else
                Instantiate(manager.blackSton, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);

            isWhite = value;
            isSton = true; 
        }
        get { return isWhite; }
    }
    public bool IsSton => isSton;


    private void Awake()
    {
        manager = GetComponentInParent<GroundManager>();
        manager.AddGround(this);
    }

}
