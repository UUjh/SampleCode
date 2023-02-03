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

    public Slider slider;

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
        slider.gameObject.SetActive(false);

    }

    void Update()
    {
        if (!isSowed) icon.color = Color.clear;
        
        // 식물이 자라는 과정
        if (isSowed && !isAdult)
        {
            slider.gameObject.SetActive(true);
            slider.maxValue = growTime;
            slider.value = curTime;
            GrowingHub();
        }
        // 식물이 다 자라면 슬라이더 제거
        if (curTime > growTime)
        {
            slider.gameObject.SetActive(false);
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
                    //Debug.Log("1/4");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Soil_Water");
                }
                else if (curTime < growTime * 0.66)
                {
                    //Debug.Log("2/4");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant01_01");
                }
                else if (curTime < growTime)
                {
                    //Debug.Log("3/4");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant01_02");
                }
                else
                {
                    //Debug.Log("Adult");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant01_Final");
                }
                break;

            case 1:
                if (curTime < growTime / 3)
                {
                    //Debug.Log("1/3");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Soil_Water");
                }
                else if (curTime < growTime * 0.66)
                {
                    //Debug.Log("2/3");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant02_01");
                }
                else if (curTime < growTime)
                {
                    //Debug.Log("3/3");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant02_02");
                }
                else
                {
                    //Debug.Log("Adult");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant02_Final");
                }
                break;

            case 2:
                if (curTime < growTime / 3)
                {
                    //Debug.Log("1/3");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Soil_Water");
                }
                else if (curTime < growTime * 0.66)
                {
                    //Debug.Log("2/4");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant03_01");
                }
                else if (curTime < growTime)
                {
                    //Debug.Log("3/4");
                    icon.sprite = ExtendFunction.ins.HubSpriteReturn("Hub_Plant03_02");
                }
                else
                {
                    //Debug.Log("Adult");
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
    public int myPupNum;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("IsPup: " + Plants.instance.IsPup);

        // 팝업 열려있으면 닫기
        if (Plants.instance.IsPup)
        {
            Plants.instance.ClosePup();
            return;
        }
        // 식물이 안 심어진 상태면 팝업 열기
        if (!isSowed)
        {
            Plants.instance.SowPup(myPupNum);
        }
        // 식물이 다 자라면 식물 수확
        if (isAdult) GetHub();
    }
    #endregion


}
