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
        
        // �Ĺ��� �ڶ�� ����
        if (isSowed && !isAdult)
        {
            GrowingHub();
        }
        // �Ĺ��� �� �ڶ��� ��
        if (curTime > growTime)
        {
            AdultHub();
        }
    }

    #region �ڶ�� ����
    public void GrowingHub()
    {
 
        // ���� ������ ���� SpriteAtlas���� �ٸ� ��������Ʈ �ҷ��ͼ� ����
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


    #region �Ĺ� �� �ڶ�� �ʿ��� ���
    public void AdultHub()
    {
        isAdult = true;
    }

    // �Ĺ� ��Ȯ
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

    #region ���� ����

    // �ν����Ϳ��� myPupNum�� �����ؼ� �� ������ ��ȣ�� ����.
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
