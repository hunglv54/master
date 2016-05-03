using System;
using System.Collections.Generic;
using Lib.Convert;
using Thesis;

namespace Lib.Convert
{
    class Program
    {

        static void Main(string[] args)
        {
            LTSTestCase4();
            Console.ReadKey();
        }

        public static void LTSTestCase4()
        {
            ConvertToBF test = new ConvertToBF();
            List<AutomatonBase> models = test.readFile("AP3_INPUT_P.txt");
            List<BooleanStruct> res = test.BoolFormula(models);
            test.Output(res);
        }
    }
}
