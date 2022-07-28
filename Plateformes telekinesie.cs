using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Cinemachine;



//using UnityEditor;

/***
 * Fait par Farouk:le 29/04/2022
 * update: le 30/04/2022
 *         le 02/05/2022
 *         le 07/06/2022
 * */

public class Ing_PlateformeFloor : MonoBehaviour,ITelekinesis,IToggleable
{
	#region base
    private Transform tr;
    public GameObject railStart;
    public GameObject railEnd;
    private Transform start;
    private Transform end;
	private Transform[] tempTransforms;
	#endregion
    
	#region plateforme en mouvement
	private Transform inactive;
	public bool telekinesis;
    private bool telekinesisActive;
    private bool needcam; 
    public SO_DataRiley dataRiley;
    private Transform riley;
    //private float temp;
    //private int sign = 0;
    //private Vector3 inactiveRot = new Vector3();
    private float d;
    private float t;
    private Vector3 normal = new Vector3();
    private Vector3 newPos;
    private Vector3 finish;
    private Vector3 begin;
    private float speed;
    private Vector3 orientation;
    private Vector3 rail;
    private Vector3 rilPos;
    //private float autoSpeed = 0;
    private float curSpeed = 0;
    private float sensSpeed = -1;
    private bool isActive;
    private Transform tempTr;
    private Vector3 top;
    private Transform aim;
    private Vector3 camup;
    private bool gotPlateforme;
	#endregion
	
    public bool isPlateformCeiling = false;

    [SerializeField]
    [Range(0, 1)]
    private float posOnRail;
    private Transform tempCont;
    private Riley_Main rileyMain;
    private Camera_center cam;
    private Coroutine yBouge;

    public enum Direction
    {
        AxeX=0,
        AxeXZ=-45,
        AxeZ=-90,
        AxemMinXZ=-135,
        AxeMinX=-180,
        AxeMinXMinZ=-225,
        AxeMinz=-270,
        AxeXminZ=-315


    }

    public Direction sens = new Direction();
    private CharacterController tempChar;
    private Vector3 chilpos;
    private Vector3 MemPos;
    public Transform center;
    private Vector3 platform;
    [HideInInspector]
    public Vector3 gizmo;
    private List <Transform> chilTrans = new List<Transform>();
    private int j;
    private Tg_Trigger Aura;
    private Ing_Cursedzone Corruption;
    private Riley_Main Riley;
    private Telekinesis tel;
    private Riley_ST_Aim rilAim;

    private float normTime;
    private float k;
    private float time = 0;
    
    private Vector3 startPos;

    [Header("Vitesse")]
    public bool defaultSpeed=true;
    public float speedValue;
    
    #region son
    [HideInInspector]
    public float timeSonStart;
    [HideInInspector]
    public bool moving = false;
    private Coroutine soundend;
    [HideInInspector]
    public bool ending;
    [HideInInspector]
    public float timeSonEnd;
    [HideInInspector]
    public bool starting;

    [HideInInspector]
    public float timeSonStartTel;
    [HideInInspector]
    public float movingTel = 0;
    private Coroutine soundendTel;
    [HideInInspector]
    public bool endingTel;
    [HideInInspector]
    public float timeSonEndTel;
    [HideInInspector]
    public bool startingTel;
	#endregion
    #region type de collider
	private GameObject tworil;
    private Riley_Melee ficli;
    private Riley_Shield shicli;
    private int rilcount;
    private CobSR_FollowPoint cob;
    private Ing_Totem totem;
    private MeshCollider mc;
    private BoxCollider bc;
	#endregion
    public bool isPlatform4m;
    private bool rilon; //On verifie si Riley est sur la plateforme.
    private Vector3 back2Aim;
    private bool recadrageNeeded;
    Vector3 camdist;
    Vector3 oricam;
    private CinemachineVirtualCamera camLock;
    //private Transform dummytarget;

