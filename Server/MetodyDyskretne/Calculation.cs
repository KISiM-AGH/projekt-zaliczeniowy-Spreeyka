using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetodyDyskretne
{
    class Calculation
    {
        public string[] Lines { get; set; }
        public string Filename { get; set; }
        public string TimeOutputFilename { get; set; }
        
        public Calculation(int Index)
        {            
            Filename = $"parameters{Index}.txt";
            Lines = File.ReadAllLines(@$"..\..\..\{Filename}").Skip(12).ToArray(); //tutaj dynamicznie
        }

        //**
        public void Run(int Index)
        {
            Simulation Simulation = new Simulation();

            Simulation.ClearTimeOutputFile(Index);

            Simulation.InitializeWatch();
            Simulation.InitializeDataFromFile(Lines);
            Simulation.SetInitials();
            Simulation.InitializeElements();
            Simulation.SetInitialPattern();

            Simulation.Grow();


            Simulation.IterateMonteCarlo();
            Simulation.WriteOutputToFile(Index);


            Simulation.SaveTimeElapsedInFile($"Execution Time: {Simulation.Watch.ElapsedMilliseconds} ms \n", Index);
        }
    }
}
