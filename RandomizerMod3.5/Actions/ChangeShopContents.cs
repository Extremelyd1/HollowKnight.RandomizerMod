using System.Collections.Generic;
using RandomizerMod.Randomization;
using UnityEngine;

namespace RandomizerMod.Actions
{
    public struct ShopItemDef
    {
        // Values from ShopItemStats
        public string PlayerDataBoolName;
        public string NameConvo;
        public string DescConvo;
        public string RequiredPlayerDataBool;
        public string RemovalPlayerDataBool;
        public bool DungDiscount;
        public string NotchCostBool;
        public int Cost;

        // Sprite name in resources
        public string SpriteName;
    }


    public class ChangeShopContents : RandomizerAction, ISerializationCallbackReceiver
    {
        // Variable for serialization hack
        private List<string> _itemDefStrings;
        private ShopItemDef[] _items;

        // Variables that actually get used

        public ChangeShopContents(string sceneName, string objectName, ShopItemDef[] items)
        {
            SceneName = sceneName;
            ObjectName = objectName;
            _items = items;
        }

        public override ActionType Type => ActionType.GameObject;

        public string SceneName { get; }

        public string ObjectName { get; }

        public void OnBeforeSerialize()
        {
            _itemDefStrings = new List<string>();
            foreach (ShopItemDef item in _items)
            {
                _itemDefStrings.Add(JsonUtility.ToJson(item));
            }
        }

        public void OnAfterDeserialize()
        {
            List<ShopItemDef> itemDefList = new List<ShopItemDef>();

            foreach (string item in _itemDefStrings)
            {
                itemDefList.Add(JsonUtility.FromJson<ShopItemDef>(item));
            }

            _items = itemDefList.ToArray();
        }

        public void AddItemDefs(ShopItemDef[] newItems)
        {
            if (_items == null)
            {
                _items = newItems;
                return;
            }

            if (newItems == null)
            {
                return;
            }

            ShopItemDef[] combined = new ShopItemDef[_items.Length + newItems.Length];
            _items.CopyTo(combined, 0);
            newItems.CopyTo(combined, _items.Length);
            _items = combined;
        }

        public override void Process(string scene, Object changeObj)
        {
            if (scene != SceneName)
            {
                return;
            }

            foreach (GameObject shopObj in Object.FindObjectsOfType<GameObject>())
            {
                if (shopObj.name != ObjectName) continue;

                ShopMenuStock shop = shopObj.GetComponent<ShopMenuStock>();
                GameObject itemPrefab = Object.Instantiate(shop.stock[0]);
                itemPrefab.SetActive(false);

                // Remove all charm type items from the store
                List<GameObject> newStock = new List<GameObject>();

                foreach (ShopItemDef itemDef in _items)
                {
                    // Create a new shop item for this item def
                    GameObject newItemObj = Object.Instantiate(itemPrefab);
                    newItemObj.SetActive(false);

                    // Apply all the stored values
                    ShopItemStats stats = newItemObj.GetComponent<ShopItemStats>();
                    stats.playerDataBoolName = itemDef.PlayerDataBoolName;
                    stats.nameConvo = itemDef.NameConvo;
                    stats.descConvo = itemDef.DescConvo;
                    stats.requiredPlayerDataBool = itemDef.RequiredPlayerDataBool;
                    stats.removalPlayerDataBool = itemDef.RemovalPlayerDataBool;
                    stats.dungDiscount = itemDef.DungDiscount;
                    stats.notchCostBool = itemDef.NotchCostBool;
                    stats.cost = itemDef.Cost;

                    // Need to set all these to make sure the item doesn't break in one of various ways
                    stats.priceConvo = string.Empty;
                    stats.specialType = 2;
                    stats.charmsRequired = 0;
                    stats.relic = false;
                    stats.relicNumber = 0;
                    stats.relicPDInt = string.Empty;

                    // Apply the sprite for the UI
                    stats.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite =
                        RandomizerMod.GetSprite(itemDef.SpriteName);

                    // Treat Lore as a Wanderer's Journal; we're not modifying Lemm Shop so this shouldn't cause issues
                    if (LogicManager.GetItemDef(stats.playerDataBoolName.Split('.')[2]).type == ItemType.Lore)
                    {
                        stats.specialType = 4;
                    }

                    newStock.Add(newItemObj);
                }

                foreach (GameObject item in shop.stock)
                {// Update normal stock (specialType: 0 = lantern, elegant key, quill; 1 = mask, 2 = charm, 3 = vessel, 4-7 = relics, 8 = notch, 9 = map, 10 = simple key, 11 = egg, 12-14 = repair fragile, 15 = salubra blessing, 16 = map pin, 17 = map marker)
                    if (RandomizerMod.Instance.Settings.RandomizeMaps)
                    {
                        if (item.GetComponent<ShopItemStats>().specialType == 9 || item.GetComponent<ShopItemStats>().playerDataBoolName == "hasQuill") continue;
                    }

                    string shopBool = item.GetComponent<ShopItemStats>().playerDataBoolName;
                    if (!LogicManager.HasItemWithShopBool(shopBool))
                    {// LogicManager doesn't know about this shop item, which means it's never potentially randomized. Put it back!
                        if (!(shopBool.StartsWith("salubraNotch") && RandomizerMod.Instance.Settings.CharmNotch))
                        {// If Salubra QOL is off, we need to add these notches back into her shop.
                            newStock.Add(item);
                        }
                    }
                }

                shop.stock = newStock.ToArray();

                // Update alt stock; Sly only
                if (shop.stockAlt != null)
                {
                    // Save unchanged list for potential alt stock
                    List<GameObject> altStock = new List<GameObject>();
                    altStock.AddRange(newStock);

                    foreach (GameObject item in shop.stockAlt)
                    {
                        string shopBool = item.GetComponent<ShopItemStats>().playerDataBoolName;
                        if (!LogicManager.HasItemWithShopBool(shopBool) && !newStock.Contains(item))
                        {
                            altStock.Add(item);
                        }
                    }

                    shop.stockAlt = altStock.ToArray();
                }
            }
        }
    }
}
