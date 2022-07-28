using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
//using UnityEditor;

/****
 * Fait par Farouk
 * le 2022-04-11
 * Update:2022-04-12
 *        2022-04-13
 *        2022-04-20
 *        2022-04-22
 *        
 *        Flavien
 * Update: 2022/05/10
 *         2022/05/23
 *         2022/05/25
 *         2022/05/27
 * */
public class Tg_Trigger : MonoBehaviour
{
    //la partie fonction
    public SO_DataRiley dataRiley;
    [Header("Qui ?")]
    [Tooltip("Ajoute la Main Camera à la liste d'objets pouvant activer le trigger")]
    public bool hadMCameraToList = false;
    [Tooltip("Ajoute Riley à la liste d'objets pouvant activer le trigger")]
    public bool hadPlayerToList = false;
    [Tooltip("Ajoute les objets suivant à la liste d'objets pouvant activer le trigger")]
    public List<GameObject> inGameGameObjectList = new List<GameObject>();

    [Header("Quand ? Quoi ?")]
    [Tooltip("Execute la fonction choisi de cette ingredient lorsque l'objet rentre dans le trigger")]
    public UnityEvent OnEnter = new UnityEvent();
    [HideInInspector]
    public UnityEvent<int> OnActive;
    [Tooltip("Execute la fonction choisi de cette ingredient tant que l'objet est dans le trigger")]
    public UnityEvent OnStay = new UnityEvent();
    [Tooltip("Execute la fonction choisi de cette ingredient lorsque l'objet quitte le trigger")]
    public UnityEvent OnExit = new UnityEvent();
    [Tooltip("Pour le OnStay seulement, délai en seconde avant l'activation des événement")]
    [HideInInspector]
    public UnityEvent<int> OnSetup;
    [HideInInspector]
    public UnityEvent<int> OnDeactivate;
    public float delay;
    private Coroutine checkTrigger;
    private BoxCollider bc;
    private SphereCollider sc;
    private GameObject cameraGo;
    private GameObject playerGo;
    [HideInInspector]
    public Riley_Main rileyMain;


    //la partie forme
    private Vector3 center = new Vector3();
    public int usage;
    public bool unlimited = true;
    public Color drawColor = new Vector4(0.68f, 0.2f, 0.2f, 0.7f);
    public Color wireColor = new Vector4(0.68f, 0.10f, 0.10f, 0.8f);
    public bool isUsingSize = true;
    public Vector3 cubeCenter = new Vector3(0, 0, 0);
    public Vector3 cubeSize = new Vector3(1, 1, 1);
    public float sphereSize = 1f;
    //private Matrix4x4 cubeTransform;
    //private Matrix4x4 oldGizmosMatrix;
    public enum viewMode { full, Wireframe, none };
    public viewMode view;
    public SO_Event events;

    [SerializeField]
    public int guid; //à utiliser pour le trigger loader
    void Start()
    {
        OnSetup = new UnityEvent<int>();
        OnDeactivate = new UnityEvent<int>();
        OnActive = new UnityEvent<int>();
        UpdateCollider();
        if (hadMCameraToList)
        {
            cameraGo = Camera.main.gameObject;
            inGameGameObjectList.Add(cameraGo);
        }
        if (hadPlayerToList)
        {
            playerGo = dataRiley.rileyMain.gameObject.transform.Find("TriggerCollider").gameObject;//riley n'était plus sur la meme scene.
            rileyMain = dataRiley.rileyMain;
            inGameGameObjectList.Add(playerGo);
        }

        
    }
	#region forme
    private void OnValidate() //Permet d'update la taille du collider lorsqu'il y a un changement.
    {
        if (isUsingSize)
        {
            if (bc != null)
            {
                bc.size = cubeSize;
                bc.center = cubeCenter;
            }
            if (sc != null)
            {
                sc.radius = sphereSize;
            }
        }
        else
        {
            if (bc != null)
            {
                bc.size = transform.localScale;
            }
            /*if (sc != null)
            {
                sc.radius = transform.localScale.x;
            }*/
        }
    }
    //cette partie est le forme de l'ingredient dans l'editeur
    //inspiree par le DisplayRange code de Fred
    private void OnDrawGizmos()
    {
        if (bc != null)
        {
            center = transform.position;

            if (view == viewMode.full)
            {
                Gizmos.color = drawColor;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(bc.center, bc.size);
            }
            if (view == viewMode.Wireframe)
            {
                Gizmos.color = wireColor;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(bc.center, bc.size);
            }
        }
        if (sc != null)
        {
            if (view == viewMode.full)
            {
                center = transform.position;
                Gizmos.color = drawColor;
                if (isUsingSize)
                {
                    Gizmos.DrawSphere(center + cubeCenter, sphereSize);
                }
                else
                {
                    Gizmos.DrawSphere(center + cubeCenter, transform.localScale.x);
                }
            }
            if (view == viewMode.Wireframe)
            {
                center = transform.position;
                Gizmos.color = wireColor;
                if (isUsingSize)
                {
                    Gizmos.DrawWireSphere(center + cubeCenter, sphereSize);
                }
                else
                {
                    Gizmos.DrawWireSphere(center + cubeCenter, transform.localScale.x);
                }
            }
        }
    }
#endregion
	
