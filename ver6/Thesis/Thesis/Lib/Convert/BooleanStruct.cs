using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Convert;

namespace Thesis
{
    public class BooleanStruct
    {
        public StringBuilder variable;
        public StringBuilder init;
        public StringBuilder FromState;
        public StringBuilder ToState;
        public StringBuilder EventEncode;
        public StringBuilder bool_expression;
        public Dictionary<string, string> fromStateMapping;
        public Dictionary<string, string> toStateMapping;
        public Dictionary<string, string> eventMapping;

        public BooleanStruct()
        {
            variable = new StringBuilder();
            init = new StringBuilder();
            FromState = new StringBuilder();
            ToState = new StringBuilder();
            EventEncode = new StringBuilder();
            bool_expression = new StringBuilder();
            fromStateMapping = new Dictionary<string, string>();
            toStateMapping = new Dictionary<string, string>();
            eventMapping = new Dictionary<string, string>();
        }
    }
}
