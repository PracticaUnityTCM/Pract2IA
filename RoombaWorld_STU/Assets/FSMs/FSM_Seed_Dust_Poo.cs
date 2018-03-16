using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;

namespace FSM
{
    [RequireComponent(typeof(FSM_RouteExecutor))]
    [RequireComponent(typeof(FSM_WanderRoomba))]
    [RequireComponent(typeof(ROOMBA_Blackboard))]
    public class FSM_Seed_Dust_Poo : FiniteStateMachine
    {

        public enum State { INITIAL, SEED_POO, SEED_DUST, SEED_DUST_IN_MEMORY, PIVOT_STATE };

        public State currentState = State.INITIAL;

        private ROOMBA_Blackboard myBlackBoard;

        private FSM_RouteExecutor fsm_RouteExec;
        
        [SerializeField]
        private GameObject targetedDust;
        [SerializeField]
        private GameObject targetedPoo;
        [SerializeField]
        private GameObject nearestDust;

        private int dustToRemoveIndex;

        void Start()
        {
            myBlackBoard = GetComponent<ROOMBA_Blackboard>();
            fsm_RouteExec = GetComponent<FSM_RouteExecutor>();
            fsm_RouteExec.enabled = false;
            nearestDust = null;
        }

        public override void Exit()
        {
            targetedDust = null;
            targetedPoo = null;
            fsm_RouteExec.Exit(); ;
            fsm_RouteExec.enabled = false;
            base.Exit();
        }

        public override void ReEnter()
        {
            base.ReEnter();
            nearestDust = null;
            currentState = State.INITIAL;
        }

        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.PIVOT_STATE);
                    break;

                case State.SEED_POO:
                    GameObject otherPoo;
                    otherPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", myBlackBoard.farPooDetectionRadius);
                    if (otherPoo != null && !otherPoo.Equals(targetedPoo))
                    {
                        if (DistanceFromMe(otherPoo) < DistanceFromMe(targetedPoo))
                        {
                            targetedPoo = otherPoo;
                            fsm_RouteExec.Exit();
                            fsm_RouteExec.ReEnter();
                            fsm_RouteExec.target = otherPoo;
                        }
                    }
                    if (targetedPoo != null && CheckIfDustInCloseRange() || CheckIfDustInLargeRange())
                    {
                        /*GameObject dustOnTheWay;
                        dustOnTheWay = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", myBlackBoard.closeDustDetectionRadius);
                        if (dustOnTheWay != null)
                        {
                            if (!CheckIfDustIsInMemory(dustOnTheWay))
                            {
                                myBlackBoard.AddToMemory(dustOnTheWay);
                            }
                        }*/

                        if (CheckIfDustInCloseRange())
                        {
                            myBlackBoard.AddToMemory(targetedDust);
                            targetedDust = null;
                        }
                        if (CheckIfDustInLargeRange())
                        {
                            myBlackBoard.AddToMemory(targetedDust);
                            targetedDust = null;
                        }

                    }
                    if (targetedPoo != null && DistanceFromMe(targetedPoo) < myBlackBoard.pooReachedRadius)
                    {
                        CleanUp(targetedPoo);
                        ChangeState(State.PIVOT_STATE);
                        break;
                    }
                    break;

                case State.PIVOT_STATE:
                    if (CheckIfPooInCloseRange() || CheckIfPooInLargeRange())
                    {
                        ChangeState(State.SEED_POO);
                        break;
                    }
                    else if (myBlackBoard.memory.Count > 0)
                    {
                        ChangeState(State.SEED_DUST_IN_MEMORY);
                        break;
                    }
                    else if (CheckIfDustInCloseRange())
                    {
                        ChangeState(State.SEED_DUST);
                        break;
                    }
                    else if (CheckIfDustInLargeRange())
                    {
                        ChangeState(State.SEED_DUST);
                        break;
                    }
                    else
                    {

                    }
                    break;

                case State.SEED_DUST:
                    if (CheckIfPooInCloseRange() || CheckIfPooInLargeRange() && DistanceFromMe(targetedDust) > myBlackBoard.dustReachedRadius)
                    {
                        targetedDust = null;
                        ChangeState(State.SEED_POO);
                        break;
                    }
                    GameObject otherDust;
                    otherDust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", myBlackBoard.closeDustDetectionRadius);
                    if (otherDust != null && !otherDust.Equals(targetedDust))
                    {
                        if (DistanceFromMe(otherDust) < DistanceFromMe(targetedDust))
                        {
                            targetedDust = otherDust;
                            fsm_RouteExec.Exit();
                            fsm_RouteExec.ReEnter();
                            fsm_RouteExec.target = otherDust;
                        }
                    }
                    if (targetedDust != null && DistanceFromMe(targetedDust) < myBlackBoard.dustReachedRadius)
                    {
                        CleanUp(targetedDust);
                        ChangeState(State.PIVOT_STATE);
                        break;
                    }
                    break;

