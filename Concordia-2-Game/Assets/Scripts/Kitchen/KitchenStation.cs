using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenStation : MonoBehaviour
{
    private CookingMinigame m_miniGame;
    private GameObject m_storedIngredient;
    [SerializeField]
    private bool m_hasOwner;

    private void Start()
    {
        m_miniGame = GetComponent<CookingMinigame>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickableObject ingredient = collision.gameObject.GetComponent<PickableObject>();
        if (ingredient)
        {
            m_storedIngredient = collision.gameObject;
            m_storedIngredient.SetActive(false);
            m_miniGame.StartMinigame();
        }
    }

    public void SetOwner(GameObject owner)
    {
        if (m_hasOwner)
        {
            m_miniGame.SetStationOwner(owner);

            //Apply player color to station?
        }
    }
}
