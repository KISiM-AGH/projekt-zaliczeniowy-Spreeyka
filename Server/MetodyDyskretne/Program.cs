using System;
using System.IO;
using System.Threading.Tasks;

namespace MetodyDyskretne
{
    class Program
    {
        //**
        static void Main()
        {
            Task[] tasks = new Task[3];
            tasks[0] = Task.Factory.StartNew(() => {
                Calculation c1 = new Calculation(1);
                c1.Run(1);
            });
            //tasks[1] = Task.Factory.StartNew(() =>
            //{
            //    Calculation c2 = new Calculation(2);
            //    c2.Run(2);
            //});
            //tasks[2] = Task.Factory.StartNew(() =>
            //{
            //    Calculation c3 = new Calculation(3);
            //    c3.Run(3);
            //});
            Console.Read();
        }
    }
}
