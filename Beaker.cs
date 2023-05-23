using BansheeGz.BGDatabase;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Beaker : MonoBehaviour, IPointerClickHandler
{

    public static Beaker instance;

    public InventorySlot slot0;
    public InventorySlot slot1;
    public InventorySlot slot2;

    public Vector2 basePos;
    public Image img;
    public Image img1;
    public Image img2;

    public Image beakerImg;

    public Transform parent;

    public DB_Potion curPotion;
    public DB_Potion curPotion1;
    public DB_HubPowder curHubPowder;

    public int potionNum;
    public int potionNum1;
    public int addPotionNum;
    public int hubPowderNum;

    public int itemNum;
    public int itemNum1;

    bool isHubPowder;
    bool isPotion;


    public BoxCollider2D beakerCol;
    Vector3 slot0BasePos;
    Vector3 slot1BasePos;
    Vector3 slot2BasePos;

    public GameObject lockPanel;

    public GameObject mixToast;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        img = GetComponentInChildren<Image>();
        img1 = GetComponentInChildren<Image>();
        img2 = GetComponentInChildren<Image>();
        basePos = transform.position;
        parent = transform.parent;
        slot0.transform.parent.gameObject.SetActive(false);
        slot1.transform.parent.gameObject.SetActive(false);
        slot2.transform.parent.gameObject.SetActive(false);
        slot2BasePos = slot2.transform.parent.position;
        // ��Ŀ�� �رݵǾ��ٸ� lockPanel ����
        if (DataManager.instance.mixUnlock) Destroy(lockPanel);
    }

    public bool isSlot0Full;
    public bool isFull;

    // IPointer Interface�� ���� Ŭ�� �̺�Ʈ ����
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(beakerCol.gameObject.name);
        
        // ��Ŀ�� �ΰ��� ������ ��� á���� ����
        if (isFull)
        {
            // ���ʰ� ������ �ʴٸ� ���� + ���� ���չ����� ����
            if (!isHubPowder) MixPotion(potionNum, potionNum1);
            else
            {
                // ���ʰ� ���ְ�, ù��° ������ �ι�° ����� ���� �ʴٸ� ���� + ���� ���չ����� ����.
                if (!isPotion) potionNum = potionNum1;
                HubPotion(potionNum, hubPowderNum);
                isHubPowder = false;
            }
            SoundManager.instance.effectAudioSource.clip = SoundManager.instance.mixEffect;
            SoundManager.instance.effectAudioSource.Play();

            // ù��°, �ι�° ���� SetActive(false)
            slot0.transform.parent.gameObject.SetActive(false);
            slot1.transform.parent.gameObject.SetActive(false);
            
            // �߸��� ���չ��� �ƴ϶�� AddMixPotion()����
            if (addPotionNum != -1)
            {
                SoundManager.instance.effectAudioSource.clip = SoundManager.instance.mixEffect;
                SoundManager.instance.effectAudioSource.Play();

                DataManager.instance.AddPotion(addPotionNum);
                StartCoroutine(AddMixPotion());
            }
            else
            {
                // �߸��� ���չ��̶�� �佺Ʈ ����
                StartCoroutine(Toast());
                SoundManager.instance.effectAudioSource.clip = SoundManager.instance.wrongEffect;
                SoundManager.instance.effectAudioSource.Play();
            }
            isSlot0Full = false;
            isFull = false;

        }

        else return;
        Debug.Log(gameObject.tag);
    }


    public string potionName;

    // ù��° ���Կ� �� ���� �Ǵ� ���� ����
    public void ChangeItem()
    {
        Debug.Log("ChangeItem");

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");
        var hubPowderKind = entity.Get<List<BGEntity>>("HubPowder");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                slot0.InitItem(potionKind[i].Get<string>("key"));
                potionNum = potionKind[i].Get<int>("PotionNum");
                isPotion = true;
                isSlot0Full = true;
                return;
            }
        }
        for (int i = 0; i < hubPowderKind.Count; i++)
        {
            if (hubPowderKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curHubPowder = DB_HubPowder.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot0.transform.parent.gameObject.SetActive(true);
                slot0.InitItem(hubPowderKind[i].Get<string>("key"));
                hubPowderNum = hubPowderKind[i].Get<int>("powderNum");
                isHubPowder = true;
                isSlot0Full = true;
            }
        }

    }

    // �ι�° ���Կ� �� ���� �Ǵ� ���� ���� 
    public void ChangeItem1()
    {
        Debug.Log("ChangeItem1()");

        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");
        var hubPowderKind = entity.Get<List<BGEntity>>("HubPowder");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curPotion1 = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot1.transform.parent.gameObject.SetActive(true);
                slot1.InitItem(potionKind[i].Get<string>("key"));
                potionNum1 = potionKind[i].Get<int>("PotionNum");
                isFull = true;
            }
        }

        for (int i = 0; i < hubPowderKind.Count; i++)
        {
            if (hubPowderKind[i].Get<string>("key") == GameSceneManager.ins.itemName)
            {
                curHubPowder = DB_HubPowder.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot1.transform.parent.gameObject.SetActive(true);
                slot1.InitItem(hubPowderKind[i].Get<string>("key"));
                hubPowderNum = hubPowderKind[i].Get<int>("powderNum");
                isHubPowder = true;
                isFull = true;
            }
        }

    }

    // ����� ���� ����
    public void MixPotion(int potionNum, int potionNum1)
    {
        Debug.Log("MixPotion");
        switch (potionNum)
        {
            case 0:
                switch (potionNum1)
                {
                    case 1:
                        addPotionNum = 5;
                        Debug.Log("OrangePotion");
                        break;
                    case 2:
                        addPotionNum = 4;
                        Debug.Log("PuplePotion");
                        break;
                    default:
                        Debug.Log("�߸��� ����");
                        addPotionNum = -1;
                        break;
                }
                break;
            case 1:
                switch (potionNum1)
                {
                    case 0:
                        addPotionNum = 5;
                        Debug.Log("OrangePotion");
                        break;
                    case 2:
                        addPotionNum = 3;
                        Debug.Log("GreenPotion");
                        break;
                    default:
                        addPotionNum = -1;
                        Debug.Log("�߸��� ����");
                        break;
                }
                break;
            case 2:
                switch (potionNum1)
                {
                    case 0:
                        addPotionNum = 4;
                        Debug.Log("PurplePotion");
                        break;
                    case 1:
                        addPotionNum = 3;
                        Debug.Log("GreenPotion");
                        break;
                    default:
                        Debug.Log("�߸��� ����");
                        addPotionNum = -1;
                        break;
                }
                break;
            default:
                Debug.Log("�߸��� ����");
                addPotionNum = -1;
                break;
        }

    }

    // ����� ���� ����
    public void HubPotion(int potionNum, int hubNum)
    {
        switch (potionNum)
        {
            case 0:
                switch (hubNum)
                {
                    case 0:
                        addPotionNum = 6;
                        break;
                    default:
                        Debug.Log("�߸��� ����");
                        addPotionNum = -1;
                        break;
                }
                break;
            case 1:
                switch (hubNum)
                {
                    case 1:
                        addPotionNum = 8;
                        break;
                    default:
                        addPotionNum = -1;
                        Debug.Log("�߸��� ����");
                        break;
                }
                break;
            case 2:
                switch (hubNum)
                {
                    case 2:
                        addPotionNum = 7;
                        break;
                    default:
                        Debug.Log("�߸��� ����");
                        addPotionNum = -1;
                        break;
                }
                break;
            default:
                Debug.Log("�߸��� ����");
                addPotionNum = -1;
                break;
        }

    }

    WaitForSeconds addMixPotionTime = new WaitForSeconds(0.2f);
    WaitForSeconds addMixPotionTime2 = new WaitForSeconds(0.8f);
    WaitForSeconds addMixPotionTime3 = new WaitForSeconds(0.5f);
 
    // ������ ������ �κ��丮�� �ִ� ���
    IEnumerator AddMixPotion()
    {
        //var basePos = slot2.transform.parent.position;

        yield return addMixPotionTime;
        var repo = BGRepo.I;
        var meta = repo["PlayData"];
        var entity = meta[0];
        var potionKind = entity.Get<List<BGEntity>>("Potion");

        for (int i = 0; i < potionKind.Count; i++)
        {
            if (potionKind[i].Get<int>("PotionNum") == potionKind[addPotionNum].Get<int>("PotionNum"))
            {
                curPotion = DB_Potion.FindEntity(t => t.key.Equals(GameSceneManager.ins.itemName));
                slot2.transform.parent.gameObject.SetActive(true);

                // Database�� ����� key�� ���� ��Ŀ�� �� ����
                beakerImg.sprite = ExtendFunction.ins.BeakerSpriteReturn(potionKind[i].Get<string>("key"));

                slot2.InitItem(potionKind[i].Get<string>("key"));
            }
        }
        yield return addMixPotionTime2;

        // DOTween�� ���� inventoryBtn�� ��ġ�� ���յ� ���� ������Ʈ�� ��ġ �̵�, ���� ������Ʈ�� ũ�⸦ ����
        slot2.transform.parent.transform.DOMove(GameSceneManager.ins.inventoryBtn.position, 0.5f);
        slot2.transform.parent.transform.DOScale(0, 0.5f);
        yield return addMixPotionTime3;

        // DOTween�� ���� ����� ������Ʈ�� ũ��� ��ġ�� ������ ��ġ�� �缳��
        slot2.transform.parent.transform.localScale = new Vector3(1, 1, 1);
        slot2.transform.parent.transform.position = slot2BasePos;
        slot2.transform.parent.gameObject.SetActive(false);

        // ��Ŀ�� �̹����� �� �̹����� ����
        beakerImg.sprite = ExtendFunction.ins.BeakerSpriteReturn("Mix_Machine(empty)");

        // �κ��丮 ���ΰ�ħ
        if (GameSceneManager.ins.isInventory) Inventory.ins.SetInventory();

        DataManager.instance.SaveFile();
    }


    WaitForSeconds waitSec = new WaitForSeconds(0.8f);

    // �߸��� �������� ���� ���� �� �佺Ʈ ����
    public IEnumerator Toast()
    {
        mixToast.SetActive(true);
        yield return waitSec;
        mixToast.SetActive(false);
    }
}
