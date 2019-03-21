using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{
    public class OnCompletePotion : MonoBehaviour
    {
        public GameObject m_potionPrefab;
        public Image m_audienceInterface;

        public void OnPotionComplete(RecipeManager cauldron)
        {
            var potionObj = Instantiate(m_potionPrefab, cauldron.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
            var potion = potionObj.GetComponent<FlyingPotion>();

            //Set color
            var liquidColor = ColorsManager.Get().CauldronLiquidColors[cauldron.GetOwner().ID];
            potion.SetColor(liquidColor);

            //Set Destination
            potion.SetDestination(m_audienceInterface);
            
        }
    }
}
