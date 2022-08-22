using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class hideWhenFlippedOver : MonoBehaviour
{
    public TextMeshProUGUI text;
    public SpriteRenderer sprite;
    public Image image;
    private BusController _bus;
    private void Start()
    {
        _bus = FindObjectOfType<BusController>();
    }

    void Update() {
        if (_bus.isFlippedOver) {
            if (text != null) text.color = new Color (0, 0, 0, 0);
            if (sprite != null) sprite.color = new Color (0, 0, 0, 0);
            if (image != null ) image.color = new Color (0, 0, 0, 0);
        }
    }
}
