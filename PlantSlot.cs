using BansheeGz.BGDatabase;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlantSlot : MonoBehaviour, IPointerClickHandler
{
    public static PlantSlot instance;

    public bool isSowed;
    public bool isAdult;
    public int seedNum = -1;

    public float curTime;
    public float growTime;
    public Image icon;


    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        icon.color = Color.white;
    }

    void Update()
    {
        if (!isSowed) icon.color = Color.clear;
        
        // 식물이 자라는 과정
        if (isSowed && !isAdult)
        {
            GrowingHub();
        }
        // 식물이 다 자랐을 때
        if (curTime > growTime)
        {
            AdultHub();
        }
    }

    #region 자라는 과정
    public void GrowingHub()
    {
 
        // 씨앗 종류에 따라 SpriteAtlas에서 다른 스프라이트 불러와서 적용
        switch (seedNum)
        {
            case 0:
                if (curTime < growTime / 3)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Soil_Water");
                }
                else if (curTime < growTime * 0.66)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant01_01");
                }
                else if (curTime < growTime)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant01_02");
                }
                else
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant01_Final");
                }
                break;

            case 1:
                if (curTime < growTime / 3)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Soil_Water");
                }
                else if (curTime < growTime * 0.66)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant02_01");
                }
                else if (curTime < growTime)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant02_02");
                }
                else
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant02_Final");
                }
                break;

            case 2:
                if (curTime < growTime / 3)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Soil_Water");
                }
                else if (curTime < growTime * 0.66)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant03_01");
                }
                else if (curTime < growTime)
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant03_02");
                }
                else
                {
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant03_Final");
                }
                break;

        }
    }

    #endregion


    #region 식물 다 자라면 필요한 기능
    public void AdultHub()
    {
        isAdult = true;
    }

    // 식물 수확
    public void GetHub()
    {
        Debug.Log("GetHUb()");
        isAdult = false;
        isSowed = false;

        DataManager.instance.GetHub(seedNum);
        seedNum = -1;
        curTime = 0;

        Plants.instance.PlantsSave();
        DataManager.instance.SaveFile();
    }


    #endregion

    #region 슬롯 관리

    // 인스펙터에서 myPupNum을 설정해서 각 슬롯의 번호를 설정.
    public int myPupNum;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Plants.instance.IsPup)
        {
            Plants.instance.ClosePup();
            return;
        }
        if (!isSowed)
        {
            Plants.instance.SowPup(myPupNum);
        }
        if (isAdult) GetHub();
    }
    #endregion


}
