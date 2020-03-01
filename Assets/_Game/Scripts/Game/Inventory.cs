///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Inventory (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/29/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using Tofunaut.Animation;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Inventory : MonoBehaviour
    {
        private const float IntoInventoryAnimTime = 0.7f;

        private HashSet<InventoryItem> _items = new HashSet<InventoryItem>();

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {

                foreach(InventoryItem item in new List<InventoryItem>(_items))
                {
                    Remove(item, false);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        public void Add(InventoryItem item)
        {
            item.transform.SetParent(transform);
            _items.Add(item);

            Vector3 startLocalPosition = item.transform.localPosition;
            int prevRenderOrder = -1;
            string prevRenderLayer = string.Empty;

            new TofuAnimation()
                .Execute(() => 
                {
                    if(item.SpriteRenderer)
                    {
                        prevRenderLayer = item.SpriteRenderer.sortingLayerName;
                        prevRenderOrder = item.SpriteRenderer.sortingOrder;
                        item.SpriteRenderer.sortingOrder = 1;
                        item.SpriteRenderer.sortingLayerName = "Default";
                    }
                })
                .Value01(IntoInventoryAnimTime, EEaseType.EaseInBack, (float newValue) =>
                {
                    item.transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero, newValue);
                    item.transform.localPosition = Vector3.LerpUnclamped(startLocalPosition, Vector3.zero, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    item.gameObject.SetActive(false);

                    if (item.SpriteRenderer)
                    {
                        item.SpriteRenderer.sortingOrder = prevRenderOrder;
                        item.SpriteRenderer.sortingLayerName = prevRenderLayer;
                    }
                })
                .Play();
        }

        // --------------------------------------------------------------------------------------------
        public void Remove(InventoryItem item, bool destroy)
        {
            _items.Remove(item);

            if(destroy)
            {
                Destroy(item.gameObject);
            }
            else
            {
                item.transform.SetParent(null);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = item.transform.localPosition.RoundToInt();
                item.gameObject.SetActive(true);
            }
        }

        // --------------------------------------------------------------------------------------------
        public bool ContainsItem(InventoryItem item) => _items.Contains(item);
    }
}