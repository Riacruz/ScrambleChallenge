using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using System;
using System.Timers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class Teclado : MonoBehaviour
{
    public TextMeshProUGUI resultado, palabra, desordenada, temaPista, tiempoTotal, tiempoPalabra, sumaTiempo, monedas, marcador;
    private string[,] contenido = new string[1, 3];
    private string[] tema, todo;
    public string pista, palabraActual, desordenadaActual;
    private char[] desordenadaChar;
    private int pistaInt;
    public bool noPuedeEscribir;
    public GameObject sparks;
    private ParticleSystem particle;
    public ParticleSystem particlePeque, particlePeque2;
    private Vector3 touchPos;
    private int touchCount, totalMonedas,totalMarcador;
    private Animator animDesordenada, animTemaPista;
    List<string[]> arrayFinal = new List<string[]>();
    private float timeTotal, timeWord, timeMoneda;
    public GameObject panelVideo, panelOptions;
    private bool finalDePartida;
    public  Color colorAmarillo;
    private MueveLetras mueve;
    public List<GameObject> ultimaLetraUsada = new List<GameObject>();
    public Canvas canvas;
    public Animator animMonedas;
    public AudioClip soundAcierta, soundPista;
    private Admob admob;

    // Start is called before the first frame update
    private void Awake()
    {
        /*
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = false; //OJO desactivar al finalizar
        */
    }
    void Start()
    {
        admob = gameObject.GetComponent<Admob>();
        //((PlayGamesPlatform)Social.Active).Authenticate((bool success) => { }, true);
        /*
        Social.localUser.Authenticate((bool success) => {
             // handle success or failure
        });
        */
        colorAmarillo = temaPista.color;
        totalMarcador = 0;
        marcador.text = "Puntos: "+totalMarcador.ToString("D2");
        particle = sparks.GetComponent<ParticleSystem>();
        animDesordenada = desordenada.GetComponent<Animator>();
        animTemaPista = temaPista.GetComponent<Animator>();
        totalMonedas = 10;
        monedas.text = totalMonedas.ToString() ;
        CargaContenido();
        print("CargaContenido");
        timeTotal = 120; 
        timeWord = 10;
        timeMoneda = 5;
        CargaArrayFinal();
        print("CargaArrayFinal");
        mueve = GetComponent<MueveLetras>();
        Elige();
        print("Elige");

        //EligePalabra();

    }
    private void Update()
    {
        if (Util.videoVisto) videoVisto();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(touchCount);
            touchPos = touch.position;
            touchCount += 1;
        }

        if (Util.timeIsRunning)
        {
            timeTotal -= Time.deltaTime;
            if (timeWord > 0) timeWord -= Time.deltaTime;
            if (timeMoneda > 0) timeMoneda -= Time.deltaTime;
        }

        // tiempoTotal.text = time.ToString("f0");
        tiempoTotal.text = ConvierteAMinutosHoras((int)timeTotal);
        tiempoPalabra.text = ((int)timeWord).ToString("D2"); 

        if (timeTotal <= 0 && !finalDePartida)
        {
            Util.timeIsRunning = false;
            FinalDePartida();
            finalDePartida = true;
        }
    }

    private void FinalDePartida()
    {
        particle.Play();
        noPuedeEscribir = true;
        palabra.gameObject.SetActive(false);
        resultado.gameObject.SetActive(false);
        tiempoPalabra.gameObject.SetActive(false);
        tiempoTotal.gameObject.SetActive(false);
        temaPista.gameObject.SetActive(false);
        desordenada.gameObject.SetActive(false);
        GameObject padre = GameObject.FindGameObjectWithTag("Letras");
        padre.SetActive(false);
        resultado.gameObject.GetComponentInParent<RectTransform>().gameObject.SetActive(false);
        marcador.gameObject.GetComponent<Animator>().enabled = true;
        marcador.gameObject.GetComponent<Button>().enabled = true;
        if(PlayerPrefs.HasKey("puntos"))
        {
            if(PlayerPrefs.GetInt("puntos")<totalMarcador)
            {
                if (Social.localUser.authenticated)
                {
                    PlayerPrefs.SetInt("puntos", totalMarcador);
                    Social.ReportScore(totalMarcador, "CgkI6M_74ckQEAIQAQ", (bool success) => { });
                }
                else
                {
                    Social.localUser.Authenticate((bool success) => { });
                    PlayerPrefs.SetInt("puntos", totalMarcador);
                    Social.ReportScore(totalMarcador, "CgkI6M_74ckQEAIQAQ", (bool success) => { });
                }
            }
        }
        else
        {
            if (Social.localUser.authenticated)
            {
                PlayerPrefs.SetInt("puntos", totalMarcador);
                Social.ReportScore(totalMarcador, "CgkI6M_74ckQEAIQAQ", (bool success) => { });
            }
            else
            {
                Social.localUser.Authenticate((bool success) => { });
                PlayerPrefs.SetInt("puntos", totalMarcador);
                Social.ReportScore(totalMarcador, "CgkI6M_74ckQEAIQAQ", (bool success) => { });
            }
        }
        
        //Invoke("CambiaEscena", 5);
       
    }

    public void CambiaEscena()
    {
        SceneManager.LoadScene("GameScene");
    }

    private string ConvierteAMinutosHoras(int tsegundos)
    {
        var horas = (tsegundos / 3600);
        var minutos = ((tsegundos - horas * 3600) / 60);
        var segundos = tsegundos - (horas * 3600 + minutos * 60);
        return minutos.ToString("D2") + ":" + segundos.ToString("D2"); 
       // return horas.ToString("f0") + ":" + minutos.ToString() + ":" + segundos.ToString();
    }

    private void CargaContenido()
    {
        string text = Resources.Load<TextAsset>("ResES").text;
        //string text = System.IO.File.ReadAllText("Assets/Resources/Res.bkt");
        todo = text.Split('*');
    }

    private void CargaArrayFinal()
    {
        for (int i = 0; i < todo.Length; i++)
        {
            string[] thisTema = todo[i].Split(',');
            var tema = thisTema[0];
            for (int e = 1; e < thisTema.Length; e++)
            {
                string[] st = new string[2];
                st[0] = tema;
                st[1] = thisTema[e];
                arrayFinal.Add(st);
            }
        }
    }

    /*
    private void EligeTema()
    {
        var rand = Random.Range(0, todo.Length);
        tema = todo[rand].Split(',');        
        temaPista.text = tema[0].Trim ();
        todo = EliminaElementoDeArray(rand, todo);
    }
    */


    private void EligePalabra()
    {
        resultado.text = "";
        pistaInt = 0;
        pista = "";
        palabra.color = new Color(145, 145, 145, 0);
        var rand = UnityEngine.Random.Range(0, arrayFinal.Count);
        var te = arrayFinal[rand];
        string t = te[0];
        string p = te[1];

        temaPista.text = t.Trim();
        palabra.text = ToUpper(p);
        palabraActual = palabra.text;

        string[] st = new string[palabra.text.Length];
        char[] chars = palabra.text.ToCharArray();
        Shuffle(chars);
        desordenada.text = "";
        foreach (char s in chars)
        {
            desordenada.text += s;
        }

        desordenadaChar = desordenada.text.ToCharArray();
        desordenadaActual = desordenada.text;
        print("EligePalabra " + desordenadaActual);
        arrayFinal.Remove(te);

        mueve.DoIt();


        // print(arrayFinal.Count);
        //tema = EliminaElementoDeArray(rand, tema);
    }

    private void Elige()
    {

        Util.timeIsRunning = true;
        sumaTiempo.gameObject.SetActive(false);
        tiempoPalabra.gameObject.SetActive(true);
        desordenada.gameObject.SetActive(true);
        temaPista.gameObject.SetActive(true);
        noPuedeEscribir = false;
        timeWord = 10;
        timeMoneda = 5;
        tiempoPalabra.text = timeWord.ToString();
        if (arrayFinal.Count > 1) EligePalabra();
        else
        {
            print("Ya no hay mas palabras");
        }
        
        
    }

    /*
    private string[] EliminaElementoDeArray(int indice, string[] array)
    {
        List<string> tmp = new List<string>(array);
        tmp.RemoveAt(indice);
        array = tmp.ToArray();
        return array;
    }
    */



    public static void Shuffle<T>(IList<T> values)
    {
        var n = values.Count;
        //var rnd = new Random();
        for (int i = n - 1; i > 0; i--)
        {
            var j = (int)UnityEngine.Random.Range(0f, i);
            var temp = values[i];
            values[i] = values[j];
            values[j] = temp;
        }
    }

    private string ToUpper(string word)
    {
       return CultureInfo.CurrentCulture.TextInfo.ToUpper(word);
    }

    private async void AciertaPalabra()
    {
        AudioSource.PlayClipAtPoint(soundAcierta, Vector3.zero );
        tiempoPalabra.gameObject.SetActive(false);
        timeTotal += timeWord;
        if (timeMoneda > 0) { totalMonedas += 1; particlePeque2.Play(); monedas.text = totalMonedas.ToString(); animMonedas.SetBool("ok", true); }
        totalMarcador += 1;
        marcador.text = "Puntos: "+totalMarcador.ToString("D2");
        if (timeWord > 0)
        {
            sumaTiempo.text = "+" + ((int)timeWord).ToString();
            sumaTiempo.gameObject.SetActive(true);
        }
        particle.Play();
        
        desordenada.gameObject.SetActive (false);
        temaPista.gameObject.SetActive(false);
        palabra.text = resultado.text;
        //resultado.color = Color.green;
        noPuedeEscribir = true;
        Invoke("Elige", 2);
        await Task.Delay(TimeSpan.FromSeconds(2f));
        animMonedas.SetBool("ok", false);
    }
    public void PintaDesordenada() //Pintamos de un color cada letra que vayamos usando
    {
        var finalWord = "";
        char[] chari = resultado.text.ToCharArray();

        foreach (char s in desordenadaChar)
        {
            if (System.Array.Exists(chari, element => element == s))
            {
                finalWord += "<#FFFCB1>" + s + "</color>";
            }
            else
            {
                finalWord += s;
            }
        }
        desordenada.text = finalWord;
    }
    public void OnClickLetter(string letter)
    { if (!noPuedeEscribir)
        {
            

            var t = resultado.text;
            t += letter;
            resultado.text = ToUpper(t);

            PintaDesordenada();
            
            OnClickEnter();
        }
    }

    public void OnClickBack()
    {
        var t = resultado .text;
        resultado.color = colorAmarillo;
        resultado.GetComponent<Animator>().enabled = false;
        if (t.Length-pista.Length  >0)
        {
            
            t = t.Remove(t.Length - 1, 1);
            t = ToUpper(t);
            //print(t);
            resultado.text = t;
            // PintaDesordenada();
            if(ultimaLetraUsada.Count > 0)
            {
                ultimaLetraUsada[ultimaLetraUsada.Count-1].GetComponent<Animator>().SetBool("FadeOut", false);
                ultimaLetraUsada.RemoveAt(ultimaLetraUsada.Count-1);
            }
            
        }
        
    }

    public void OnClickEnter()
    {
        if(resultado.text == palabraActual )
        {
            AciertaPalabra();
            
        } else
        {
            if (resultado.text.Length == palabraActual.Length)
            {
                resultado.GetComponent<Animator>().enabled = true;
                resultado.color = Color.red;
            }
            else
            {
                resultado.color = colorAmarillo;
                resultado.GetComponent<Animator>().enabled = false;
            }
        }
    }
  

    public void OnClickPista()
    {
        if (!noPuedeEscribir)
        {
            if (totalMonedas > 0)
            {
                resultado.color = colorAmarillo;
                resultado.GetComponent<Animator>().enabled = false;
                AudioSource.PlayClipAtPoint(soundPista, Vector3.zero);
                pista = "";
                pistaInt += 1;
                totalMonedas -= 1;
                monedas.text = totalMonedas.ToString();
                char[] chars = palabraActual.ToCharArray();

                if (pistaInt < chars.Length)
                {
                    for (int i = 0; i < pistaInt; i++)
                    {
                        pista += chars[i];

                        if (pistaInt - i == 1)
                        {
                           
                            // print(chars[i] + "(clone)");
                            // mueve.letrasActual.RemoveAt(pistaInt);
                            for (int e = 0; e < mueve.letrasActual.Count; e++)
                            {
                                if (mueve.letrasActual[e].ToString().Remove(1, mueve.letrasActual[e].ToString().Length - 1) == chars[i] + "")
                                {
                                    mueve.letrasActual[e].GetComponent<Animator>().SetBool("FadeOut", true);
                                    particlePeque.transform.position = mueve.letrasActual[e].transform.position;
                                    particlePeque.Play();
                                    Destroy(mueve.letrasActual[e],2);
                                    mueve.letrasActual.RemoveAt(e); //debug
                                    
                                    print("FadeOut " + chars[i]);
                                    break;
                                }
                            }     
                        }


                    }
                    palabra.text = pista;
                    resultado.text = pista;
                    mueve.OnClickRemoveLetters();
                    //PintaDesordenada();
                    palabra.color = new Color(145, 145, 145, 255);
                }
                else
                {
                    for (int i = 0; i < pistaInt; i++)
                    {
                        pista += chars[i];

                        if (pistaInt - i == 1)
                        {
                           // print(chars[i] + "(clone)");
                            // mueve.letrasActual.RemoveAt(pistaInt);
                            for (int e = 0; e < mueve.letrasActual.Count; e++)
                            {
                                if (mueve.letrasActual[e].ToString().Remove(1, mueve.letrasActual[e].ToString().Length - 1) == chars[i] + "")
                                {
                                    mueve.letrasActual[e].GetComponent<Animator>().SetBool("FadeOut", true);
                                    Destroy(mueve.letrasActual[e],2);
                                    mueve.letrasActual.RemoveAt(e); //debug
                                    
                                    print("FadeOut " + chars[i]);
                                    break;
                                }
                            }
                        }
                       
                    }
                    palabra.text = pista;
                    resultado.text = pista;
                    //PintaDesordenada();
                    AciertaPalabra();
                }
            } else
            {
                Util.timeIsRunning = false;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                panelVideo.SetActive(true);
                noPuedeEscribir = true;
            }
        }
    }

    public void OnClickVerVideo()
    {
        admob.ShowAdd();        
        
    }

    public void videoVisto()
    {
        Util.videoVisto = false;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        totalMonedas += 10;
        animMonedas.SetBool("ok", true);
        monedas.text = totalMonedas.ToString();
        panelVideo.SetActive(false);
        // Util.timeIsRunning = true;
        noPuedeEscribir = false;
        particlePeque2.Play();
       // await Task.Delay(TimeSpan.FromSeconds(2f));
        animMonedas.SetBool("ok", false);
        
    }

    public void OnClickNoGracias(GameObject p)
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Util.timeIsRunning = true;
        p.SetActive(false);
        noPuedeEscribir = false;
        
    }
    
    public void OnClickOptions()
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Util.timeIsRunning = false;
        noPuedeEscribir = true;
        panelOptions.SetActive(true);
    }

    public void OnClickSalir()
    {
        Application.Quit();
    }

    
    

    
}
