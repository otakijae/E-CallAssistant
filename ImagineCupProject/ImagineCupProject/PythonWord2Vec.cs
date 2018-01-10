using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ImagineCupProject
{
    class PythonWord2Vec
    {
        private static PythonWord2Vec pythonWord2Vec;

        private PythonWord2Vec()
        {

        }

        public static PythonWord2Vec getInstance()
        {
            if (pythonWord2Vec == null)
            {
                pythonWord2Vec = new PythonWord2Vec();
            }
            return pythonWord2Vec;
        }

        public string WordAnalysis(string keyWords)
        {
            
        }
    }


}
