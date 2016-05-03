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
    public class Transition
    {
        public Event Event;
        public string Evt;

        public StateBase FromState;
        public StateBase ToState;

        public bool Visited;

        public List<Transition> OroginalTransitions;
        public string TransitionID;

        public Transition(Event e, StateBase from, StateBase to)
        {
            Event = e;
            Evt = e.BaseName;
            FromState = from;
            ToState = to;
        }

        public override string ToString()
        {
            return "\"" + FromState + "\"--" + Event + "-->\"" + ToState + "\"";
        }

        public string GetInfo()
        {
            string program = "";

            return Event + program;
        }

        public string GetTransitionID()
        {
            return TransitionID;
        }

        internal void GetGlobalVariables(List<string> vars)
        {
            throw new System.NotImplementedException();
        }
    }
}
