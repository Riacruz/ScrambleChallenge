using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickLetras : MonoBehaviour
{
    private Teclado tec;
    private Button but;
    private Animator anim;
    private AudioSource sound;
    void Start()
    {
        sound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        but = GetComponent<Button>();
        tec = GameObject.Find("Teclado").GetComponent<Teclado>();
        GetComponent<Image>().color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 255);
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetBool("FadeOut"))
        {
            but.interactable = false;
        } else
        {
            but.interactable = true;
        }
    }

    public void OnClickThis()
    {
        //print(gameObject.name);
        if (!tec.noPuedeEscribir)
        {
            sound.Play();
            tec.OnClickLetter(gameObject.name.Remove(1, 7));
            anim.SetBool("FadeOut", true);
            tec.ultimaLetraUsada.Add( gameObject);
        }
    }
}
