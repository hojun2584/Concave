using Assets.Network.Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public int posX, posY;


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


        if (Input.GetMouseButtonDown(0)) // 0은 왼쪽 마우스 버튼을 의미합니다.
        {
            // 카메라에서 마우스 위치로 레이를 생성합니다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 레이를 발사하여 오브젝트와의 충돌을 검사합니다.
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
            if (GameManager.instance.IsMyTurn)
            {
                sendPacket.InitPacket(hitGround.x,hitGround.y, GameManager.instance.isMyColor);
                ArraySegment<byte> sendBuffer = sendPacket.Write();

                Debug.Log(sendPacket.isWhite);
                NetWorkObj.instance.session.Send(sendBuffer);
            }
        }

    }

    


    

}
