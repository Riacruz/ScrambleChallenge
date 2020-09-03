using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portada : MonoBehaviour
{
    public GameObject[] letras;
    public ParticleSystem particle;
    private GameObject posActual;
    public Animator animGrupo;
   // public Admob admob;

    // Start is called before the first frame update
    async void Start()
    {
       // admob = gameObject.GetComponent<Admob>();
        foreach(GameObject s in letras )
        {
            posActual  = s;
            s.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (posActual.tag == "LetraPortada")
        {
            particle.transform.position = posActual.transform.position;
        }
    }
    
       

    public async void OnClickPlay()
    {
        this.GetComponent<AudioSource>().Play();
        animGrupo.enabled = true;
        await Task.Delay(TimeSpan.FromSeconds(2f));
        //admob.ShowAdd();        
        SceneManager.LoadScene(1);
    }
}
