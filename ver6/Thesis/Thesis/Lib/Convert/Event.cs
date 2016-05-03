/*
 * Author: HungLV
 * Date: 2015/04/12
 */


using System;
using System.Collections.Generic;

namespace Lib.Convert
{
    public class Event
    {
        public string BaseName; /* Basic event name */
        public string EventName;

        /* Constructor */
        public Event(string name)
        {
            BaseName = name; 
        }

        public override string ToString()
        {
            return BaseName;
        }
    }
}