                case State.SEED_DUST_IN_MEMORY:
                    if (nearestDust != null && DistanceFromMe(nearestDust) < myBlackBoard.dustReachedRadius)
                    {
                        RemoveDustFromMemory(nearestDust);
                        CleanUp(nearestDust);
                        if (myBlackBoard.memory.Count > 0)
                        {
                            ChangeState(State.SEED_DUST_IN_MEMORY);
                            break;
                        }
                        else
                        {
                            ChangeState(State.PIVOT_STATE);
                            break;
                        }
                    }
                    break;
            }
        }
        
        void ChangeState(State newState)
        {
            //EXIT STATE LOGIC
            switch (currentState)
            {
                case State.SEED_POO:
                    targetedDust = null;
                    targetedPoo = null;
                    fsm_RouteExec.target = null;
                    fsm_RouteExec.Exit();
                    fsm_RouteExec.enabled = false;
                    break;

                case State.SEED_DUST:
                    targetedDust = null;
                    fsm_RouteExec.target = null;
                    fsm_RouteExec.Exit();
                    fsm_RouteExec.enabled = false;
                    break;

                case State.SEED_DUST_IN_MEMORY:
                    nearestDust = null;
                    fsm_RouteExec.target = null;
                    fsm_RouteExec.Exit();
                    fsm_RouteExec.enabled = false;
                    break;


                case State.PIVOT_STATE:
                    break;
            }

            //ENTER STATE LOGIC
            switch (newState)
            {
                case State.SEED_POO:
                    fsm_RouteExec.ReEnter();
                    fsm_RouteExec.target = targetedPoo;
                    fsm_RouteExec.enabled = true;
                    break;

                case State.SEED_DUST:
                    fsm_RouteExec.ReEnter();
                    fsm_RouteExec.target = targetedDust;
                    fsm_RouteExec.enabled = true;
                    break;

                case State.SEED_DUST_IN_MEMORY:
                    FindNearestDustInMemory();
                    fsm_RouteExec.ReEnter();
                    fsm_RouteExec.target = nearestDust;
                    fsm_RouteExec.enabled = true;
                    break;

                case State.PIVOT_STATE:
                    break;
            }
            currentState = newState;
        }

        // ////FUNCTIONS//// //
        private GameObject FindNearestDustInMemory()
        {
            //GameObject nearestDust = null;

            foreach (GameObject dust in myBlackBoard.memory)
            {
                if (nearestDust == null)
                {
                    nearestDust = dust;
                }
                else
                {
                    if (DistanceFromMe(dust) < DistanceFromMe(nearestDust))
                    {
                        nearestDust = dust;
                    }
                }
            }

            return nearestDust;
        }
        private void RemoveDustFromMemory(GameObject dustToRemove)
        {
            foreach (GameObject dust in myBlackBoard.memory)
            {
                if (dust.Equals(dustToRemove))
                {
                    dustToRemoveIndex = myBlackBoard.memory.IndexOf(dust);
                }
            }
            myBlackBoard.memory.RemoveAt(dustToRemoveIndex);
        }

        private void CleanUp(GameObject objToClean)
        {

            Destroy(objToClean);
        }
        
        private bool CheckIfDustIsInMemory(GameObject dust)
        {
            foreach (GameObject d in myBlackBoard.memory)
            {
                if (d.Equals(dust))
                {
                    return true;
                }
            }
            return false;
        }

        // ////SENSE EXTRA FUNCTIONS//// //
        
        bool CheckIfDustInCloseRange ()
        {
            GameObject dust;
            dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", myBlackBoard.closeDustDetectionRadius);
            if (dust != null && !dust.Equals(targetedDust))
            {
                targetedDust = dust;
                return true;
            }
            return false;
        }
        
        bool CheckIfPooInCloseRange()
        {
            GameObject poo;
            poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", myBlackBoard.closeDustDetectionRadius);
            if (poo != null && !poo.Equals(targetedPoo))
            {
                targetedPoo = poo;
                return true;
            }
            return false;
        }
        
        bool CheckIfDustInLargeRange()
        {
            GameObject dust;
            dust = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "DUST", myBlackBoard.farDustDetectionRadius);
            if (dust != null && !dust.Equals(targetedDust))
            {
                targetedDust = dust;
                return true;
            }
            return false;
        }
        
        bool CheckIfPooInLargeRange()
        {
            GameObject poo;
            poo = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "POO", myBlackBoard.farPooDetectionRadius);
            if (poo != null && !poo.Equals(targetedPoo))
            {
                targetedPoo = poo;
                return true;
            }
            return false;
        }

        private float DistanceFromMe(GameObject target)
        {
            float dis;
            
            dis = (transform.position - target.transform.position).magnitude;
            Debug.Log("FSM_SEED: DistanceFromMe Target = " + dis);
            return dis;
        }
    }
}
