using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{
    public Button ThrowFoodButton;

    private void Start()
    {
        ThrowFoodButton.onClick.AddListener(SessionManager.Instance.User.ThrowFood);
    }
}
