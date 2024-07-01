using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic
{

    bool isWhite;
    GroundManager instance;

    public Logic()
    {
        instance = GroundManager.instance;
    }
    ushort posX, posY;

    int count = 0;
    public const int MaxCount = 4;

    private bool isGameEnd = false;

    public bool IsGameEnd
    {
        get
        {
            return isGameEnd;
        }
    }

    public void ConcadeLogic()
    {
        isWhite = instance.currentGround.IsWhite;

        posX = instance.currentGround.x;
        posY = instance.currentGround.y;
        
        isGameEnd = WidthCheck();

        

        JobQueue.Instance.Add(() => { GameManager.instance.textMesh.text = $"{count} , {isGameEnd}"; });
    }


    private bool WidthCheck()
    {
        bool leftFlag = true;
        bool rightFlag = true;

        count = 1;

        // int i= 0 이 아닌 1 인 이유, 0은 시작지점 자기 자신일 터 이니 자기 자신과 색이 다를리 없으므로
        // +1 된 상황에서 검사하면 나를 제외한 다른 지점 부터 찾기 때문.

        Ground selectGround;
        for (int i = 1; ; i++)
        {
            if ( i > MaxCount )
                return false;

            if( (posX - i) >= 0 )
            {
                selectGround = instance.grounds[posX-i , posY];
                

                if (selectGround.IsSton &&leftFlag && selectGround.IsWhite == this.isWhite)
                    count++;
                else
                    leftFlag = false;
            }
            else
            {
                leftFlag = false;
            }

            //if( (posX + i) < GroundManager.maxWidth && rightFlag && instance.GetGround((posX + i), posY).IsWhite == this.isWhite)
            //    count++;
            //else
            //    rightFlag = false;


            //---------------------------------------
            if (count >= MaxCount)
                return true;

            if (leftFlag && rightFlag == false)
                return false;
        }

    }



}