    // Start is called before the first frame update
    void Start()
    {
        /*gotPlateforme = (railStart != null && railEnd != null);
        tr = this.transform;
        platform = center.position - tr.position;
        if (gotPlateforme)
        {
            tempTransforms = railStart.GetComponentsInChildren<Transform>();

            start = tempTransforms[1];
            tempTransforms = railEnd.GetComponentsInChildren<Transform>();

            end = tempTransforms[1];
            telekinesisActive = false;
            begin = start.position;

            finish = end.position;
            speed = 0.5f;
            rail = finish - begin;
            tr.position = begin + posOnRail * rail;
            gizmo = tr.position + platform;
            tr.Rotate(new Vector3(tr.rotation.eulerAngles.x, ((float)sens), tr.rotation.eulerAngles.z), Space.World);
            aim = Camera.main.transform;

            if (isPlateformCeiling)
            {
                tr.position = new Vector3(tr.position.x, tr.position.y - 3.55f, tr.position.z);
                foreach (Transform child in tr)
                {
                    child.position = new Vector3(tr.position.x, tr.position.y + 3.55f, tr.position.z);
                }
                begin = new Vector3(begin.x, begin.y - 3.55f, begin.z);
                finish = new Vector3(finish.x, finish.y - 3.55f, finish.z);
            }

        }*/
        AtBegin();

        this.TryGetComponent<Camera_center>(out cam);
        //dummytarget = gameObject.AddComponent<Transform>();
        back2Aim = dataRiley.rileyMain.camAim.transform.localPosition - dataRiley.rileyMain.camMain.transform.localPosition;
        //dataRiley.rileyMain.camAim.AddCinemachineComponent<CinemachineTransposer>();
        oricam = dataRiley.rileyMain.camAim.transform.localPosition;
        GameObject.Find("CamLock").TryGetComponent<CinemachineVirtualCamera>(out camLock);
           
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (dataRiley.rileyMain.chosenOne)
        //{
        //    Debug.Log(dataRiley.rileyMain.chosenOne + " is Riley target");
        //}
        if(rilon)
        {
            if (dataRiley.rileyMain.chosenOne==this.gameObject)
            {
                Cancel();
                dataRiley.rileyMain.ChangeAnimation(dataRiley.rileyMain.isPlateformHold, false);
            }
        }
        curSpeed = time / (k * normTime); 
		//permet un déplacement uniforme de la plateforme sur le rail quand elle n'est pas  télékinesiable
        //Debug.Log(curSpeed);
        MemPos = tr.position;
        if (gotPlateforme)
        {
            riley = dataRiley.myPlayerTr;
            rilPos = riley.position;
            if (telekinesis)
                Move();
            else
                Auto();
            speed = 0.5f;
            
        }
        
    }


#region pas de telekinesie
//si la plateforme n'est pas telekinesiable
    private void Auto()
    {
        if (gotPlateforme)
        {
            TryGetComponent<Telekinesis>(out tel);
			
			//on désactive le réticule de télékinésie pour ne pas créer de faux appels;
            if (tel)
            {
                if (tel.reticle.activeInHierarchy == true)
                { tel.reticle.SetActive(false); }
            }
			//si la platefrome va vers son end
            if (sensSpeed > 0)
            {
                if ((tr.position - begin).sqrMagnitude <= rail.sqrMagnitude) 
					//si la distance start/plateforme<= la distance start/end 
				//(on s'assure que la plateforme ne dépasse pas son end)
                    tr.position = Vector3.Lerp(startPos, finish, curSpeed);
                else //si la plateforme atteint ou dépasse end la plateforme ne bouge plus;
                {
                    if (moving == true)
                    {
                        moving = false;
                        StopCoroutine(StartMove());//pour le son
                        if (soundend == null)
                            soundend = StartCoroutine(Endingsound());//pour le son
                    }
                }
				
				//si des objets sont sur la plateforme, on s'assure qu'ils bougent avec
                if (chilTrans.Count > 0)
                {
                    for (int i = 0; i < chilTrans.Count; i++)
                    {
                        if (chilTrans[i] == null)
                        {
                            NullCheck(i);
                        }
                        else
                        if (Riley)
                        { tempChar.Move(tr.position - MemPos); }
                        else
                        { chilTrans[i].Translate((tr.position - MemPos), Space.World); }
                    }
                }
               
            }
			//si la platefrome va vers son start
            else if (sensSpeed < 0)
            {
                if ((tr.position - finish).sqrMagnitude <= rail.sqrMagnitude)
					//si la distance end/plateforme<= la distance start/end 
				//(on s'assure que la plateforme ne dépasse pas son start)
                    tr.position = Vector3.Lerp(startPos, begin, curSpeed);
                else
                {
                    if (moving == true)
                    {
                        moving = false;
                        StopCoroutine(StartMove());
                        if (soundend == null)
                            soundend = StartCoroutine(Endingsound());
                    }
                }
				//si des objets sont sur la plateforme, on s'assure qu'ils bougent avec
                if (chilTrans.Count > 0)
                {
                    for(int i=0; i<chilTrans.Count; i++)
                    {
                        if (chilTrans[i] == null)
                        {
                            NullCheck(i);
                        }
                        else
                        if (Riley)
                        { tempChar.Move(tr.position - MemPos); }
                        else
                        { chilTrans[i].Translate((tr.position - MemPos), Space.World); }
                    }
                }
            }
            time += Time.deltaTime;
			//permet de déplacer la plateforme sur rail en tenant compte du framerate
        }

       
    }
#endregion
//pour le son
    private IEnumerator Endingsound()
    {
        ending = true;
        yield return new WaitForSeconds(timeSonEnd);
        ending = false;
    }
    private void NullCheck(int rank) //on evite les vides dans les listes
    {
        chilTrans.RemoveAt(rank);
    }
#region Telekinesie plateforme
    private void Move()
    {
        if (gotPlateforme)
        {
            if (telekinesisActive == false)
            {
                //inactive = riley;
                //inactiveRot = inactive.rotation * Vector3.forward;
                if (recadrageNeeded) //on realigne la camera
                {
                    //camLock.Priority = 7;
                    var POV = dataRiley.rileyMain.camAim.GetCinemachineComponent<CinemachinePOV>();
                    POV.m_HorizontalAxis.m_MaxSpeed = 1f;
                    POV.m_VerticalAxis.m_MaxSpeed = 0.8f;
                   
                    recadrageNeeded = false;
                }
                if (movingTel >0) //pour le son
                {
                    movingTel = 0;
                    StopCoroutine(StartMoveTel());
                    if (soundend == null)
                        soundendTel = StartCoroutine(EndingsoundTel());
                }
            }
            else
            {
                camdist = dataRiley.rileyMain.camAim.transform.position - tr.position;
				//vecteur 3 camera to riley
				
				
                

                //var newCamRot = dataRiley.rileyMain.transform.rotation;
                //dummytarget.position = (tr.position - dataRiley.rileyMain.camAim.transform.position)/10;



                //m_CameraDistance = Mathf.Sqrt((camdist).sqrMagnitude);

                //camLock.Priority = 20;
                #region stay on screen
				//On s'assure que la plateforme reste à l'écran.
				if (Camera.main.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(tr.position)).x <= 0 || 
                 Camera.main.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(tr.position)).x >= Screen.width)
                {
                    var POV = dataRiley.rileyMain.camAim.GetCinemachineComponent<CinemachinePOV>();
                    POV.m_HorizontalAxis.m_MaxSpeed = 0f;
                    if (Camera.main.WorldToViewportPoint(tr.position).x <= 0)
                        POV.m_HorizontalAxis.Value -= 0.1f;
                    else
                        POV.m_HorizontalAxis.Value += 0.1f;
                }
                else if (Camera.main.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(tr.position)).y <= 0||
                    Camera.main.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(tr.position)).y >= Screen.height)
                {
                    var POV = dataRiley.rileyMain.camAim.GetCinemachineComponent<CinemachinePOV>();
                    POV.m_VerticalAxis.m_MaxSpeed = 0f;
                    if (Camera.main.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(tr.position)).y <= Screen.height)
                        POV.m_VerticalAxis.Value += 0.1f;  
                            else
                        POV.m_VerticalAxis.Value -= 0.1f;
                }
                else
                { 
                    var POV = dataRiley.rileyMain.camAim.GetCinemachineComponent<CinemachinePOV>();
                    POV.m_HorizontalAxis.m_MaxSpeed = 1f;
                    POV.m_VerticalAxis.m_MaxSpeed = 0.8f;
                }
            
                recadrageNeeded = true;
				#endregion;
                //dataRiley.rileyMain.camAim.transform.SetPositionAndRotation(dataRiley.rileyMain.camAim.transform.position, dataRiley.rileyMain.camAim.transform.rotation);
                if (movingTel >0)
                { movingTel = Mathf.Sqrt((MemPos - tr.position).sqrMagnitude) / Mathf.Sqrt((rail).sqrMagnitude);
                    if (movingTel<0.1f)
                    { movingTel = 0.1f; }
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(StartMove());
                }
                orientation = riley.transform.rotation * Vector3.forward;
                normal = riley.transform.rotation * Vector3.right;
                top = riley.rotation * Vector3.up;
                if (!aim)
                    aim = Camera.main.transform;

                camup = Camera.main.transform.rotation * Vector3.up;

                if ((!needcam)&&(cam))
                {
                    cam.CamCen(tr);
                    needcam = true;
                }

				/*
				Pour permettre un contrôle de la position de la plateforme sur le rail, il existe deux situations.
				
				A) La droite dirigée par le vecteur directeur rail n'appartient pas au plan P formé par le z et le y local de Riley.
				B) La droite dirigée par le vecteur directeur rail appartient  au plan P formé par le z et le y local de Riley.
				*/
                if (Mathf.Abs(Vector3.Dot(top.normalized, rail.normalized)) < 0.999f) //quand le rail n'est pas vertical
                {
					
                    if (Mathf.Abs(Vector3.Dot(normal.normalized, rail.normalized)) > 0)
                    {
						
						/*A)
						Pour faire en sorte que la plateforme se deplace sur le rail en fonction de l'endroit où riley vise, il suffit de trouver les coordonnes (x,y,z) du vecteur newPos.
                newPos se situe à l'intersection du plan P formé par le y local et le z local de Riley(1) et la droite D ayant pour point begin et de vecteur directeur rail(2) 
                
                (1)Ce plan P a pour equation  aX+bY+cZ+d=0 où a,b et c sont les composantes du vecteur normal (a,b,c) et (X,Y,Z) les coordonnées de rilPos.
                Comme le x local de riley est perpendiculaire au y local et z local, il est normal du plan mentionné.
                
                Riley.position  appartient au plan P, une équation de P est:
                normal.x*riley.position.x + normal.y*(riley.position.y+1.6f) + normal.z*riley.position.z + d = 0
                on isole d
                
                (2)La droite D a pour equation paramètrique
                    newPos.x= begin.x + t(rail.x)
                    newPos.y= begin.y + t(rail.y)
                    newPos.z= begin.z + t(rail.z)
    
                comme newPos est à l'intersection du plan P, une equation  de P est normal.x*newPos.x + normal.y*newPos.y + normal.z*newPos.z + d = 0
    
                on isole t;
    
                (3)On peut  maintenant calculer le vecteur Position newPos en utilisant l'equation paramètrique de D.
                 */

                        d = -(normal.x * riley.position.x + normal.y * (riley.position.y+1.6f) + normal.z * riley.position.z);
                        t = -(normal.x * begin.x + normal.y * begin.y + normal.z * begin.z + d)
                            / (normal.x * (finish - begin).x + normal.y * (finish - begin).y +
                               normal.z * (finish - begin).z);
                        newPos = new Vector3(begin.x + (finish - begin).x * t, begin.y + (finish - begin).y * t,
                            begin.z + (finish - begin).z * t);
                    }

                    

                    else if (Mathf.Abs(Vector3.Dot(orientation.normalized, rail.normalized)) < 1f)
                    {
						/* B)
					Exception quand D appartient à P.
                     newPos se trouve à l'intersection de  P2 formé par le x local et le z local de la camera(1) et la droite D ayant pour point begin et de vecteur directeur rail(2) 
                
                    (1)Ce plan P2 a pour equation  aX+bY+cZ+d=0 où a,b et c sont les composantes du vecteur normal (a,b,c) et (X,Y,Z) les coordonnées de camup.
                Comme le vecteur camup  est perpendiculaire au x local et z local, il est normal du plan mentionné.
				
				Riley.position appartient au  plan P2, une equation de P2 est.
				(1-camup.x)*riley.position.x + (1-camup.y)*(riley.position.y+1.6f) + (1-camup.z)*riley.position.z + d = 0
                On utilise 1- camup, car l'angle de la camera va en retrecissant à mesure que la camera regarde vers le haut.
				on isole d    
                    (2)La droite D a pour equation paramètrique
                    newPos.x= begin.x + t(rail.x)
                    newPos.y= begin.y + t(rail.y)
                    newPos.z= begin.z + t(rail.z)
    
                comme newPos est à l'intersection du plan P2, une equation  de P2 est (1-camup.x)*newPos.x + (1-camup.y)*newPos.y + (1-camup.z*newPos.z) + d = 0
    
                on isole t;
				
				(3)On peut  maintenant calculer le vecteur Position newPos en utilisant l'equation paramètrique de D.
                 */

                        d = -((1 - camup.x) * riley.position.x + (1 - camup.y) * (riley.position.y + 1.6f) +
                              (1 - camup.z) * riley.position.z);
                        t = -((1 - camup.x) * begin.x + (1 - camup.y) * begin.y + (1 - camup.z) * begin.z + d)
                            / ((1 - camup.x) * (finish - begin).x + (1 - camup.y) * (finish - begin).y +
                               (1 - camup.z) * (finish - begin).z);
                        newPos = new Vector3(begin.x + (finish - begin).x * t, begin.y + (finish - begin).y * t,
                            begin.z + (finish - begin).z * t);
                    }
                }
                else //quand le rail est vertical
                {
                    d = -(camup.x * riley.position.x + camup.y * (riley.position.y + 1.6f) + camup.z * riley.position.z);
                    t = -(camup.x * begin.x + camup.y * begin.y + camup.z * begin.z + d)
                        / (camup.x * (finish - begin).x + camup.y * (finish - begin).y + camup.z * (finish - begin).z);
                    newPos = new Vector3(begin.x + (finish - begin).x * t, begin.y + (finish - begin).y * t,
                        begin.z + (finish - begin).z * t);
                }


                
                if (Vector3.Dot((begin - tr.position), (newPos - tr.position)) >= 0 &&
                    (begin - tr.position).sqrMagnitude >= 0.2f)//condition de déplacement de la plateforme si elle va vers end
                {
                    if ((newPos - tr.position).sqrMagnitude > (begin - tr.position).sqrMagnitude)//si la plateforme atteint end.
                        speed = 0;
                    tr.position = Vector3.Lerp(tr.position, newPos, speed);
                    
                    if (chilTrans.Count> 0) //si un objet se trouve sur la plateforme
                    {
                        for (int i = 0; i < chilTrans.Count; i++)
                        {
                            if (chilTrans[i] == null)
                            {
                                NullCheck(i);
                            }
                            else
                            if (Riley)
                            { tempChar.Move(tr.position - MemPos); }
                            else
                            { chilTrans[i].Translate((tr.position - MemPos), Space.World); }
                            
                        }
                    }
                }
                else if (Vector3.Dot((finish - tr.position), (newPos - tr.position)) >= 0 &&///condition de déplacement de la plateforme si elle va vers begin
                         (finish - tr.position).sqrMagnitude >= 0.2f)
                {
                    if ((newPos - tr.position).sqrMagnitude > (finish - tr.position).sqrMagnitude)///si la plateforme atteint begin.
                        speed = 0;
                    tr.position = Vector3.Lerp(tr.position, newPos, speed);
                    if (chilTrans.Count > 0) ////si un objet se trouve sur la plateforme
                    {
                        for (int i = 0; i < chilTrans.Count; i++)
                        {
                            if (chilTrans[i] == null)
                            {
                                NullCheck(i);
                            }
                            else
                            if (Riley)
                            { tempChar.Move(tr.position - MemPos); }
                            else
                            { chilTrans[i].Translate((tr.position - MemPos), Space.World); }
                            
                        }
                    }
                }

                

                
                
            }
           
        }
    }
