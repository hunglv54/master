using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Convert;
using Thesis;
using System.IO;

namespace Lib.Convert
{
    public class ConvertToBF
    {
        /* This function will delete space and '(' or ')' in automata */
        public static string RemoveWhitespace(string input)
        {
            int j = 0, inputlen = input.Length;
            char[] newarr = new char[inputlen];

            for (int i = 0; i < inputlen; ++i)
            {
                char tmp = input[i];

                if (!char.IsWhiteSpace(tmp) && tmp != ')' && tmp != '(')
                {
                    newarr[j] = tmp;
                    ++j;
                }
            }

            return new String(newarr, 0, j);
        }

        /* Read content from input file and convert it to automat */
        public List<AutomatonBase> readFile(string fileName)
        {
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(fileName); /* Read file */
                int numOfAutomat = Int32.Parse(file.ReadLine().ToString()); /* Number of automata */
                bool isInit = false;
                List<AutomatonBase> models = new List<AutomatonBase>();

                for (int i = 0; i < numOfAutomat; i++)
                {
                    List<StateBase> inputStateList = new List<StateBase>();
                    string initState = file.ReadLine().ToString(); /* Init state */
                    string []listOfState = file.ReadLine().ToString().Split();

                    foreach (string state in listOfState)
                    {
                        if (state.CompareTo(initState) == 0)
                        {
                            isInit = true;
                            inputStateList.Add(new StateBase(state, state, isInit, true));
                            isInit = false;
                        }
                        else
                        {
                            inputStateList.Add(new StateBase(state, state, isInit, true));
                        }
                    }

                    /* Delete white space and ( */
                    string lineOfTrans = RemoveWhitespace(file.ReadLine().ToString());
                    string[] listOfTrans = lineOfTrans.Split(';'); /* Split to each transition */
                    List<StateBase> fromSt = new List<StateBase>();
                    List<StateBase> toSt = new List<StateBase>();

                    foreach (string trans in listOfTrans)
                    {

                        string[] list = trans.Split('.');
                        string from = list[0];
                        string evt = list[1];
                        string to = list[2];
                        StateBase sbFrom = inputStateList[0];
                        StateBase sbTo = inputStateList[0];

                        for (int j = 0; j < inputStateList.Count; j++)
                        {
                            // Add members to FromList state
                            bool flag = false;
                            if (from.CompareTo(inputStateList[j].ToString()) == 0)
                            {
                                sbFrom = inputStateList[j];
                                if(fromSt.Count > 0)
                                {
                                    foreach (StateBase s in fromSt)
                                        if (s.Name.CompareTo(from) == 0)
                                            flag = true;
                                }

                                if(flag == false)
                                    fromSt.Add(sbFrom);
                            }

                            // Add members to ToList state
                            flag = false;
                            if (to.CompareTo(inputStateList[j].ToString()) == 0)
                            {
                                sbTo = inputStateList[j];
                                if(toSt.Count > 0)
                                {
                                    foreach (StateBase s in toSt)
                                        if (s.Name.CompareTo(to) == 0)
                                            flag = true;
                                }

                                if(flag == false)
                                    toSt.Add(sbTo);
                            }
                        }


                        for (int j = 0; j < inputStateList.Count; j++)
                            if (inputStateList[j].ToString().CompareTo(from.ToString()) == 0)
                                inputStateList[j].OutgoingTransitions.Add(new Transition(new Event(evt), sbFrom, sbTo));
                    }
                    AutomatonBase input = new AutomatonBase("Input", null, inputStateList, fromSt, toSt);
                    input.CollectTransitions();
                    input.CollectEvent();
                    models.Add(input);
                }
                return models;
            }
            catch (IOException )
            {
                Console.WriteLine("File not found!");
                return null;
            }
        }

