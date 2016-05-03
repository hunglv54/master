/*
 *********************************************************************************************
 *
 *   This file is part of CELL.
 *   CELL is an open-source (free) software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or any later version.
 *
 *   You are supposed to receive a copy of the GNU Lesser General Public License
 *   along with CELL source code.  If not, see <http://www.gnu.org/licenses/>.
 *
 *   Author: Ji Kun <jikun@comp.nus.edu.sg> Lin Shang-Wei <tsllsw@nus.edu.sg>
 *
 *********************************************************************************************
 */


using System.Collections.Generic;
using System.Text;

namespace Lib.Convert
{
    public class AutomatonBase
    {
        public StateBase InitialState;
        public string Name;
        public List<string> Parameters;
        public List<StateBase> States;
        public List<Transition> Transitions;

        public List<string> EventList;
        public bool IsProperty;
        public int StateNumber;
        public int TransitionNumber;

        public const int HashTableSize = 4096;

        public AutomatonBase(string name, List<string> vars, List<StateBase> states)
        {
            Name = name;
            InitialState = states[0];
            InitialState.IsInitial = true;
            States = states;
            Parameters = vars;
            EventList = new List<string>();
            StateNumber = states.Count;
            TransitionNumber = 0;
            IsProperty = false;
            Transitions = new List<Transition>();
        }

        public AutomatonBase()
        {
            // TODO: Complete member initialization
        }

        /*public List<Transition> GetTransitions()
        {
            List<Transition> evtList = new List<Transition>();

            StringHashTable visitedTrans = new StringHashTable(64);

            foreach (Transition trans in Transitions)
            {
                string key = "";

                    key = trans.GetSimpleInfo();

                if (!visitedTrans.ContainsKey(key))
                {
                    visitedTrans.Add(key);
                    evtList.Add(trans);
                }
            }

            return evtList;
        }*/

        /// <summary>
        /// Return all the events
        /// </summary>
        /// <returns></returns>
        public List<Event> GetEvents()
        {
            List<Event> evtList = new List<Event>();

            foreach (Transition trans in Transitions)
            {
                if (evtList.Contains(trans.Event) == false)
                {
                    evtList.Add(trans.Event);
                }
            }

            return evtList;
        }

        public void SetTransitions(List<Transition> transitions)
        {
            Transitions = transitions;
            foreach (Transition transition in transitions)
            {
                foreach (StateBase state in States)
                {
                    if (state.Name == transition.FromState.Name)
                    {
                        state.OutgoingTransitions.Add(transition);
                    }
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Process \"" + Name + "\"(" + ")" + InitialState.ToString());
            foreach (Transition transition in Transitions)
            {
                sb.AppendLine(transition.ToString());
            }
            sb.AppendLine(";");
            return sb.ToString();
        }

        /// <summary>
        /// Add the event names into EventList
        /// </summary>
        public void CollectEvent()
        {
            foreach (Transition transition in Transitions)
            {
                if (transition.Event.EventName != null)
                {
                    string eventName = transition.Event.ToString();
                    AddEvent(eventName);
                }
                else
                {
                    transition.Event.EventName = transition.Event.ToString();
                    AddEvent(transition.Event.EventName);
                }
            }
        }

        
        public void AddEvent(string evt)
        {
            if (EventList.Contains(evt) == false)
            {
                EventList.Add(evt);
            }
        }


        public AutomatonBase Clone()
        {
            var stateList = new List<StateBase>();
            var newInitState = new StateBase(InitialState.Name, InitialState.ID, InitialState.IsInitial,
                                             InitialState.IsAccepted
                                             );
            stateList.Add(newInitState);
            var clonedAutomaton = new AutomatonBase(Name + "_d_", null, stateList);

            var stateDic = new Dictionary<StateBase, StateBase>();
            stateDic.Add(InitialState, newInitState);

            foreach (StateBase oState in States)
            {
                if (oState.IsInitial)
                {
                    continue;
                }

                var nState = new StateBase(oState.Name, oState.Name, oState.IsInitial, oState.IsAccepted);
                clonedAutomaton.States.Add(nState);

                stateDic.Add(oState, nState);
            }

            foreach (StateBase state in States)
            {
                foreach (Transition oldOTran in state.OutgoingTransitions)
                {
                    StateBase nSource = null;
                    StateBase nDestination = null;

                    nSource = stateDic[oldOTran.FromState];
                    nDestination = stateDic[oldOTran.ToState];


                    var newOTran = new Transition(oldOTran.Event, nSource, nDestination);

                    nSource.OutgoingTransitions.Add(newOTran);
                }
            }

            foreach (string evt in EventList)
            {
                clonedAutomaton.EventList.Add(evt);
            }

            return clonedAutomaton;
        }

        public Encode Encoding()
        {
            var encoder = new Encode();
            encoder.Initialize(this, IsProperty);
            return encoder;
        }


        public void CollectTransitions()
        {
            foreach (var stateBase in States)
            {
                foreach (var tran in stateBase.OutgoingTransitions)
                {
                    if (!Transitions.Contains(tran))
                    {
                        Transitions.Add(tran);
                    }
                }
            }
        }
    }
}