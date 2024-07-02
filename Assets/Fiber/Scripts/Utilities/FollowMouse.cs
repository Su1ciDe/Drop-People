using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] hands;

    private void Update()
    {
        transform.position = Input.mousePosition - new Vector3(-50 , 100,0);
        if (Input.GetMouseButtonDown(0))
        {
            image.sprite = hands[0];

        }
        if (Input.GetMouseButtonUp(0))
        {
            image.sprite = hands[1];

        }
    }
}