        /* Create boolean function */
        public List<BooleanStruct> BoolFormula(List<AutomatonBase> models)
        {
            List<BooleanStruct> res = new List<BooleanStruct>();
            BooleanStruct tmp;

            /* Encode LTS */
            for(int i = 0; i < models.Count; i ++)
            {
                tmp = new BooleanStruct();
                //tmp.automat = models[i];
                bool flag = false;
                int check = 0;

                // Counts number varibable need to encode list of "event"
                var GlobalEventsBoolVarNeeded = (int)Math.Ceiling((Math.Log(models[i].EventList.Count, 2)));

                Dictionary<string, List<bool>> EncodeEventMapping = new Dictionary<string, List<bool>>();
                Dictionary<int, string> EncodeEventRevertMapping = new Dictionary<int, string>();
                Dictionary<String, String> toStateMapping = new Dictionary<string, string>();
                Dictionary<string, string> fromStateMapping = new Dictionary<string, string>();
                Dictionary<String, String> EventEncodeMapping = new Dictionary<string, string>();

                Encode M1 = models[i].Encoding();
                int t = 0;
                foreach (string globalEvent in models[i].EventList)
                {
                    EncodeEventMapping.Add(globalEvent, Encode.EventEncoding(GlobalEventsBoolVarNeeded, t++));
                    EncodeEventRevertMapping.Add(models[i].EventList.IndexOf(globalEvent), globalEvent);
                }

                /* Encode init state */
                List<bool> temp = M1.InitialStateEncoding;
                var pos = new List<int>();
                int p = 1;

                /* Add init state and count variables need to encode */
                tmp.init.Append("{");
                check = 0;
                foreach (bool b in temp)
                {
                    if (check == temp.Count - 1)
                        flag = true;

                    /* Encode init state */
                    tmp.variable.Append(p + " ");
                    pos.Add(p);
                    tmp.FromState.Append(p + " ");
                    if (b)
                    {
                        tmp.init.Append(p++);
                        check++;
                    }
                    else
                    {
                        tmp.init.Append(-p);
                        p++;
                        check++;
                    }

                    if (flag == true)
                        tmp.init.Append("}");
                    else
                        tmp.init.Append(" & ");
                }

                /* Adds events and count variables need to encode */
                var evtPos = new List<int>();
                for (int k = p; k < p + GlobalEventsBoolVarNeeded; k++)
                {
                    tmp.variable.Append(k + " ");
                    evtPos.Add(k);
                    tmp.EventEncode.Append(k + " ");
                }

                /* Adds States and count variables need to encode */
                var toPos = new List<int>();
                for (int k = p + GlobalEventsBoolVarNeeded; k < p + GlobalEventsBoolVarNeeded + p - 1; k++)
                {
                    tmp.variable.Append(k + " ");
                    toPos.Add(k);
                    tmp.ToState.Append(k + " ");
                }

                /* Encode boolean function */
                var bf = new StringBuilder();
                bf.Append("{");
                check = 0;
                flag = false;
                foreach (EncodedTranstion etran in M1.Transtions)
                {
                    if(check == (M1.Transtions.Count - 1))
                        flag = true;
 
                    var boolean_expression = new List<int>();
                    List<bool> evtEncode = EncodeEventMapping[etran.Event];
                    List<int> fromState = etran.FromState;
                    List<int> toState = etran.ToState;
                    StateBase fromName = etran.fromName;
                    StateBase toName = etran.toName;

                    /* Encode from states */
                    string encode = "";
                    for (int j = 0; j < fromState.Count; j ++)
                    {
                        if (fromState[j] == 1)
                        {
                            boolean_expression.Add(pos[j]);
                            encode += pos[j];
                        }
                        else if (fromState[j] == 0)
                        {
                            boolean_expression.Add(-pos[j]);
                            encode += -pos[j];
                        }

                        if (j != fromState.Count - 1)
                            encode += " & ";
                    }
                    if(!fromStateMapping.ContainsKey(fromName.Name))
                        fromStateMapping.Add(fromName.Name, encode);

                    /* Encode event state */
                    encode = "";
                    for (int j = 0; j < evtEncode.Count; j ++)
                    {
                        if (evtEncode[j])
                        {
                            boolean_expression.Add(evtPos[j]);
                            encode += evtPos[j];
                        }
                        else
                        {
                            boolean_expression.Add(-evtPos[j]);
                            encode += -evtPos[j];
                        }

                        if (j != evtEncode.Count - 1)
                            encode += " & ";
                    }
                    if (!EventEncodeMapping.ContainsKey(etran.Event))
                        EventEncodeMapping.Add(etran.Event, encode);

                    /* Encode to state */
                    encode = "";
                    for (int j = 0; j < toState.Count; j ++)
                    {
                        if (toState[j] == 1)
                        {
                            boolean_expression.Add(toPos[j]);
                            encode += toPos[j];
                        }
                        else if (toState[j] == 0)
                        {
                            boolean_expression.Add(-toPos[j]);
                            encode += -toPos[j];
                        }

                        if (j != toState.Count - 1)
                            encode += " & ";
                    }
                    if (!toStateMapping.ContainsKey(toName.Name))
                        toStateMapping.Add(toName.Name, encode);

                    /* Format boolean function */
                    var sb = new StringBuilder();
                    sb.Append("{");
                    for (int j = 0; j < boolean_expression.Count - 1; j ++)
                    {
                        sb.Append(boolean_expression[j]);
                        sb.Append(" & ");
                    }
                    sb.Append(boolean_expression[boolean_expression.Count - 1].ToString());
                    sb.Append(" }");
                    bf.Append(sb);

                    if(flag == false)
                        bf.Append(" | ");

                    check++;
                }
                bf.Append("}");
                tmp.bool_expression = bf;
                tmp.eventMapping = EventEncodeMapping;
                tmp.fromStateMapping = fromStateMapping;
                tmp.toStateMapping = toStateMapping;
                res.Add(tmp);
            }
            return res;
        }

