using BansheeGz.BGDatabase;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static InventorySlot instance;

    public bool isBag;
    public Image icon;
    private DB_Kind_Item curItem;


    public bool isBeaker;
    public bool isBeakerEnter;

    public bool isPotEnter;
    public bool isPot;

    public bool isMortarEnter;
    public bool isMortar;

    private DB_Inventory invenData;

    public TMP_Text text;

    public DB_Potion curPotion;

    public DB_HubPowder curPowder;

    private void Awake()
    {
        instance = this;
        icon = GetComponent<Image>();
    }

    public void InitItem(DB_Kind_Item item)
    {
        curItem = item;
        icon.sprite = ExtendFunction.ins.SpriteReturn(item.key);
    }

    public void InitItem(DB_Potion potion)
    {
        curPotion = potion;
        icon.sprite = ExtendFunction.ins.SpriteReturn(potion.Get<string>("key"));
    }
    public void InitItem(string key)
    {
        icon.sprite = ExtendFunction.ins.SpriteReturn(key);
    }

    public void InitItem(DB_HubPowder powder)
    {
        curPowder = powder;
        icon.sprite = ExtendFunction.ins.HubPowderSpriteReturn(powder.Get<string>("key"));
    }

    public bool isPressItem;
    private void Update()
    {
        if (isPressItem && !isBag)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameSceneManager.ins.tapNum != 1 && isBag) return;

        GameSceneManager.ins.itemName = gameObject.name;
        isPressItem = true;
        transform.SetParent(GameSceneManager.ins.m_canvas);


    }
 
    // IPointer Interface�� ����� ������ ����
    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (isBag)
            return;

        // �������� ���濡 �� ���¿��� ��ġ�� �����ٸ� �������� ����.
        if (isBagEnter && DataManager.instance.open)
        {
            Debug.Log("������ ����");
            // �̹� �������� ����Ǿ� �ִٸ� ������ ���ڸ���
            if (Bag.ins.isPacking)
            {
                isPressItem = false;
                transform.SetParent(Inventory.ins.content);
                return;
            }
            SoundManager.instance.EffectPlay(SoundManager.instance.bagEffect);
            bag.ChangeItem();
            GameSceneManager.ins.SubInventoryItem();
            Destroy(gameObject);

        }
        // �������� ��Ŀ�� �� ���¿��� ��ġ�� �����ٸ� ��Ŀ�� ������ �߰�
        else if (isBeakerEnter && !Beaker.instance.isSlot0Full && DataManager.instance.mixUnlock)
        {
            beaker.ChangeItem();
            GameSceneManager.ins.SubInventoryItem();
            Destroy(gameObject);
        }
        else if (isBeakerEnter && Beaker.instance.isSlot0Full && !Beaker.instance.isFull)
        {
            beaker.ChangeItem1();
            GameSceneManager.ins.SubInventoryItem();
            Destroy(gameObject);
        }
        
        else if (isMortarEnter)
        {
            Mortar.instance.ChangeHub();
            GameSceneManager.ins.SubInventoryItem();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("������ �κ��丮 ���ư�");
            isPressItem = false;
            transform.SetParent(Inventory.ins.content);
        }
        Inventory.ins.SetInventory();
    }

    public bool isBagEnter;
    Bag bag;
    public Beaker beaker;
    public Mortar mortar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameSceneManager.ins.tapNum == 1)
        {
            if (isBag || Bag.ins.isPacking)
                return;
        }
        
        // �������� ������Ʈ�� ���ٸ� bool�� ����
        switch (collision.tag)
        {
            case "Bag":
                Debug.Log("������ Ʈ����");
                bag = collision.GetComponent<Bag>();
                isBagEnter = true;
                break;

            case "Beaker":
                Debug.Log("Beaker Trigger");
                beaker = collision.GetComponent<Beaker>();
                isBeakerEnter = true;
                break;

            case "Mortar":
                Debug.Log("Mortar Trigger");
                mortar = collision.GetComponent<Mortar>();
                isMortarEnter = true;
                break;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isBag || isBeaker || isPot || isMortar)
            return;

        switch (collision.tag)
        {
            case "Bag":
                Debug.Log("������ Ʈ���� ����");
                isBagEnter = false;
                break;
            case "Beaker":
                Debug.Log("Beaker Trigger Exit");
                isBeakerEnter = false;
                break;
            case "Pot":
                Debug.Log("Pot Trigger Exit");
                isPotEnter = false;
                break;
            case "Mortar":
                Debug.Log("Mortar Trigger Exit");
                isMortarEnter = false;
                break;
        }
    }
}
