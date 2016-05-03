/*
 * Author: HungLV
 * Class: StateBase.cs
 * Date: 2015/04/07
 */


using System.Collections.Generic;
using System.Text;

namespace Lib.Convert
{
    public class StateBase
    {
        public List<Transition> OutgoingTransitions;
        public string Name;
        public string ID;

        public bool IsAccepted;
        public bool IsInitial;

        public StateBase(string name, string id, bool isInitial, bool isAccepted)
        {
            Name = name;
            ID = id;
            OutgoingTransitions = new List<Transition>();

            IsInitial = isInitial;
            IsAccepted = isAccepted;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}