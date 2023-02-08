using BansheeGz.BGDatabase;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pot : MonoBehaviour, IPointerClickHandler
{
    public static Pot instance;

    public InventorySlot slot0;
    public InventorySlot slot1;
    public InventorySlot slot2;

    public Vector2 basePos;
    public Image img;
    public Image img1;
    public Image img2;

    public Image potImg;

    public Transform parent;

    public DB_Potion curPotion;
    public DB_Hub curHub;
    public DB_PlayData playData;

    public string itemKey;
    public string itemKey1;

    public int addPotionNum;

    public bool isSlot0Full;
    public bool isFull;
    public bool isPotion;
    public bool isHub;

    public bool isBoiling;
    public bool isBoiled;


    public float boilTime;
    public float maxTime;

    public string potionName;

    public Slider slider;

    public BoxCollider2D box;

    string potName;

    Vector3 slot2BasePos;

    public GameObject lockPanel;

    public GameObject mixToast;

    private void Awake()
    {
        instance = this;
        playData = DB_PlayData.FindEntity(null);
        DataManager.instance.LoadFile();

    }

    void Start()
    {
        //img = GetComponentInChildren<Image>();
        //img1 = GetComponentInChildren<Image>();
        //img2 = GetComponentInChildren<Image>();
        potImg = GetComponent<Image>();
        basePos = transform.position;
        parent = transform.parent;
        slot0.transform.parent.gameObject.SetActive(false);
        slot1.transform.parent.gameObject.SetActive(false);
        slot2.transform.parent.gameObject.SetActive(false);
        //GetComponent<Pot>();
        //box = GetComponent<BoxCollider2D>();
        slider.gameObject.SetActive(false);
        potName = PlayerPrefs.GetString("potName");
        slot2BasePos = slot2.transform.parent.position;

        if (DataManager.instance.boilUnlock) Destroy(lockPanel);

    }

    // Update is called once per frame
    void Update()
    {
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];

        var meta1 = entity.Get<List<BGEntity>>("Boil");
        
        potName = PlayerPrefs.GetString("potName");
        potImg.sprite = ExtendFunction.ins.PotSpriteReturn(PlayerPrefs.GetString("potName"));


        if (isBoiling && boilTime < maxTime)
        {
            meta1[0].Set("isBoiling", true);
            meta1[0].Set("Boiltime", boilTime);
            slider.gameObject.SetActive(true);
            slider.value = boilTime;
            PlayerPrefs.SetString("potName", "boiling");
            //DataManager.instance.SaveFile();
        }
        else if (isBoiling && boilTime >= maxTime)
        {
            isBoiled = true;
            PlayerPrefs.SetString("potName", "boiled");
            meta1[0].Set("isBoiled", true);
            DataManager.instance.SaveFile();
            slider.gameObject.SetActive(false);
        }
        else PlayerPrefs.SetString("potName", "empty");
    }

    //bool isPotTimeSet;
    //IEnumerator PotDelaySet()
    //{

    //}


    public void OnPointerClick(PointerEventData eventData)
    {
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var meta1 = entity.Get<List<BGEntity>>("Boil");

        if (isFull && !isBoiling)
        {
            BoilPotion(itemKey, itemKey1);

            SoundManager.instance.effectAudioSource.clip = SoundManager.instance.boilEffect;
            SoundManager.instance.effectAudioSource.Play();
        }
        if (addPotionNum != -1 && isBoiled)
        {
            StartCoroutine(AddBoiledPotion());
            DataManager.instance.AddPotion(addPotionNum);
            meta1[0].Set("potionNum", addPotionNum);
            PlayerPrefs.SetString("potName", "empty");
        }
    }

    public void ChangItem()
    {
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");
        var hubKind = entity.Get<List<BGEntity>>("Hub");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                itemKey = potionKind[i].Get<string>("key");
                isSlot0Full = true;
                isPotion = true;
            }
        }
        for(int i = 0; i < hubKind.Count; i++)
        {
            if (hubKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curHub = DB_Hub.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                slot0.InitItem(hubKind[i].Get<string>("key"));
                itemKey = hubKind[i].Get<string>("key");
                isSlot0Full = true;
                isHub = true;
            }
        }
        slot0.InitItem(itemKey);

    }

    public void ChangItem1()
    {
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");
        var hubKind = entity.Get<List<BGEntity>>("Hub");

        if (isHub)
        {
            for (int i = 0; i < potionKind.Count; i++)
            {
                if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
                {
                    curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                    slot1.transform.parent.gameObject.SetActive(true);
                    itemKey1 = potionKind[i].Get<string>("key");
                    isFull = true;
                }
            }
        }
        if (isPotion)
        {
            for (int i = 0; i < hubKind.Count; i++)
            {
                if (hubKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
                {
                    curHub = DB_Hub.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                    slot1.transform.parent.gameObject.SetActive(true);
                    itemKey1 = hubKind[i].Get<string>("key");
                    isFull = true;
                }
            }
        }
        slot1.InitItem(itemKey1);

    }

    public void BoilPotion(string itemKey, string itemKey1)
    {
        slot0.transform.parent.gameObject.SetActive(false);
        slot1.transform.parent.gameObject.SetActive(false);
        switch (itemKey)
        {
            case "Red_Vac":
                switch (itemKey1)
                {
                    case "Hub0":
                        addPotionNum = 6;
                        break;
                    default:
                        addPotionNum = -1;
                        break;
                }
                break;
            case "Blue_Vac":
                switch (itemKey1)
                {
                    case "Hub1":
                        addPotionNum = 7;
                        break;
                    default:
                        addPotionNum = -1;
                        break;
                }
                break;
            case "Yellow_Vac":
                switch (itemKey1)
                {
                    case "Hub2":
                        addPotionNum = 8;
                        break;
                    default:
                        addPotionNum = -1;
                        break;
                }
                break;
            case "Hub0":
                switch (itemKey1)
                {
                    case "Red_Vac":
                        addPotionNum = 6;
                        break;
                    default:
                        addPotionNum = -1;
                        break;
                }
                break;
            case "Hub1":
                switch (itemKey1)
                {
                    case "Blue_Vac":
                        addPotionNum = 7;
                        break;
                    default:
                        addPotionNum = -1;
                        break;
                }
                break;
            case "Hub2":
                switch (itemKey1)
                {
                    case "Yellow_Vac":
                        addPotionNum = 8;
                        break;
                    default:
                        addPotionNum = -1;
                        break;
                }
                break;
            default:
                addPotionNum = -1;
                break;
        }
        if (addPotionNum < 0)
        {
            Debug.Log("잘못된 조합");
            isHub = false;
            isPotion = false;
            isSlot0Full = false;
            isFull = false;
            PlayerPrefs.SetString("potName", "empty");
            StartCoroutine(Toast());
            return;
        }

        boilTime = 0;
        isBoiling = true;

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var meta1 = entity.Get<List<BGEntity>>("Boil");

        meta1[0].Set("isBoiling", true);
        meta1[0].Set("potionNum", addPotionNum);
        slider.gameObject.SetActive(true);
        maxTime = 20f;
        slider.maxValue = maxTime;
        meta1[0].Set("maxTime", maxTime);
        if (GameSceneManager.ins.isInventory) Inventory.ins.SetInventory();
        DataManager.instance.SaveFile();

    }


    WaitForSeconds addBoiledTime = new WaitForSeconds(0.3f);
    WaitForSeconds addBoiledTime2 = new WaitForSeconds(0.5f);
    IEnumerator AddBoiledPotion()
    {
        var basePos = slot2.transform.parent.position;
        //yield return new WaitForSeconds(0.5f);
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var pot = entity.Get<List<BGEntity>>("Boil");
        var potionKind = entity.Get<List<BGEntity>>("Potion");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<int>("PotionNum") == potionKind[addPotionNum].Get<int>("PotionNum"))
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot2.transform.parent.gameObject.SetActive(true);
                slot2.InitItem(potionKind[i].Get<string>("key"));
            }
        }
        yield return addBoiledTime;

        slot2.transform.parent.transform.DOMove(GameSceneManager.ins.inventoryBtn.position, 0.5f);
        slot2.transform.parent.transform.DOScale(0, 0.5f);

        yield return addBoiledTime2;
        slot2.transform.parent.transform.localScale = new Vector3(1, 1, 1);
        slot2.transform.parent.transform.position = slot2BasePos;

        slot2.transform.parent.gameObject.SetActive(false);
        
        boilTime = 0;
        isBoiled = false;
        isBoiling = false;
        isSlot0Full = false;
        isFull = false;
        isHub = false;
        isPotion = false;

        pot[0].Set<bool>("isBoiling", isBoiling);
        pot[0].Set<bool>("isBoiled", isBoiled);
        pot[0].Set<float>("Boiltime", 0);
        slider.gameObject.SetActive(false);

        if (GameSceneManager.ins.isInventory) Inventory.ins.SetInventory();

        DataManager.instance.SaveFile();

    }

    WaitForSeconds waitSec = new WaitForSeconds(0.8f);
    public IEnumerator Toast()
    {
        mixToast.SetActive(true);
        yield return waitSec;
        mixToast.SetActive(false);
    }
}