        public void Output(List<BooleanStruct> out_file)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("d:\\output.txt");
            int j;

            /* Output Header */
            file.WriteLine("Labeled Transition System");
            file.WriteLine("{x, i(x), T(x,e,x')}");
            file.WriteLine();

            /* Output Mapping*/
            for(int i = 0; i < out_file.Count; i ++)
            {
                file.WriteLine("x = {" + out_file[i].variable + "}");
                file.WriteLine("i(x) = " + out_file[i].init);
                file.WriteLine("T(x,e,x') = " + out_file[i].bool_expression);

                file.WriteLine("---Mapping---");
                /* Print from state to file */
                file.Write("From State: " + out_file[i].FromState + " where ");
                j = 0;
                foreach (KeyValuePair<string, string> dic in out_file[i].fromStateMapping)
                {
                    file.Write(dic.Key + " = " + "{" + dic.Value + "}");
                    j++;
                    if (j != out_file[i].fromStateMapping.Count)
                        file.Write(", ");
                }
                file.WriteLine();

                file.Write("Event : " + out_file[i].EventEncode + " where ");
                j = 0;
                foreach (KeyValuePair<string, string> dic in out_file[i].eventMapping)
                {
                    file.Write(dic.Key + " = " + "{" + dic.Value + "}");
                    j++;
                    if (j != out_file[i].eventMapping.Count)
                        file.Write(", ");
                }
                file.WriteLine();

                file.Write("To State: " + out_file[i].ToState + " where ");
                j = 0;
                foreach (KeyValuePair<string, string> dic in out_file[i].toStateMapping)
                {
                    file.Write(dic.Key + " = " + "{" + dic.Value + "}");
                    j++;
                    if (j != out_file[i].toStateMapping.Count)
                        file.Write(", ");
                }
                file.WriteLine();
                file.WriteLine();
            }
            file.Close();
        }
    }
}