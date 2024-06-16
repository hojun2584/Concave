using Assets.Network.Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);

        clickEvent += HitObjectHandler;
        
    }
    public static MouseController instance;

    public event Action<GameObject> clickEvent;

    public MakeSton sendPacket = new MakeSton();

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // 0�� ���� ���콺 ��ư�� �ǹ��մϴ�.
        {
            // ī�޶󿡼� ���콺 ��ġ�� ���̸� �����մϴ�.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���̸� �߻��Ͽ� ������Ʈ���� �浹�� �˻��մϴ�.
            if (Physics.Raycast(ray, out hit))
            {
                clickEvent?.Invoke(hit.transform.gameObject);
            }
        }
    }

    void HitObjectHandler(GameObject hitObject)
    {
        Ground hitGround;
        if(hitObject.TryGetComponent<Ground>(out hitGround))
        {
            if (GameManager.instance.isMyTurn)
            {
            
                ArraySegment<byte> sendBuffer = sendPacket.Write(hitGround.x, hitGround.y);
                NetWorkObj.instance.session.Send(sendBuffer);
            
            }
        }

    }

    


    

}
