using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    bool estaPulando;
    int click;

    public AudioClip clip;
    public GameObject moeda;

    public float distanciaPuloX = 80.0f;
    public List<GameObject> faixas;
    Vector3 localDoPulo;
    public float velocidade = 100;
    private float pontoCentralX;
    private float pontoCentralZ;

    public int indiceDaMaiorFaixa;

    public float incAlturaPulo = 5.0f;
    private float posicaoInicialY;

    private bool taMorto = false;

    public GameObject patoMesh;

    public GameObject panelStart;
    public GameObject panelOver;

	//public GameObject char1;
	//public GameObject char2;
	//public GameObject char3;

    // public GameObject cantoEsquerdo;
    // public GameObject cantoDireito;

    private bool taRodandoAMorte = false;
    bool gameEstaRodando = false;
    float compensadorPulo = -2.5f;

    public int score;
    public Text scoreText;

    public GameObject[] poolDePrefabsDeFaixas;

    private bool isJumpingCima;
    private bool isJumpingBaixo;
    private bool isJumpingDireita;
    private bool isJumpingEsquerda;

    private int criarMoeda;

    void Start()
    {
        panelOver.SetActive(false);
        estaPulando = false;
        click = 0;
        posicaoInicialY = this.transform.position.y;
        taRodandoAMorte = false;
        gameEstaRodando = false;
		SetCharacterSelecionado ();
    }

    void Update()
    {

        if (taMorto) // para impedir que ele se mova quando morto
            return;

        if (!gameEstaRodando)
            return;

        if (isJumpingCima)
        {
            if (this.transform.position.x > localDoPulo.x)
            {
                this.transform.position = new Vector3(transform.position.x - (velocidade * Time.deltaTime), transform.position.y, transform.position.z);
                if (this.transform.position.x > pontoCentralX)
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y + incAlturaPulo * Time.deltaTime, transform.position.z);
                }
                else
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y - incAlturaPulo * Time.deltaTime, transform.position.z);
                }
            }
            else
            {
                isJumpingCima = false;
                this.transform.position = new Vector3(this.transform.position.x, this.posicaoInicialY, this.transform.position.z);
            }
        }
        else if (isJumpingBaixo)
        {
            if (this.transform.position.x < localDoPulo.x)
            {
                this.transform.position = new Vector3(transform.position.x + (velocidade * Time.deltaTime), transform.position.y, transform.position.z);
                if (this.transform.position.x < pontoCentralX)
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y + incAlturaPulo * Time.deltaTime, transform.position.z);
                }
                else
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y - incAlturaPulo * Time.deltaTime, transform.position.z);
                }
            }
            else
            {
                isJumpingBaixo = false;
                this.transform.position = new Vector3(this.transform.position.x, this.posicaoInicialY, this.transform.position.z);
            }
        }
        else if (isJumpingDireita)
        {
            if (this.transform.position.z < localDoPulo.z)
            {
                this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (velocidade * Time.deltaTime));
                if (this.transform.position.z < pontoCentralZ)
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y + incAlturaPulo * Time.deltaTime, transform.position.z);
                }
                else
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y - incAlturaPulo * Time.deltaTime, transform.position.z);
                }
            }
            else
            {
                isJumpingDireita = false;
                this.transform.position = new Vector3(this.transform.position.x, this.posicaoInicialY, this.transform.position.z);
            }
        }
        else if (isJumpingEsquerda)
        {
            if (this.transform.position.z > localDoPulo.z)
            {
                this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (velocidade * Time.deltaTime));
                if (this.transform.position.z < pontoCentralZ)
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y + incAlturaPulo * Time.deltaTime, transform.position.z);
                }
                else
                {
                    this.transform.position = new Vector3(transform.position.x, transform.position.y - incAlturaPulo * Time.deltaTime, transform.position.z);
                }
            }
            else
            {
                isJumpingEsquerda = false;
                this.transform.position = new Vector3(this.transform.position.x, this.posicaoInicialY, this.transform.position.z);
            }
        }

        if (taRodandoAMorte)
        {
            UpdateTaRodandoAMorte();
        }
    }

    void Jump()
    {
        click += 1;
        GameObject faixa = faixas[click] as GameObject;

        localDoPulo = new Vector3(faixa.transform.position.x, transform.position.y, transform.position.z);
        pontoCentralX = localDoPulo.x + ((this.transform.position.x - localDoPulo.x) / 2);
		pontoCentralZ = localDoPulo.z + ((this.transform.position.z - localDoPulo.z) / 2);
        GerarNovaFaixa();
        float playerX = transform.position.x;

        /* if(click > indiceDaMaiorFaixa)
		{
			scoreText.text = "Score: " + score;
		} */
    }

    void GerarNovaFaixa()
    {

        int contPrefabsFaixas = poolDePrefabsDeFaixas.Length;
        int random = Random.Range(0, contPrefabsFaixas);

        GameObject item = poolDePrefabsDeFaixas[random] as GameObject;

        Transform itemChild = item.transform.GetChild(0) as Transform;
        float tamanhoItem = itemChild.gameObject.GetComponent<Renderer>().bounds.size.x;
        GameObject ultimaFaixa = faixas[faixas.Count - 1];

        GameObject novaFaixa = Instantiate(item, ultimaFaixa.transform.position, ultimaFaixa.transform.rotation) as GameObject;

        criarMoeda = Random.Range(0, 1);

        if (criarMoeda == 0)
        {

			//Corrigir esse erro
			Vector3 novaPos = new Vector3(novaFaixa.transform.position.x, ultimaFaixa.transform.position.z + 25f, Random.Range(-100, tamanhoItem));
            GameObject coin = Instantiate(moeda, novaPos, moeda.transform.rotation) as GameObject;
        }

        novaFaixa.transform.position = new Vector3(novaFaixa.transform.position.x - tamanhoItem, novaFaixa.transform.position.y, novaFaixa.transform.position.z);
        faixas.Add(novaFaixa);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
            YouDied();

        if (other.gameObject.tag == "Coin")
        {
            score += 1;
            scoreText.text = "Score: " + score;
            
            AudioSource meuclip = GetComponent<AudioSource>();
            meuclip.PlayOneShot(clip);
            Destroy(other.gameObject);
        }
    }

    void YouDied()
    {
        taRodandoAMorte = true;
    }
    // ------------------------------------------------------------------------------------------------
    void UpdateTaRodandoAMorte()
    {
        if (patoMesh.transform.localScale.z > 0.1)
        {
            patoMesh.transform.localScale -= new Vector3(0f, 0f, 0.1f);
        }
        else
        {
            gameEstaRodando = false;
            taRodandoAMorte = false;
        }

        if (patoMesh.transform.rotation.eulerAngles.x == 0 || patoMesh.transform.rotation.eulerAngles.x > 270)
        {
            patoMesh.transform.Rotate(-10.0f, 0, 0);
        }

        panelOver.SetActive(true);
    }

    // --------------------------------------------------------------------------------------------------
    void SwipeUp()
    {
        if (!isJumpingCima)
        {
            isJumpingCima = true;
            JumpCima();
        }
    }

    void SwipeDown()
    {
        if (!isJumpingBaixo)
        {
            isJumpingBaixo = true;
            JumpBaixo();
        }
    }

    void SwipeRight()
    {
        if (!isJumpingDireita)
        {
            isJumpingDireita = true;
            JumpDireita();
        }
    }

    void SwipeLeft()
    {
        if (!isJumpingEsquerda)
        {
            isJumpingEsquerda = true;
            JumpEsquerda();
        }
    }
    // ------------------------------------------------------------------------------------------------------------

    void JumpCima()
    {
        click++;
        GameObject faixa = faixas[click] as GameObject;
        localDoPulo = new Vector3(faixa.transform.position.x, transform.position.y, transform.position.z);
        pontoCentralX = localDoPulo.x + ((this.transform.position.x - localDoPulo.x) / 2);
        patoMesh.transform.localEulerAngles = new Vector3(0, 0, 0); // garante que o ângulo esteja sempre para frente 
        GerarNovaFaixa();
        float playerX = transform.position.x;
    }

    void JumpBaixo()
    {
        click -= 1;

        if (click < 0)
            click = 0;
        else
        {
            GameObject faixa = faixas[click] as GameObject;

            localDoPulo = new Vector3(faixa.transform.position.x, transform.position.y, transform.position.z);
            pontoCentralX = localDoPulo.x + ((this.transform.position.x - localDoPulo.x) / 2);
            patoMesh.transform.localEulerAngles = new Vector3(0, 180, 0); // garante que o ângulo esteja sempre para frente 
            GerarNovaFaixa();
            float playerX = transform.position.x;
        }

    }

    void JumpEsquerda()
    {
		localDoPulo = new Vector3(transform.position.x, transform.position.y, (transform.position.z - distanciaPuloX));
        pontoCentralZ = localDoPulo.z - ((this.transform.position.z - localDoPulo.z) / 2);
        patoMesh.transform.localEulerAngles = new Vector3(0, -90, 0); // garante que o ângulo esteja sempre para frente 
    }

    void JumpDireita()
    {
		localDoPulo = new Vector3(transform.position.x, transform.position.y, (transform.position.z + distanciaPuloX));
        pontoCentralZ = localDoPulo.z + ((this.transform.position.z + localDoPulo.z) / 2);
        patoMesh.transform.localEulerAngles = new Vector3(0, +90, 0); // garante que o ângulo esteja sempre para frente 
    }

    public void BotaoStartPressionado()
    {
        gameEstaRodando = true;
        panelStart.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene("cena porco");
        gameEstaRodando = true;
    }

	public void CharacterSelectionPato(){
		Debug.Log ("Selecionado Pato 1");
		PlayerPrefs.SetInt ("character", 0);
		SceneManager.LoadScene ("cena porco");
	}

	public void CharacterSelectionGalinha2(){
		Debug.Log ("Selecionado Pato 2");
		PlayerPrefs.SetInt ("character", 1);
		SceneManager.LoadScene ("cena porco");
	}

	public void CharacterSelectionGalinha3(){
		Debug.Log ("Selecionado Pato 3");
		PlayerPrefs.SetInt ("character", 2);

	}

	void SetCharacterSelecionado(){
		int selected = PlayerPrefs.GetInt ("character");

		switch (selected) {
		case 0:
			// char1.setActive(true);
			//char2.setActive(false);
			//char3.setActive(false);
			break;
		case 1:
			// char1.setActive(false);
			//char2.setActive(true);
			//char3.setActive(false);
			break;
		case 2:
			// char1.setActive(false);
			//char2.setActive(false);
			//char3.setActive(true);
			break;
		}
	}

}
