using UnityEngine;

//Used to quickly switch between shop-presets
[CreateAssetMenu(menuName = "FF-Assets/Shop")]
public class ShopData : ScriptableObject
{
    public ItemData[] itemsInShop;
}