	#region fonction
	//fond du code on regarde ce qui entre dans le trigger, si c'est dans la liste on appelle un unity event
    private void OnTriggerEnter(Collider other)
    {
        if (inGameGameObjectList != null)                                                             ///Code de FRED ajoute le 13-04-2022
        {
            for (int i = inGameGameObjectList.Count - 1; i >= 0; i--) //list des objets qui déclenchent le trigger
            {
                if (inGameGameObjectList[i] != null)
                {
                    if (inGameGameObjectList[i] == other.gameObject)
                    {
                        if (!unlimited) //si le trigger se détruit après un certain nombre d'activation
                        {
                            if (usage > 0)
                            {
                                usage--;
                            }
                            else { this.gameObject.SetActive(false); };
                        }

                        //Debug.Log(this + "appelle un event");
                        OnEnter.Invoke();//unity event
                        if(events)
                        events.Active(guid);//utilisé sur le scriptable object event
                        if (this.gameObject.name== "Ing_TriggerBox Se soigner")
                        dataRiley.rileyMain.trig = this;
                        
                    }
                }
            }
        }
    }


	private void OnTriggerExit(Collider other)
    {
        if (inGameGameObjectList != null)                                                             ///Code de FRED ajout� le 13-04-2022
        {
            for (int i = inGameGameObjectList.Count - 1; i >= 0; i--)
            {
                if (inGameGameObjectList[i] != null)
                {
                    if (inGameGameObjectList[i] == other.gameObject)
                        OnExit.Invoke();//unity event
                    if (checkTrigger != null)
                    {
                        StopAllCoroutines(); //si l avatar quitte le on stay la coroutine s'arrete;
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (inGameGameObjectList != null)                                                             ///Code de FRED ajout� le 13-04-2022
        {
            for (int i = inGameGameObjectList.Count - 1; i >= 0; i--)
            {
                if (inGameGameObjectList[i] != null)
                {
                    if (inGameGameObjectList[i] == other.gameObject)
                        checkTrigger = StartCoroutine("OnStayTrigger"); //on utilise une coroutine pour attendre le temps necessaire afin que l'event s'enclenche
                }
            }
        }
    }
	#endregion
    private IEnumerator OnStayTrigger()
    {
        yield return new WaitForSeconds(delay);
        OnStay.Invoke();//unity event
    }
    public void UpdateCollider()
    {
        bc = this.GetComponent<BoxCollider>();
        sc = this.GetComponent<SphereCollider>();
    }

    public void GetGuid()
    {
        guid = Guid.NewGuid().GetHashCode();
    }

    private void OnEnable()
    {
        StartCoroutine(Enabling());
       
    }

    private IEnumerator Enabling()//utile pour trigger load
    {
        yield return new WaitForSeconds(1f);
        if (events)
        events.Setup(guid);
    }

    private void OnDisable()//utile pour trigger load
    {
        if (events)
        events.Deactivate(guid);
        //Debug.Log(this + "successfully failed");
    }
}