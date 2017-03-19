using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGramsCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Database database = new Database("C:\\Users\\jskid\\Documents\\Visual Studio 2015\\Projects\\NGramsProject\\NGramsProject\\bin\\Debug\\Corpus\\CrimeAndPunishment.txt", "3", "5");
        }
    }
}
