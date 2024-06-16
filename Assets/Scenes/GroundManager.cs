using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{

    public GameObject whiteSton;
    public GameObject blackSton;

    Ground[,] grounds = new Ground[10,10];

    

    public Ground GetGround(int x, int y)
    {
        return grounds[x, y];
    }

    public void AddGround(Ground ground)
    {
        grounds[ground.x, ground.y] = ground;
    }

    public void Awake()
    {
        whiteSton = Resources.Load<GameObject>("WhiteRock");
        blackSton = Resources.Load<GameObject>("BloackRock");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
