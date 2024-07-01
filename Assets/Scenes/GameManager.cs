using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GroundManager groundManager;
    public TextMeshProUGUI textMesh;
    public TextMeshProUGUI textcolor;
    public TextMeshProUGUI myTurn;
    public int sessionId;


    public event Action whiteWin;
    public event Action blackWin;

    PlayerStruct playerData;

    public PlayerStruct PlayerData 
    {
        get { return playerData; }
        set
        {
            playerData = value;

            // 검정색 우선 시작
            IsMyTurn = !playerData.isWhite;
            isMyColor = playerData.isWhite;
            sessionId = playerData.sessionId;

            textcolor.text = isMyColor + " my color";
        }
    }

    public Logic concadeLogic;




    public void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        concadeLogic = new Logic();
        MouseController.instance.clickEvent += ( GameObject template ) => { concadeLogic.ConcadeLogic(); };
    }

    public bool IsMyTurn
    {
        get
        {
            return isMyTurn;
        }
        set
        {
            isMyTurn = value;
            myTurn.text = "current Turn is mine " + value;
        }
    }

    [SerializeField]
    bool isMyTurn = false;

    [SerializeField]
    public bool isMyColor = true;

    public bool start = false;


    public void Update()
    {
        if (JobQueue.Instance.jobActions.Count != 0)
            JobQueue.Instance.jobActions.Dequeue()?.Invoke();
            
    }

}
