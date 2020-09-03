using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MueveLetras : MonoBehaviour
{
    private Teclado tec;
    public GameObject[] positions;
    public GameObject[] pos;
    public GameObject[] letras;
    private GameObject canvas;
    public char[] letrasParaInt;
    public List<GameObject> letrasActual;
    public string desordenadaActual;
    public char[] ch;

    // Start is called before the first frame update
    void Awake()
    {
        
        letrasActual = new List<GameObject>();
        canvas = GameObject.Find("Canvas");
        tec = GetComponent<Teclado>();
        letrasParaInt = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
       

        
    }

    private GameObject[] EligePosicion(GameObject[] go, int size) //Elige el gameobject de posicion segun el tamaño
    {
        var b = go[size].GetComponentsInChildren<Image>();
        GameObject[] p = new GameObject[b.Length];
        for (int i = 0; i < b.Length; i++)
        {
            p[i] = b[i].gameObject;
            print(p[i].name);
        }
        return p;
    }


    public void DoIt()
    {
        GameObject padre = GameObject.FindGameObjectWithTag("Letras");
        RemoveLetras();
        print(tec.desordenadaActual);
        ch = tec.desordenadaActual.ToCharArray();
        //var palabraChar = tec.palabraActual.ToCharArray();
        print("length: "+ch.Length);
        positions = EligePosicion(pos, ch.Length);
        for (int i = 0; i < ch.Length; i++)
        {
            //print(ch[i]);
            //print(letras[System.Array.IndexOf(letrasParaInt, ch[i])].name);
            GameObject l = Instantiate(letras[System.Array.IndexOf(letrasParaInt, ch[i])], positions[i].transform.position, Quaternion.identity);
            
            l.transform.SetParent(padre.transform);
            
            letrasActual.Add(l);
            
        }

        //print("EI? " + letrasActual[0].ToString().Remove(1, letrasActual[0].ToString().Length-1));

    }

    

    public void OnClickRemoveLetters()
    {
        if (!tec.noPuedeEscribir)
        {
            tec.resultado.color = tec.colorAmarillo;
            tec.resultado.GetComponent<Animator>().enabled = false;
            for (int i = 0; i < letrasActual.Count; i++)
            {
                letrasActual[i].GetComponent<Animator>().SetBool("FadeOut", false);

            }
            tec.resultado.text = tec.pista + "";
        }
    }

    public void OnClickRanking()
    {
        if (Social.localUser.authenticated)
        {
            ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI("CgkI6M_74ckQEAIQAQ");
        }
        else
        {
          Social.localUser.Authenticate((bool success) => { });
        }
    }

    public void OnClickReniciarPArtida()
    {
        SceneManager.LoadScene(0);
    }

    public void RemoveLetras()
    {
        for(int i=0; i < letrasActual .Count; i++)
        {
            Destroy(letrasActual[i]);
        }
        letrasActual.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            tec.OnClickBack();
        }

        if(Input.GetButtonDown("Fire1"))
        {
            //print(letrasActual.Count);
        }
    }
}
