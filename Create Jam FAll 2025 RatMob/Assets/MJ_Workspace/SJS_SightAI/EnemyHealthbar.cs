using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    private Transform cam;
    private Image fill;
    private Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        fill = slider.fillRect.GetComponent<Image>();
        cam = Camera.main.transform;
       
    }
    private void Update()
    {
        transform.LookAt(transform.position + cam.forward);

    }
    public void BarUpdate(float percent,Color color)
    {
        slider.gameObject.SetActive(percent > 0|| color==Color.red);
        slider.value = percent;
        fill.color = color;
    }

}
