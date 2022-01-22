using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MetodyDyskretne
{
    class Simulation
    {
        //**
        public Element[,,] elements { get; set; }
        public int[] idList { get; set; }
        public int maxIndex { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public string boundary { get; set; }
        public string neighbourhoodType { get; set; }
        public int numberOfNucleons { get; set; }
        public int numberOfIteration { get; set; }


        //static List<Element> elementsToShuffle;
        //static int randomIndex;
        //static int[,,] indexes;
        //static Element randomElement;
        //static int initialId;
        //static int initialEnergy;
        ////static List<int> neighborsIds;
        //static int randomIdIndex;
        //static double randomProbability;
        //static int testId;
        //static int deltaEnergy;

        static Random random;
        public double Kt { get; set; }
        public int NumberOfMonteCarloIterations { get; set; }


        public string TimeOutputFilename { get; set; }

        public string outputFilename { get; set; }
        public Stopwatch Watch { get; set; }


        public void InitializeDataFromFile(string[] lines)
        {          
            width = Int32.Parse(lines[0]) + 2;
            height = Int32.Parse(lines[1]) + 2;
            length = Int32.Parse(lines[2]) + 2;
            boundary = lines[3];
            neighbourhoodType = lines[4];
            numberOfNucleons = Int32.Parse(lines[5]);
            numberOfIteration = Int32.Parse(lines[6]);
            Kt = Double.Parse(lines[7]);
            NumberOfMonteCarloIterations = Int32.Parse(lines[8]);
        }

        public void SetInitials()
        {
            idList = new int[10];
            //elementsToShuffle = new List<Element>();

            for (int i = 0; i < 10; ++i) //cyfry od 0-9 jako początkowe ziarna
            {
                idList[i] = i;
            }


            //InitializeDataFromFile();

            random = new Random();
            //neighborsIds = new List<int>();

            //indexes = new int[width - 1, height - 1, length - 1];
            //for (int j = 1; j < width - 1; j++)
            //{
            //    for (int k = 1; k < height - 1; k++)
            //    {
            //        for (int i = 1; i < length - 1; i++)
            //        {
            //            indexes[j, k, i] = new int();
            //        }
            //    }
            //}
        }

        public void InitializeElements()
        {
            elements = new Element[width, height, length];

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        elements[i, j, k] = new Element(idList[0]);
                    }
                }
            }
            for (int i = 1; i < width - 1; ++i)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int k = 1; k < length - 1; k++)
                    {
                        if (neighbourhoodType == "Moore") elements[i, j, k].neighbours = new Element[26];
                        if (neighbourhoodType == "Neumann") elements[i, j, k].neighbours = new Element[6];
                    }
                }
            }
        }

        public void SetInitialPattern()
        {
            int drawnColorIndex;
            Random random = new Random();
            for (int j = 1; j <= numberOfNucleons; j++)
            //Parallel.For(1, numberOfNucleons, j =>
            {
                drawnColorIndex = random.Next(1, idList.Count()); //random color
                elements[random.Next(1, width - 2), random.Next(1, height - 2), random.Next(1, length - 2)].id = idList.ElementAt(drawnColorIndex);
            }
            SetNeighbors();
        }

        public void SetNeighbors()
        {
            if (neighbourhoodType == "Moore") NeighborhoodUtils.SetMooreNeighbours(width, height, length, elements);
            if (neighbourhoodType == "Neumann") NeighborhoodUtils.SetNeumannNeighbours(width, height, length, elements);
        }

        public void SetIdForBorder()
        {
            //plaszczyzna dolna i gorna (bez krawędzi)
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    elements[i, j, 0].id = elements[i, j, length - 1].id;
                    elements[i, j, length - 1].id = elements[i, j, 0].id;
                }
            }

            //górna i dolna ściana
            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    elements[0, i, j].id = elements[width - 2, i, j].id;
                    elements[width - 1, i, j].id = elements[1, i, j].id;
                }
            }
            //lewa i prawa ściana
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    elements[i, 0, j].id = elements[i, height - 2, j].id;
                    elements[i, height - 1, j].id = elements[i, 1, j].id;
                }
            }

            //wierzchołki         
            elements[0, 0, 0].id = elements[width - 2, height - 2, length - 2].id; //dolna pl lewa gora
            elements[0, height - 1, 0].id = elements[width - 2, 1, length - 2].id; // dolna pl prawy gora ???? było height - 2 w pierwszej czesci rownania
            elements[width - 1, height - 1, 0].id = elements[1, 1, 1].id; //dolna pl prawy dol
            elements[width - 1, 0, 0].id = elements[1, height - 2, length - 2].id; //dolna pl lewy dol

            elements[0, 0, length - 1].id = elements[width - 2, height - 2, length - 2].id; //gorna pl lewa gora
            elements[0, height - 1, length - 1].id = elements[width - 2, 1, length - 2].id; // gorna pl prawy gora ???? było height - 2 w pierwszej czesci rownania
            elements[width - 1, height - 1, length - 1].id = elements[1, 1, 1].id; //gorna pl prawy dol
            elements[width - 1, 0, length - 1].id = elements[1, height - 2, length - 2].id; //gorna pl lewy dol

        }

        public void Grow()
        {
            //Parallel.For(0, numberOfIteration, i =>
            for (int i = 0; i < numberOfIteration; i++)
            {
                {
                    if (boundary == "Periodic") SetIdForBorder();
                    IterateAndMarkForIdChange();
                }
                ChangeIdOfMarked();
            }
        }

        private void IterateAndMarkForIdChange()
        {

            Parallel.For(1, width - 1, j =>
            {
                int winnerColor, max; //zmienne prywatne dla wątków
                int[] idCounter = InitializeLocalIdCounterForThread(); //zmienne prywatne dla wątków
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        if (elements[j, k, i].id == 0) //lecimy po wszystkich białych (id = 0)
                        {
                            if (elements[j, k, i].neighbours.Any(m => m.id != 0)) //jesli element o id = 0 (bialy) ma jakiegokolwiek sąsiada o innym id (inny kolor)
                            {
                                elements[j, k, i].IsColored = true;
                                foreach (var neigh in elements[j, k, i].neighbours.Where(b => b.id != 0))
                                {
                                    ++idCounter[Array.FindIndex(idList, z => z == neigh.id)];                          //podliczanie sąsiadów
                                }
                                max = idCounter.Max();                                                          //który sąsiad wygrywa
                                maxIndex = Array.IndexOf(idCounter, max);
                                winnerColor = idList[maxIndex];
                                elements[j, k, i].idChanged = winnerColor;                                      //oznaczenie zmiany id
                                Array.Clear(idCounter, 0, idCounter.Count());
                            }
                        }
                    }
                }
            });
        }

        private int[] InitializeLocalIdCounterForThread()
        {
            int[] idCounter = new int[idList.Length];
            for (int i = 0; i < idList.Length; ++i)
            {
                idCounter[i] = new int();
            }

            return idCounter;
        }

        private void ChangeIdOfMarked()
        {
            //second phase -> malowanie oznaczonego (zmiana id)
            Parallel.For(1, width - 1, j => // - 0.2s udało się urwać
            {
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        if (elements[j, k, i].IsColored == true)
                        {
                            elements[j, k, i].id = elements[j, k, i].idChanged;
                            elements[j, k, i].IsColored = false;
                        }
                    }
                }
            });
        }

        public void IterateMonteCarlo()
        {
            Parallel.For(0, NumberOfMonteCarloIterations, j =>
            {
                Element randomElement = new Element(0);
                int initialId = 0;
                int initialEnergy = 0;
                int randomIdIndex = 0;
                double randomProbability = 0;
                int testId = 0;
                int deltaEnergy = 0;
                int randomIndex = 0;

                List<Element> localElementsToShuffle = new List<Element>();
                List<int> neighborsIds = new List<int>();
                MonteCarlo(localElementsToShuffle, neighborsIds, randomIndex, randomElement, initialId, 
                    initialEnergy, randomIdIndex, randomProbability, testId, deltaEnergy);
            });
        }

        private void MonteCarlo(List<Element> elementsToShuffle, List<int> neighborsIds, int randomIndex, Element randomElement, int initialId, int initialEnergy, int randomIdIndex, double randomProbability, int testId, int deltaEnergy)
        {
            AddElementsToShuffle(elementsToShuffle); //1. dodajemy elementy do listy
            for (int i = 0; i < elementsToShuffle.Count; i++)
            {
                DrawElement(elementsToShuffle, randomIndex, randomElement); //2. losowanie

                if (randomElement.Energy != 0) //jak każdy sąsiad ma taki sam id (kolor), to nie ma sensu sprawdzać
                {
                    AddAllDifferentIdsOfRandomElementNeighbors(neighborsIds, randomElement, initialId, initialEnergy);
                    DrawNewIdAndCheckEnergy(neighborsIds, randomIdIndex, testId, randomElement, deltaEnergy, initialEnergy);
                    CheckIfAcceptNewElement(deltaEnergy, randomProbability, randomElement, initialId, initialEnergy);
                    neighborsIds.Clear();
                }
                randomElement.Energy = 0;
                ListExt.RemoveBySwap(elementsToShuffle, randomIndex); //3. Usuwanie tego elementu z listy, żeby już go nie losowało
                //elementsToShuffle.Remove(randomElement); 
            }
        }


        private void AddElementsToShuffle(List<Element> elementsToShuffle)
        {
            for (int j = 1; j < width - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        elementsToShuffle.Add(elements[j, k, i]); //dodajemy wszystkie elementy
                    }
                }
            }
        }

        private void DrawElement(List<Element> elementsToShuffle, int randomIndex, Element randomElement)
        {
            randomIndex = random.Next(0, elementsToShuffle.Count);
            randomElement = elementsToShuffle.ElementAt(randomIndex);
            randomElement.SetEnergy();
        }

        private void AddAllDifferentIdsOfRandomElementNeighbors(List<int> neighborsIds, Element randomElement, int initialId, int initialEnergy)
        {
            initialId = randomElement.id;
            initialEnergy = randomElement.Energy;

            foreach (var diffColorElement in randomElement.neighbours.Where(c => c.id != 0)) //dodajemy wszystkie otaczające kolory (id)
            {
                if (neighborsIds.Exists(n => n == diffColorElement.id) == false)
                {
                    neighborsIds.Add(diffColorElement.id);
                }
            }
        }

        private void DrawNewIdAndCheckEnergy(List<int> neighborsIds, int randomIdIndex, int testId, Element randomElement, int deltaEnergy, int initialEnergy)
        {
            randomIdIndex = random.Next(0, neighborsIds.Count); //losowanie koloru spośród sąsiadów i sprawdzenie energii
            testId = neighborsIds.ElementAt(randomIdIndex);
            randomElement.id = testId;
            randomElement.Energy = 0;
            randomElement.SetEnergy();
            deltaEnergy = randomElement.Energy - initialEnergy;
        }

        private void CheckIfAcceptNewElement(int deltaEnergy, double randomProbability, Element randomElement, int initialId, int initialEnergy)
        {
            if (deltaEnergy > 0) //jeśli energia jest większa to korzystamy ze wzoru
            {
                randomProbability = random.NextDouble();
                if (randomProbability > Math.Exp(-(deltaEnergy / Kt)))
                {
                    randomElement.id = initialId;
                    randomElement.Energy = initialEnergy;
                }
            }
        }

        public void Show()
        {
            for (int k = 1; k < length - 1; k++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        Console.Write(elements[i, j, k].id + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

        }

        public void Run()
        {
            
            //Console.WriteLine("Przed");
            //Show();

            //Grow();

            //Console.WriteLine();
            //Console.WriteLine("Po");

            //Show();
            //Console.WriteLine("Monte carlo");

            //IterateMonteCarlo();


        }

        //public static void RunAndSaveOutput()
        //{
        //    ClearTimeOutputFile();

        //    for (int i = 1; i <= CountParameterFiles(); i++)
        //    {
        //        //InitializeWatch(i);


        //        //SetInitials();
        //        //InitializeElements();
        //        //SetInitialPattern();

        //        //Grow();

        //        //IterateMonteCarlo();
        //        //WriteOutputToFile(i);


        //        //SaveTimeElapsedInFile($"Execution Time: {Watch.ElapsedMilliseconds} ms \n");
        //    }
        //}


        public void InitializeWatch()
        {
            Watch = new Stopwatch();        
            Watch.Start();
        }

        public void WriteOutputToFile(int fileIndex)
        {
            outputFilename = $@"C:\Users\Shprei\Desktop\Discrete-methods-main\Server\MetodyDyskretne\output{fileIndex}.txt"; //tutaj dynamicznie
            File.WriteAllText(outputFilename, String.Empty);

            using StreamWriter outputFile = new StreamWriter(outputFilename);
            for (int k = 1; k < length - 1; k++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        outputFile.Write(elements[i, j, k].id + " ");
                    }
                    outputFile.WriteLine();
                }
                outputFile.WriteLine();
            }

        }

        public void ClearTimeOutputFile(int Index)
        {
            TimeOutputFilename = @$"C:\Users\Shprei\Desktop\Discrete-methods-main\Server\MetodyDyskretne\time{Index}.txt";
            File.WriteAllText(TimeOutputFilename, String.Empty);
        }

        public int CountParameterFiles()
        {
            return Directory.GetFiles(@"C:\Users\Shprei\Desktop\Discrete-methods-main\Server\MetodyDyskretne", "parameters*.txt", SearchOption.TopDirectoryOnly).Length;
        }

        public void SaveTimeElapsedInFile(string timeString, int index)
        {
            Watch.Stop();
            var fileName = @$"C:\Users\Shprei\Desktop\Discrete-methods-main\Server\MetodyDyskretne\time{index}.txt";
            File.AppendAllText(fileName, timeString);
            Console.WriteLine($"Execution Time: {Watch.ElapsedMilliseconds} ms");
            //var file = new FileStream(fileName, FileMode.OpenOrCreate);
        }
    }
}
