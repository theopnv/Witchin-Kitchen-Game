using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenStation : MonoBehaviour
{
    private CookingMinigame m_miniGame;
    private GameObject m_StoredIngredient;

    private void Start()
    {
        m_miniGame = GetComponent<CookingMinigame>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickableObject ingredient = collision.gameObject.GetComponent<PickableObject>();
        if (ingredient)
        {
            m_StoredIngredient = collision.gameObject;
            m_StoredIngredient.SetActive(false);
            m_miniGame.StartMinigame(); //coroutine?
        }
    }
}
