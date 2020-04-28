using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;

namespace FSM
{
    [RequireComponent(typeof(FSM_Seed_Dust_Poo))]
    [RequireComponent(typeof(FSM_WanderRoomba))]
    [RequireComponent(typeof(FSM_RouteExecutor))]
    public class FSM_Recharge_Behavior : FiniteStateMachine
    {
        public enum State { INITIAL, SEED, WANDER, GO_TO_ENERGY_POINT, RECHARGE };

        public State currentState = State.INITIAL;

        private ROOMBA_Blackboard myBlackBoard;
        private FSM_RouteExecutor fsm_RouteExec;
        private FSM_Seed_Dust_Poo fsm_Seed;
        private FSM_WanderRoomba fsm_Wander;

        private GameObject energyPoint;

        void Start()
        {
            myBlackBoard = GetComponent<ROOMBA_Blackboard>();
            fsm_RouteExec = GetComponent<FSM_RouteExecutor>();
            fsm_Seed = GetComponent<FSM_Seed_Dust_Poo>();
            fsm_Wander = GetComponent<FSM_WanderRoomba>();

            fsm_Seed.enabled = false;
            fsm_Wander.enabled = true;
            fsm_RouteExec.enabled = false;
        }

        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.WANDER);
                    break;

                case State.WANDER:
                    //COUNT ENERGY LEFT. WHEN BELOW 15 CHANGE TO RECHARGE STATE
                    if (myBlackBoard.currentCharge < myBlackBoard.minCharge)
                    {
                        ChangeState(State.GO_TO_ENERGY_POINT);
                        break;
                    }

                    //CHECK IF TRASH TO CLEAN
                    if (CheckIfPooInCloseRange() || CheckIfPooInLargeRange() || CheckIfDustInCloseRange() || CheckIfDustInLargeRange())
                    {
                        ChangeState(State.SEED);
                        break;       
                    }
                    
                    break;

                case State.SEED:
                    //COUNT ENERGY LEFT. WHEN BELOW 15 CHANGE TO RECHARGE STATE
                    if (myBlackBoard.currentCharge < myBlackBoard.minCharge)
                    {
                        ChangeState(State.GO_TO_ENERGY_POINT);
                        break;
                    }

                    //CHECK IF NO TRASH TO CLEAN
                    if (!CheckIfPooInCloseRange() && !CheckIfPooInLargeRange() && !CheckIfDustInCloseRange() && !CheckIfDustInLargeRange())
                    {
                        ChangeState(State.WANDER);
                        break;
                    }

                    break;

                case State.GO_TO_ENERGY_POINT:
                    //IF REACHED POINT CHANGE STATE RECHARGE
                    if (DistanceFromMe(energyPoint) < myBlackBoard.dustReachedRadius)
                    {
                        ChangeState(State.RECHARGE);
                        break;
                    }
                    break;

                case State.RECHARGE:
                    //GET FULL CHARGE (WAIT UNTIL FULL CHARGE). THEN CHANGE TO WANDER_SEED STATE
                    myBlackBoard.Recharge(Time.deltaTime);
                    if (myBlackBoard.currentCharge == 100)
                    {
                        ChangeState(State.WANDER);
                        break;
                    }
                    break;
            }

            if (currentState != State.RECHARGE) //DISCHARGE WHILE NOT IN RECHARGE STATE
            {
                myBlackBoard.Discharge(Time.deltaTime); 
            }
        }

        void ChangeState(State newState)
        {
            //EXIT STATE LOGIC
            switch (currentState)
            {
                case State.WANDER:
                    fsm_Wander.Exit();
                    fsm_Wander.enabled = false;
                    break;

                case State.SEED:
                    fsm_Seed.Exit();
                    fsm_Seed.enabled = false;
                    break;

                case State.GO_TO_ENERGY_POINT:
                    fsm_RouteExec.Exit();
                    fsm_RouteExec.enabled = false;
                    energyPoint = null;
                    break;

                case State.RECHARGE:
                    
                    break;
            }

            //ENTER STATE LOGIC
            switch (newState)
            {
                case State.WANDER:
                    fsm_Wander.enabled = true;
                    fsm_Wander.ReEnter();
                    break;

                case State.SEED:
                    fsm_Seed.enabled = true;
                    fsm_Seed.ReEnter();
                    break;

                case State.GO_TO_ENERGY_POINT:
                    myBlackBoard.memory.Clear();
                    FindNearestRechargePoint(); //FIND THE NEAREST CHARGE POINT
                    fsm_RouteExec.ReEnter();
                    fsm_RouteExec.target = energyPoint;
                    fsm_RouteExec.enabled = true; //ENABLE ROUTEEXECUTOR_FSM
                    break;

                case State.RECHARGE:
                    
                    break;
            }
            currentState = newState;
        }

        // ////FUNCTIONS//// //

        void FindNearestRechargePoint ()
        {
            GameObject[] rechargePoints;
            rechargePoints = GameObject.FindGameObjectsWithTag("ENERGY");
            foreach (GameObject r in rechargePoints)
            {
                if (energyPoint == null)
                {
                    energyPoint = r;
                }
                else
                {
                    if (DistanceFromMe(r) < DistanceFromMe(energyPoint))
                    {
                        energyPoint = r;
                    }
                }
            }
        }

        // ////SENSE EXTRA FUNCTIONS//// //
        bool CheckIfDustInCloseRange()
        {
            GameObject dust;
            dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", myBlackBoard.closeDustDetectionRadius);
            if (dust != null)
            {
                return true;
            }
            return false;
        }

        bool CheckIfPooInCloseRange()
        {
            GameObject poo;
            poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", myBlackBoard.closeDustDetectionRadius);
            if (poo != null)
            {
                return true;
            }
            return false;
        }

        bool CheckIfDustInLargeRange()
        {
            GameObject dust;
            dust = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "DUST", myBlackBoard.farDustDetectionRadius);
            if (dust != null)
            {
                return true;
            }
            return false;
        }

        bool CheckIfPooInLargeRange()
        {
            GameObject poo;
            poo = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "POO", myBlackBoard.farPooDetectionRadius);
            if (poo != null)
            {
                return true;
            }
            return false;
        }

        private float DistanceFromMe(GameObject target)
        {
            float dis;

            dis = (transform.position - target.transform.position).magnitude;

            return dis;
        }
    }
}


