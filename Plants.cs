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
        // ������ �ҷ�����
        DataManager.instance.LoadFile();

        GetComponent<BoxCollider2D>();

        isPup = false;
        seedPup.SetActive(false);
        
        if (DataManager.instance.boilUnlock) Destroy(lockPanel);

    }


    void Update()
    {
        // �˾��� ���� ��
        if (isPup)
        {
            var repo = BGRepo.I;
            var meta = repo["PlayData"];
            var entity = meta[0];
            var seedKind = entity.Get<List<BGEntity>>("Seed");

            // Database���� ������ ������ �ҷ��ͼ� �˾��� ǥ��.
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
    // 10�ʿ� �� ���� �ڵ����� �Ĺ��� �ڶ�� ���� ����
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
            // �� �Ĺ� ���Կ� �ɾ��� �Ĺ��� ���� ����.
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

    // ���� �˾�
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
        
        // Database���� ���� ������ �ҷ��ͼ� ������ 0����� return
        if (seedKind[slots[slotNum].seedNum].Get<int>("count") < 1)
        {
            return;
        }
        else
        {
            // ������ ������ 1�� �̻��̶�� ������ ����.
            seedKind[slots[slotNum].seedNum].Set<int>("count", seedKind[slots[slotNum].seedNum].Get<int>("count") - 1);
            SoundManager.instance.EffectPlay(SoundManager.instance.sowEffect);
        }

        // ������ ��ȣ�� ���� �ڶ�� �ð��� ����
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
        // �˾��� �����ִٸ�, �˾��� �ƴ� �ٸ� ���� Ŭ������ �� �˾��� ����.
        if (isPup) { ClosePup(); }
    }
}