#endregion
    private IEnumerator StartMoveTel()
    {
        startingTel = true;
        yield return new WaitForSeconds(timeSonStartTel);
        startingTel = false;
        movingTel = 0.1f;
    }

    private IEnumerator EndingsoundTel()
    {
        endingTel = true;
        yield return new WaitForSeconds(timeSonEndTel);
        endingTel = false;
    }
    public void Shoot()
    {
        telekinesisActive = false;
        yBouge = null;
    }

    public void Telekinesis()
    {
        if(yBouge==null)

        yBouge=StartCoroutine(OkGo());
    }

    private IEnumerator OkGo()
    {
        yield return new WaitForEndOfFrame();
        telekinesisActive = true;
    }

    public void Cancel()
    {
        telekinesisActive = false;
        needcam = true;
       
    }

    public void Toggle()
    {
        time = 0;

        startPos = tr.position;
        sensSpeed = -sensSpeed;
        if (sensSpeed > 0)
			//k est le rapport distance  plateforme extremité/ longueur de rail;
            k = Mathf.Sqrt((startPos - finish).sqrMagnitude) / Mathf.Sqrt(rail.sqrMagnitude);
        else
            k = Mathf.Sqrt((startPos - begin).sqrMagnitude) / Mathf.Sqrt(rail.sqrMagnitude);

        if (moving == true)
        { return; }
        else
        {
            StopAllCoroutines();
            StartCoroutine(StartMove());
        }
    }

    private IEnumerator StartMove()
    {
        starting = true;
        yield return new WaitForSeconds(timeSonStart);
        starting = false;
        moving = true;
    }
    private void OnTriggerEnter(Collider other) //tri des objets autorisés à devenir enfants de la plateforme
    {
        if (gotPlateforme)
        {
            other.TryGetComponent<Tg_Trigger>(out Aura);
            other.TryGetComponent<Ing_Cursedzone>(out Corruption);
            other.TryGetComponent<Riley_Main>(out Riley);
            other.TryGetComponent<Riley_Melee>(out ficli);
            totem=other.gameObject.GetComponentInChildren<Ing_Totem>();
            
            if  (other.gameObject.TryGetComponent<Transform>(out tempCont)&&!Aura&&!Corruption)
            {

                if ((rilcount == 0) && (Riley))//on s'assure que Riley ne compte pas plusieurs fois
                {
                    other.GetComponentInChildren<CobSR_FollowPoint>();
                    shicli = other.GetComponentInParent<Riley_Shield>();
                    if (!ficli && !shicli)
                    {
                        telekinesisActive = false;
                        TryGetComponent<Telekinesis>(out tel);
                        tempChar = Riley.gameObject.GetComponent<CharacterController>();
                        chilpos = tempCont.transform.position;
                        
                        StopCoroutine(NoCheat());
                        if (telekinesis)
                            rilon=true;

                        chilTrans.Add(tempCont);
                        rilcount++;
                        int index = chilTrans.IndexOf(tempCont);
                        //Debug.Log("Nombre enfants en entrant" + chilTrans.Count + "/n" + chilTrans[index].gameObject.name);
                    }
                }
                else if ((rilcount != 0) && (Riley))
                    return;
                else if ((!Riley) && (other.gameObject.layer == 11))
                {
                    chilTrans.Add(tempCont);
                    int index = chilTrans.IndexOf(tempCont);
                        //Debug.Log("Nombre enfants en entrant" + chilTrans.Count + "/n" + chilTrans[index].gameObject.name);
                }
                else if ((!Riley) && (totem))
                {
                    chilTrans.Add(tempCont);
                    int index = chilTrans.IndexOf(tempCont);
                    //Debug.Log("Nombre enfants en entrant" + chilTrans.Count + "/n" + chilTrans[index].gameObject.name);
                }

            } 
        }


            
        }



    private void OnTriggerExit(Collider other) //on s'assure d'actualiser la liste d'enfant quand ils quittent la plateforme
    {

        if (other.gameObject.TryGetComponent<Transform>(out tempCont))
        { 
            if (chilTrans != null)
            {
                int index = chilTrans.IndexOf(tempCont);
                if (index >= 0)
                {
                    other.TryGetComponent<Riley_Main>(out Riley);
                    if (Riley)
                    { StartCoroutine(NoCheat()); }


                    chilTrans.RemoveAt(chilTrans.IndexOf(tempCont));

                    chilpos = Vector3.zero;
                    rilcount = 0;


                }
            }
        }
    }

    private IEnumerator NoCheat()
    {
        yield return new WaitForSeconds(1f);

        rilon = false;
    }

