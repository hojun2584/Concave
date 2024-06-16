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

    public void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    public bool isMyTurn = true;
    [SerializeField]
    public bool isMyColor = true;

    public bool start = false;


    public void Update()
    {

        if (JobQueue.Instance.jobActions.Count != 0)
            JobQueue.Instance.jobActions.Dequeue()?.Invoke();
            
    }

}
