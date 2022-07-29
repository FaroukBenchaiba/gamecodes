using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Rédigé par Flavien le 23/05/22
 * Updates: none
 * 28-05-2022 par farouk ajout coroutines
 * 01-06-2022 par Farouk cycle on stay.
*/
public class Ing_Totem : MonoBehaviour
{
    public SO_DataRiley dataRiley; //le scriptable object
    private Coroutine Waitasec; //coroutine pour coroutine check
    private bool hallucinationOn;

    void OnEnable()
    {
        Waitasec = null;
        hallucinationOn = false;
        
    }
    public void IncreaseHallucinationLevel() 
    {
        if (Waitasec != null)//coroutine check
        { return; }
        else
        {
            //Debug.Log(this + "coroutine Waitasec before start coroutine = " + Waitasec);
            Waitasec = StartCoroutine(HallucinationPlus());
            //Debug.Log(this+ "coroutine Waitasec after start coroutine = "+Waitasec);
        }
        
    }
    public void DecreaseHallucinationLevel()
    {


        //Debug.Log(this + "Va appeler la diminution d'hallucination");
        StartCoroutine(PludAlu());
        
    }

    private IEnumerator PludAlu()
    {
        yield return new WaitForEndOfFrame();
        if (Waitasec == null)
        { hallucinationOn = false;
            //Debug.Log(this+" will decrease halllucination level");
            dataRiley.DecreaseHallucinationLevel();
        }
    }

    private IEnumerator HallucinationPlus()
    {
        if (hallucinationOn==false) //on s'assure que l augmentation n'arrive qu'une fois.
        {
            //Debug.Log(this+" j'augmente hallucinations level");
            dataRiley.IncreaseHallucinationLevel();
            hallucinationOn = true;
            
        }
        yield return new WaitForSeconds(0.9f);
        

        StartCoroutine(Check());


    }

    private IEnumerator Check()
    {
        Waitasec = null;
        yield return new WaitForSeconds(0.3f);
        //Debug.Log(this + "attend qu on sorte de l'aura");
        if (Waitasec == null)
        { DecreaseHallucinationLevel(); }
    }

    

    public void GestaltComplete()
    {
        if (hallucinationOn == true)
        { dataRiley.DecreaseHallucinationLevel(); }
        this.gameObject.SetActive(false);
    }
    public void PuzzleGameState(bool b)
    {
        if (b)
        {
            dataRiley.GameStateTracker = SO_DataRiley.GameState.Puzzle;
        } else
        {
            dataRiley.GameStateTracker = SO_DataRiley.GameState.Exploration;
        }
    }
}
