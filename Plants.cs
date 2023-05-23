using BansheeGz.BGDatabase;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Plants : MonoBehaviour, IPointerClickHandler
{
    public static Plants instance;

    public GameObject[] plantSlot;
    public BoxCollider2D[] box;
    public PlantSlot[] slots;

    public int seedNum;
    public GameObject seedPup;

    private bool isPup;


    public GameObject lockPanel;

    public TMP_Text hub0Txt;
    public TMP_Text hub1Txt;
    public TMP_Text hub2Txt;

    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
        // 데이터 불러오기
        DataManager.instance.LoadFile();

        GetComponent<BoxCollider2D>();

        isPup = false;
        seedPup.SetActive(false);
        
        if (DataManager.instance.boilUnlock) Destroy(lockPanel);

    }


    void Update()
    {
        // 팝업이 실행 시
        if (isPup)
        {
            var repo = BGRepo.I;
            var meta = repo["PlayData"];
            var entity = meta[0];
            var seedKind = entity.Get<List<BGEntity>>("Seed");

            // Database에서 씨앗의 개수를 불러와서 팝업에 표시.
            hub0Txt.text = seedKind[0].Get<int>("count").ToString();
            hub1Txt.text = seedKind[1].Get<int>("count").ToString();
            hub2Txt.text = seedKind[2].Get<int>("count").ToString();

        }
        if (!isSaved)
        {
            isSaved = true;
            StartCoroutine(PlantsAutoSave());
        }
    }

    bool isSaved;
    WaitForSeconds secs = new WaitForSeconds(10f);
    // 10초에 한 번씩 자동으로 식물이 자라는 것을 저장
    IEnumerator PlantsAutoSave()
    {
        yield return secs;
        isSaved = false;
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];

        var meta1 = entity.Get<List<BGEntity>>("PlantSlot");
        for (int i = 0; i < plantSlot.Length; i++)
        {
            // 각 식물 슬롯에 심어진 식물의 정보 저장.
            if (slots[i].isSowed)
            {
                meta1[i].Set("isSowed", true);
                meta1[i].Set("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);

            }
            else
            {
                meta1[i].Set("isSowed", false);

                meta1[i].Set<int>("seedNum", slots[i].seedNum);

                meta1[i].Set("time", slots[i].curTime);

            }
        }
    }

    public bool IsPup
    {
        get { return isPup; }
        set { isPup = value; }
    }

    public void ClosePup()
    {
        isPup = false;
        seedPup.SetActive(false);
    }

    public int slotNum;

    // 씨앗 팝업
    public void SowPup(int pupNum)
    {
        slotNum = pupNum;
        seedPup.gameObject.SetActive(true);
        isPup = true;
    }

    public void SowSeed(int seed)
    {
        slots[slotNum].seedNum = seed;

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var seedKind = entity.Get<List<BGEntity>>("Seed");
        var meta1 = entity.Get<List<BGEntity>>("PlantSlot");
        
        // Database에서 씨앗 정보를 불러와서 개수가 0개라면 return
        if (seedKind[slots[slotNum].seedNum].Get<int>("count") < 1)
        {
            return;
        }
        else
        {
            // 씨앗의 개수가 1개 이상이라면 씨앗을 심음.
            seedKind[slots[slotNum].seedNum].Set<int>("count", seedKind[slots[slotNum].seedNum].Get<int>("count") - 1);
            SoundManager.instance.EffectPlay(SoundManager.instance.sowEffect);
        }

        // 씨앗의 번호에 따라 자라는 시간을 설정
        switch (seedNum)
        {
            case 0:
                slots[slotNum].growTime = 20;
                break;
            case 1:
                slots[slotNum].growTime = 30;
                break;
            case 2:
                slots[slotNum].growTime = 40;
                break;
        }


        slots[slotNum].isSowed = true;
        meta1[slotNum].Set("isSowed", true);
        slots[slotNum].curTime = 0;
        meta1[slotNum].Set("time", slots[slotNum].curTime);
        slots[slotNum].icon.color = Color.white;


        seedPup.gameObject.SetActive(false);
        slots[slotNum].slider.gameObject.SetActive(true);
        isPup = false;

        PlantsSave();
        DataManager.instance.SaveFile();

    }

    public void PlantsSave()
    {
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];

        var meta1 = entity.Get<List<BGEntity>>("PlantSlot");
        for (int i = 0; i < plantSlot.Length; i++)
        {
            if (slots[i].isSowed)
            {
                meta1[i].Set("isSowed", true);
                meta1[i].Set("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);

            }
            else
            {
                meta1[i].Set("isSowed", false);
                meta1[i].Set<int>("seedNum", slots[i].seedNum);
                meta1[i].Set("time", slots[i].curTime);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 팝업이 열려있다면, 팝업이 아닌 다른 곳을 클릭했을 때 팝업을 닫음.
        if (isPup) { ClosePup(); }
    }
}
