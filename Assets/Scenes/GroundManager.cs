using Assets.Network.Packet;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public static GroundManager instance;


    public Ground currentGround;

    public const int maxWidth = 10;
    public const int maxHeight = 10;


    public int max_Width = 10;

    public GameObject whiteSton;
    public GameObject blackSton;

    public Ground[,] grounds = new Ground[maxWidth,maxHeight];
    public Packet myPacket;


    public Ground GetGround(int x, int y)
    {
        return grounds[x, y];
    }

    public void AddGround(Ground ground)
    {
        ground.check = true;
        grounds[ground.x, ground.y] = ground;
    }

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        whiteSton = Resources.Load<GameObject>("WhiteRock");
        blackSton = Resources.Load<GameObject>("BloackRock");
        MouseController.instance.clickEvent += SetCurrentGround;

    }

    private void SetCurrentGround(GameObject groundObjet)
    {
        if (groundObjet.TryGetComponent<Ground>(out Ground currentGround))
             this.currentGround = currentGround;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            myPacket = new RPCPacket();

            NetWorkObj.instance.session.Send(myPacket.Write()); ;
            Debug.Log("write Packet");
        }
    }
}
