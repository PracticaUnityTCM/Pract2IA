using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Steerings;
[RequireComponent(typeof(FSM_RouteExecutor))]
public class FSM_WanderRoomba : FiniteStateMachine
{

    public enum States { INITIAL, MISSINGPOINT, GOTOPOINT };
    public States currentState = States.INITIAL;
    private FSM_RouteExecutor RouterExecutor;
    private ROOMBA_Blackboard RoombaBB;
    public Vector3 target;
    public GameObject GOTarjet;
    // Use this for initialization
    void Start()
    {
        RouterExecutor = GetComponent<FSM_RouteExecutor>();
        RoombaBB = GetComponent<ROOMBA_Blackboard>();
        RouterExecutor.enabled = false;
        GOTarjet = new GameObject("TargetRoomba");
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.INITIAL:
                ChangeState(States.MISSINGPOINT);
                
                break;
            case States.MISSINGPOINT:
                target = RandomLocationGenerator.RandomPatrolLocation();       
                
                GOTarjet.transform.position = target;
                ChangeState(States.GOTOPOINT);
                break;
            case States.GOTOPOINT:
                if (RouterExecutor.currentState == FSM_RouteExecutor.State.TERMINATED)
                    ChangeState(States.MISSINGPOINT);
                break;
        }
    }
    void ChangeState(States newState)
    {
        switch (currentState)
        {
        
            case States.GOTOPOINT:
                RouterExecutor.Exit();
                RouterExecutor.target = null;
                RouterExecutor.enabled = false;
                break;
        }
        switch (newState)
        {
           
            case States.GOTOPOINT:
                RouterExecutor.ReEnter();
                RouterExecutor.target = GOTarjet;
                RouterExecutor.enabled = true;
                break;
        }
        currentState = newState;
    }
}