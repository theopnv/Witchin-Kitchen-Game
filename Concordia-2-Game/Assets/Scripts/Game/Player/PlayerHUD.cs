using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public class PlayerHUD : MonoBehaviour
    {
        private DetectController _DetectController;
        public int OwnerId = -1;
        public Text Message;
        public GameObject RecipeIconsParent;
        public Text Score;
        public PlayerManager Manager;

        void Start()
        {
            _DetectController = FindObjectOfType<DetectController>();
            _DetectController.OnConnected += OnConnected;
            _DetectController.OnDisconnected += OnDisconnected;
        }

        void OnDisable()
        {
            _DetectController.OnConnected -= OnConnected;
            _DetectController.OnDisconnected -= OnDisconnected;
        }

        void OnConnected(int i)
        {
            if (i == OwnerId)
            {
                Manager?.SendMessageToPlayerInHUD("", Color.red);
            }
        }

        void OnDisconnected(int i)
        {
            if (i == OwnerId)
            {
                Manager?.SendMessageToPlayerInHUD("Your controller was disconnected.", Color.red, true);
            }
        }

        public void CollectIngredient(Ingredient type)
        {
            for (int i = 0; i < RecipeIconsParent.transform.childCount; i++)
            {
                var child = RecipeIconsParent.transform.GetChild(i);
                if (child.GetComponent<IngredientType>().m_type == type)
                {
                    var renderer = child.GetComponent<Image>();
                    var color = renderer.color;
                    if (color.a > 0.75f)
                    {
                        renderer.color = new Color(0.5f * color.r, 0.5f * color.g, 0.5f * color.b, 0.3f);
                        break;
                    }
                }
            }

        }

        public void SetNewRecipeIcons(List<Image> icons)
        {
            //Clear old recipe
            while (RecipeIconsParent.transform.childCount > 0)
            {
                var child = RecipeIconsParent.transform.GetChild(0);
                child.parent = null;
                GameObject.Destroy(child.gameObject);
            }

            foreach (var icon in icons)
            {
                Instantiate(icon, RecipeIconsParent.transform);
            }
        }
    }

}
