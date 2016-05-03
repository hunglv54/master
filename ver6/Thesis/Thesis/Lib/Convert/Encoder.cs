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


using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.Convert
{
    public class EncodedTranstion
    {
        public string Event;
        public StateBase fromName;
        public StateBase toName;
        public List<int> FromState;
        public List<int> ToState; //0 for false; 1 for true; 2 for remaining;

        public EncodedTranstion(string eventName, List<int> fromState, List<int> toState, StateBase fromName, StateBase toName)
        {
            this.FromState = fromState;
            this.ToState = toState;
            this.fromName = fromName;
            this.toName = toName;
            Event = eventName;
        }
    }

    public class Encode
    {
        public int EventBoolVarNeeded;
        public Dictionary<string, List<bool>> EventMapping;
        public List<List<int>> EventPos;
        public Dictionary<int, string> EventRevertMapping;
        public int Gap;
        public List<bool> InitialStateEncoding;
        public bool IsProperty;
        public int PropertyPosition;
        public List<bool> PropertyStateEncoding;
        public int StateBoolVarNeeded;
        public Dictionary<string, List<bool>> StateMapping;
        public List<List<int>> StatePos;
        public List<EncodedTranstion> Transtions;
        public List<string> EventList;

        public Encode()
        {
            StatePos = new List<List<int>>();
            EventPos = new List<List<int>>();
            EventList = new List<string>();
            StateMapping = new Dictionary<string, List<bool>>();
            EventMapping = new Dictionary<string, List<bool>>();
            EventRevertMapping = new Dictionary<int, string>();
            StateBoolVarNeeded = 0;
            EventBoolVarNeeded = 0;
            InitialStateEncoding = new List<bool>();
            IsProperty = false;
            PropertyStateEncoding = new List<bool>();
            PropertyPosition = -1;
            Transtions = new List<EncodedTranstion>();
        }

        public void Initialize(AutomatonBase model, bool isProperty)
        {
            int stateNum = model.States.Count;
            int eventNum = model.EventList.Count;

            // Count number of bool variables need for encoding state
            if (stateNum == 1)
                StateBoolVarNeeded = 1;
            else
                StateBoolVarNeeded = (int)Math.Ceiling((Math.Log(stateNum, 2)));

            // Count number of bool variables need to use for encoding event
            if (eventNum == 1)
                EventBoolVarNeeded = 1;
            else
                EventBoolVarNeeded = (int) Math.Ceiling((Math.Log(eventNum, 2)));

            //encoding States
            InitialStateEncoding = StateEncoding(StateBoolVarNeeded, 0);

            foreach (StateBase state in model.States)
            {
                List<bool> t = StateEncoding(StateBoolVarNeeded, model.States.IndexOf(state));
                StateMapping.Add(state.ID, t);
                //  StateRevertMapping.Add(t,state.ID);

                //Note:modified to IsAccepted
                if (isProperty && state.IsAccepted)
                {
                    IsProperty = true;
                    PropertyPosition = 0;
                    PropertyStateEncoding = StateEncoding(StateBoolVarNeeded, model.States.IndexOf(state));
                }
            }

            //encoding Events
            foreach (string eventName in model.EventList)
            {
                EventMapping.Add(eventName, EventEncoding(EventBoolVarNeeded, model.EventList.IndexOf(eventName)));
                EventRevertMapping.Add(model.EventList.IndexOf(eventName), eventName);
            }

            foreach (Transition tran in model.Transitions)
            {
                var to = new List<int>();
                foreach (bool b in StateMapping[tran.ToState.ID])
                {
                    if (b)
                        to.Add(1);
                    else
                    {
                        to.Add(0);
                    }
                }
                var from = new List<int>();
                foreach (bool b in StateMapping[tran.FromState.ID])
                {
                    if (b)
                        from.Add(1);
                    else
                    {
                        from.Add(0);
                    }
                }
                var eTran = new EncodedTranstion(tran.Event.EventName, from, to, tran.FromState, tran.ToState);
                Transtions.Add(eTran);
            }
        }

        public static List<bool> EventEncoding(int length, int index)
        {
            var res = new List<bool>();
            int p = index;
            for (int i = 0; i < length; i++)
            {
                int d = p%2;
                p = p/2;
                if (d == 1)
                {
                    res.Add(true);
                }
                else
                {
                    res.Add(false);
                }
            }
            return res;
        }

        public static List<bool> StateEncoding(int length, int index)
        {
            var res = new List<bool>();
            int p = index;
            for (int i = 0; i < length; i++)
            {
                int d = p % 2;
                p = p / 2;
                if (d == 1)
                {
                    res.Add(true);
                }
                else
                {
                    res.Add(false);
                }
            }
            return res;
        }

        //private EncodedTranstion Parrallel(string name, EncodedTranstion[] t, Encode[] encodes)
        //{
        //    var fromName = new List<StateBase>();
        //    var toName = new List<StateBase>();
        //    var from = new List<int>();
        //    var to = new List<int>();
        //    for (int i = 0; i < t.Count(); i++)
        //    {
        //        if (t[i] == null)
        //        {
        //            for (int k = 0; k < encodes[i].StateBoolVarNeeded; k++)
        //            {
        //                from.Add(2);
        //                to.Add(2);
        //            }
        //        }
        //        else
        //        {
        //            from.AddRange(t[i].FromState);
        //            to.AddRange(t[i].ToState);
        //            fromName.AddRange(t[i].fromName);
        //            toName.AddRange(t[i].toName);
        //        }
        //    }
        //    return new EncodedTranstion(name, from, to, t[i].);
        //}


        //public static Encode ComposeWithEvents(AutomatonBase[] models, Encode[] encodes,
        //                                       Dictionary<string, List<bool>> encodeEventMapping,
        //                                       Dictionary<int, string> encodeEventRevertMapping)
        //{
        //    var result = new Encode();
        //    result.EventMapping = encodeEventMapping;
        //    result.EventRevertMapping = encodeEventRevertMapping;

        //    var eventList = new List<string>();

        //    // StringBuilder initialStateId = new StringBuilder();
        //    //int stateCount = 0;
        //    for (int i = 0; i < models.Length; i++)
        //    {
        //        result.StateBoolVarNeeded += encodes[i].StateBoolVarNeeded;

        //        List<string> events = models[i].EventList;
        //        foreach (string evt in events)
        //        {
        //            if (!eventList.Contains(evt))
        //            {
        //                eventList.Add(evt);
        //            }
        //        }

        //        if (encodes[i].IsProperty)
        //        {
        //            result.IsProperty = true;
        //            result.PropertyStateEncoding = encodes[i].PropertyStateEncoding;

        //            result.PropertyPosition = result.StateBoolVarNeeded - encodes[i].StateBoolVarNeeded;
        //        }

        //        // initialStateId.Append(encodes[i].StateMapping)
        //    }

        //    // result.EventBoolVarNeeded = (int)Math.Ceiling((Math.Log(EventList.Count, 2)));
        //    result.EventBoolVarNeeded = (int) Math.Ceiling((Math.Log(encodeEventMapping.Count, 2)));
        //    result.EventList = eventList;

        //    result.InitialStateEncoding = StateEncoding(result.StateBoolVarNeeded, 0);

        //    var isParallel = new bool[eventList.Count];
        //    for (int i = 0; i < isParallel.Length; i++)
        //    {
        //        isParallel[i] = false;
        //    }
        //    var eventTranMapping = new Dictionary<string, List<EncodedTranstion>[]>();

        //    int e = 0;
        //    foreach (string evt in eventList)
        //    {
        //        var temp = new List<EncodedTranstion>[models.Count()];
        //        int count = 0;
        //        for (int i = 0; i < models.Count(); i++)
        //        {
        //            var t = new List<EncodedTranstion>();
        //            if (models[i].EventList.Contains(evt))
        //            {
        //                count++;
        //                foreach (EncodedTranstion eTran in encodes[i].Transtions)
        //                {
        //                    if (eTran.Event == evt)
        //                    {
        //                        t.Add(eTran);
        //                    }
        //                }
        //            }
        //            temp[i] = t;
        //        }
        //        if (count > 1)
        //        {
        //            isParallel[e++] = true;
        //        }
        //        eventTranMapping.Add(evt, temp);
        //    }

        //    int ei = 0;
        //    foreach (string evt in eventList)
        //    {
        //        List<EncodedTranstion>[] temp = eventTranMapping[evt];
        //        if (!isParallel[ei])
        //        {
        //            int pos = 0;
        //            var t = new List<EncodedTranstion>();
        //            for (int i = 0; i < temp.Length; i++)
        //            {
        //                if (temp[i] != null)
        //                {
        //                    pos = i;
        //                    t = temp[i];
        //                }
        //            }
        //            foreach (EncodedTranstion encodedTranstion in t)
        //            {
        //                var from = new List<int>();
        //                var to = new List<int>();
        //                for (int i = 0; i < models.Count(); i++)
        //                {
        //                    if (i != pos)
        //                    {
        //                        for (int k = 0; k < encodes[i].StateBoolVarNeeded; k++)
        //                        {
        //                            from.Add(2);
        //                            to.Add(2);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        from.AddRange(encodedTranstion.FromState);
        //                        to.AddRange(encodedTranstion.ToState);
        //                    }
        //                }
        //                result.Transtions.Add(new EncodedTranstion(evt, from, to));
        //            }
        //        }
        //        else
        //        {
        //            var count = new int[models.Count()];
        //            long total = 1;
        //            for (int p = 0; p < models.Count(); p++)
        //            {
        //                if (temp[p].Count != 0)
        //                {
        //                    count[p] = temp[p].Count;
        //                    total *= count[p];
        //                }
        //            }

        //            var allTran = new EncodedTranstion[total][];

        //            for (int p = 0; p < total; p++)
        //            {
        //                allTran[p] = new EncodedTranstion[models.Count()];
        //            }

        //            for (int i = 0; i < temp.Length; i++)
        //            {
        //                if (temp[i].Count == 0)
        //                {
        //                    for (int j = 0; j < allTran.Length; j++)
        //                    {
        //                        allTran[j][i] = null;
        //                    }
        //                }
        //                else
        //                {
        //                    List<EncodedTranstion> t = temp[i];

        //                    if (t.Count == 1)
        //                    {
        //                        for (int j = 0; j < allTran.Length; j++)
        //                        {
        //                            allTran[j][i] = t[0];
        //                        }
        //                    }
        //                    else
        //                    {
        //                        for (int j = 0; j < allTran.Length; j++)
        //                        {
        //                            allTran[j][i] = t[j%t.Count];
        //                        }
        //                    }
        //                }
        //            }
        //            foreach (var encodedTranstions in allTran)
        //            {
        //                result.Transtions.Add(result.Parrallel(evt, encodedTranstions, encodes));
        //            }
        //        }
        //    }

        //    return result;
        //}

        //public void AddAllEvents(List<string> GlobalEvents)
        //{
        //    foreach (string evt in GlobalEvents)
        //    {
        //        if (!EventList.Contains(evt))
        //        {
        //            var from = new List<int>();
        //            var to = new List<int>();
        //            for (int i = 0; i < StateBoolVarNeeded; i++)
        //            {
        //                from.Add(2);
        //                to.Add(2);
        //            }
        //            var etran = new EncodedTranstion(evt, from, to);

        //            Transtions.Add(etran);
        //        }
        //    }
        //}
    }
}