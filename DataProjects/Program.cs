using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace DataProjects
{
    class Program
    {
        static void Main(string[] args)
        {
            //PremierLeagueMainProject.MainUpdatorFromEspn(7, 11);
            MachineLearningProject.PrintTrainingFile();
            MachineLearningProject.PrintTestFile(5, 11);
            Console.WriteLine("Hello world");
            Console.Read();
        }
    }
}
