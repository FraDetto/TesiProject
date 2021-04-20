using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody myRB;

    public float reactionTime = 1.2f;
    public float distanceRange = 7.0f;
    public bool firstRush = true;

    //public bool shieldUp;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");

        myRB = GetComponent<Rigidbody>();

        ////////// MAIN FSM ///////////////////
        FSMState Chase = new FSMState();
        Chase.enterActions.Add(ChaseBoos);
        Chase.stayActions.Add(ChaseBoos);


        FSMState Combact = new FSMState();
        Combact.enterActions.Add(combactFase);
        Combact.stayActions.Add(combactFase);

        // Define transitions
        FSMTransition t1 = new FSMTransition(ChaseToCombact);
        FSMTransition t2 = new FSMTransition(CombactToChase);


        // Link states with transitions
        Chase.AddTransition(t1, Combact);
        Combact.AddTransition(t2, Chase);


       


        //////////// COMBACT FSM /////////////////
        FSMState Attack = new FSMState();
        Attack.enterActions.Add(AttackBoss);
        Attack.stayActions.Add(AttackBoss);

        FSMState Defend = new FSMState();
        Defend.enterActions.Add(DefendFromAttack);

        FSMState Special = new FSMState();
        Special.enterActions.Add(ActiveSpecial);

        // Define transitions

        FSMTransition t3 = new FSMTransition(AttkToSpec);
        FSMTransition t4 = new FSMTransition(AttkToDef);
        FSMTransition t5 = new FSMTransition(DefToAttk); // different from t1
        FSMTransition t6 = new FSMTransition(DefToSpec);
        FSMTransition t7 = new FSMTransition(SpecToAttk);


        // Link states with transitions
        Attack.AddTransition(t3, Special);
        Attack.AddTransition(t4, Defend);


        Defend.AddTransition(t5, Attack);
        Defend.AddTransition(t6, Special);

        Special.AddTransition(t7, Attack);

        
        // Setup a FSA at initial state
        fsmCombact = new FSM(Attack);


        // Setup a FSA at initial state
        fsmMain = new FSM(Chase);

        // Start monitoring
        StartCoroutine(Fight());
    }




    // Periodic update, run forever
    public IEnumerator Fight()
    {
        while (true)
        {
            fsmMain.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }




    /////////////////// MAIN FSM ////////////////////////////////

    // CONDITIONS

    public bool ChaseToCombact()
    {
        //Debug.Log("ChaseToCombact");
        if ( ((boss.transform.position - myRB.transform.position).magnitude <= distanceRange && GetComponent<moreSpecificProfile>().publicGetStatus() != 2) || GetComponent<moreSpecificProfile>().publicGetStatus() == 1)
        {            
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool CombactToChase()
    {
        //Debug.Log("CombactToChase");
        if (GetComponent<TankProfile>().publicGetStatus() == 0)//If status OK work normal, otherwise FALSE -> Rooted or stunned can't move
        {
            return !ChaseToCombact();
        }
        else
        {
            return false;
        }
        
    }

    
    // ACTIONS

    public void ChaseBoos()//avvicinati al boss
    {
        /*Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
        Vector3 toBossPos = (verticalAdj - transform.position);

        if(toBossPos.magnitude > distanceRange)
        {
            Debug.Log("Tank distante dal boss mi sposto");
            transform.LookAt(verticalAdj);
            myRB.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }*/
        
        if (!GetComponent<TankMovement>().chaseFlag && !GetComponent<TankProfile>().isDashing)
        {
            //Debug.Log("ChaseBoos");
            GetComponent<TankMovement>().chaseFlag = true;
        }    

    }


    public void combactFase()
    {
        //Debug.Log("combactFase TANK");
        if (GetComponent<TankProfile>().publicGetStatus() != 2) //if 2 = stunned can't attack and move
        {
            fsmCombact.Update();
        }
       
    }




    //////////////////// COMBACT FSM //////////////////////////////

    // CONDITIONS


    public bool AttkToDef()
    {
        //if( (!GetComponent<TankProfile>().cooldownShield || !GetComponent<TankProfile>().cooldownDash) && (boss.GetComponent<BossProfile>().isAttacking && boss.GetComponent<BossProfile>().target.Equals(transform.tag)) && !GetComponent<TankProfile>().swordActive)
        if( (!GetComponent<TankProfile>().cooldownShield || !GetComponent<TankProfile>().cooldownDash) &&  (boss.GetComponent<BossProfile>().isAttacking && ( boss.GetComponent<BossProfile>().target.Equals(transform.tag) || boss.GetComponent<BossProfile>().isUsingAoE) ) )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool AttkToSpec()
    {
        
        if (!GetComponent<TankProfile>().cooldownSpecial && !GetComponent<TankProfile>().swordActive)
        {
            //Debug.Log("ATTKSPEC");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DefToAttk()
    {
        if (GetComponent<TankProfile>().cooldownSpecial && !GetComponent<TankProfile>().shieldActive)
        {
            //Debug.Log("SPEC NOT AVAIBLE SO DEF ATTK");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DefToSpec()
    {
        if (!GetComponent<TankProfile>().cooldownSpecial && !GetComponent<TankProfile>().shieldActive)
        {
            //Debug.Log("SPEC NOT AVAIBLE SO DEF ATTK");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SpecToAttk()
    {
        return true;
    }



    // ACTIONS

    public void AttackBoss()
    {
       
        if (!firstRush )
        {
            Debug.Log("ATTACK BOSS !FIRSTRUSH");
            GetComponent<TankProfile>().attackWithSword();
        }
        else
        {
            Debug.Log("ATTACK BOSS FIRST RUSH");
            firstRush = false;
        }
        
    }

    public void DefendFromAttack()
    {
        //qua mettere scelta tra dash e scudo (con prob 50%)
        if ( (!GetComponent<TankProfile>().cooldownShield && !GetComponent<TankProfile>().cooldownDash) )
        {
            //qua probabilita'
            int random = Random.Range(1, 101);

            if( random >=1 && random < 60)
            {
                GetComponent<TankProfile>().defendWithShield();
            }
            else
            {
                GetComponent<TankProfile>().rollAway();
            }
        }
        else
        {
            if (!GetComponent<TankProfile>().cooldownShield)
            {
                GetComponent<TankProfile>().defendWithShield();
            }
            else
            {
                GetComponent<TankProfile>().rollAway();
            }
        }    
    }

    public void ActiveSpecial()
    {

     GetComponent<TankProfile>().specialInAction();
       
    }
}
    