//en séparant AtBegin du start, on permet de voir les changements sur un custom editor.
    public void AtBegin()
    {
        gotPlateforme = (railStart != null && railEnd != null);
        tr = this.transform;
        platform = center.position - tr.position;
        if (gotPlateforme)
        {
            if (!telekinesis) Destroy(this.GetComponent<Telekinesis>());

            tempTransforms = railStart.GetComponentsInChildren<Transform>();

            start = tempTransforms[1];
            tempTransforms = railEnd.GetComponentsInChildren<Transform>();

            end = tempTransforms[1];
            telekinesisActive = false;
            begin = start.position;

            finish = end.position;
            speed = 0.5f;
            rail = finish - begin;
            tr.position = begin + posOnRail * rail;
            gizmo = tr.position + platform;
            tr.rotation=Quaternion.Euler(tr.rotation.eulerAngles.x, ((float)sens), tr.rotation.eulerAngles.z);
            aim = Camera.main.transform;
            this.TryGetComponent<BoxCollider>(out bc);
            mc = this.GetComponentInChildren<MeshCollider>();

            if ((isPlateformCeiling) && (isPlatform4m))
            {
                tr.position = new Vector3(tr.position.x, tr.position.y - 7.55f, tr.position.z);
                foreach (Transform child in tr)
                {
                    child.position = new Vector3(tr.position.x, tr.position.y + 7.55f, tr.position.z);
                }

                begin = new Vector3(begin.x, begin.y - 7.55f, begin.z);
                finish = new Vector3(finish.x, finish.y - 7.55f, finish.z);
                j = 0;
                //bc.transform.position = new Vector3(bc.transform.position.x, bc.transform.position.y - 7.55f, bc.transform.position.z) ;
                //mc.transform.localPosition= new Vector3(0, -4f, 0);
            }
            else if (isPlateformCeiling && !isPlatform4m)
            {
                tr.position = new Vector3(tr.position.x, tr.position.y - 3.55f, tr.position.z);
                foreach (Transform child in tr)
                {
                    child.position = new Vector3(tr.position.x, tr.position.y + 3.55f, tr.position.z);
                }

                begin = new Vector3(begin.x, begin.y - 3.55f, begin.z);
                finish = new Vector3(finish.x, finish.y - 3.55f, finish.z);
                j = 0;
            }
            
            if (defaultSpeed)
            { speedValue = 2f; }
            normTime = Mathf.Sqrt((finish - begin).sqrMagnitude) / speedValue;
            k = 1;
            startPos = tr.position;
        }
        
    }

    
}