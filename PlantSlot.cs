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
        
        // �Ĺ��� �ڶ�� ����
        if (isSowed && !isAdult)
        {
            slider.gameObject.SetActive(true);
            slider.maxValue = growTime;
            slider.value = curTime;
            GrowingHub();
        }
        // �Ĺ��� �� �ڶ�� �����̴� ����
        if (curTime > growTime)
        {
            slider.gameObject.SetActive(false);
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


    #region �Ĺ� �� �ڶ�� �ʿ��� ���
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



    #region ���� ����
    public int myPupNum;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("IsPup: " + Plants.instance.IsPup);

        // �˾� ���������� �ݱ�
        if (Plants.instance.IsPup)
        {
            Plants.instance.ClosePup();
            return;
        }
        // �Ĺ��� �� �ɾ��� ���¸� �˾� ����
        if (!isSowed)
        {
            Plants.instance.SowPup(myPupNum);
        }
        // �Ĺ��� �� �ڶ�� �Ĺ� ��Ȯ
        if (isAdult) GetHub();
    }
    #endregion


}
