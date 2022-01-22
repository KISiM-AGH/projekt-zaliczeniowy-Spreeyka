using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetodyDyskretne
{
    class NeighborhoodUtils
    {
        public static void SetNeumannNeighbours(int width, int height, int length, Element[,,] elements)
        {
            //for (int i = 1; i < width - 1; i++) //2.4 bez parallel
            Parallel.For(1, width - 1, i =>
            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int k = 1; k < length - 1; k++)
                    {
                        elements[i, j, k].neighbours[0] = (elements[i - 1, j, k]);
                        elements[i, j, k].neighbours[1] = (elements[i, j + 1, k]);
                        elements[i, j, k].neighbours[2] = (elements[i, j - 1, k]);
                        elements[i, j, k].neighbours[3] = (elements[i + 1, j, k]);

                        elements[i, j, k].neighbours[4] = (elements[i, j, k + 1]);
                        elements[i, j, k].neighbours[5] = (elements[i, j, k - 1]);
                    }
                }
            });
        }

        public static void SetMooreNeighbours(int width, int height, int length, Element[,,] elements)
        {
            //praktycznie bez efektów to zrównoleglanie tutaj
            //for (int i = 1; i < width - 1; i++)
            Parallel.For(1, width - 1, i =>
            {
                for (int j = 1; j < height - 1; j++)
                //Parallel.For(1, height - 1, j =>
                {
                    for (int k = 1; k < length - 1; k++) //2.05 bez parallel. 2.4 wszystkie. 2.05 dwie zewnętrzne. 2.04 zewnętrzna pierwsza
                    //Parallel.For(1, length - 1, k =>
                    {
                        //same
                        elements[i, j, k].neighbours[0] = (elements[i - 1, j - 1, k]);
                        elements[i, j, k].neighbours[1] = (elements[i - 1, j, k]);
                        elements[i, j, k].neighbours[2] = (elements[i - 1, j + 1, k]);

                        elements[i, j, k].neighbours[3] = (elements[i, j + 1, k]);
                        elements[i, j, k].neighbours[4] = (elements[i + 1, j + 1, k]);

                        elements[i, j, k].neighbours[5] = (elements[i + 1, j, k]);
                        elements[i, j, k].neighbours[6] = (elements[i + 1, j - 1, k]);
                        elements[i, j, k].neighbours[7] = (elements[i, j - 1, k]);

                        elements[i, j, k].neighbours[8] = (elements[i - 1, j - 1, k - 1]);
                        elements[i, j, k].neighbours[9] = (elements[i - 1, j, k - 1]);
                        elements[i, j, k].neighbours[10] = (elements[i - 1, j + 1, k - 1]);

                        elements[i, j, k].neighbours[11] = (elements[i, j + 1, k - 1]);
                        elements[i, j, k].neighbours[12] = (elements[i, j, k - 1]);
                        elements[i, j, k].neighbours[13] = (elements[i + 1, j + 1, k - 1]);

                        elements[i, j, k].neighbours[14] = (elements[i + 1, j, k - 1]);
                        elements[i, j, k].neighbours[15] = (elements[i + 1, j - 1, k - 1]);
                        elements[i, j, k].neighbours[16] = (elements[i, j - 1, k - 1]);

                        elements[i, j, k].neighbours[17] = (elements[i - 1, j - 1, k + 1]);
                        elements[i, j, k].neighbours[18] = (elements[i - 1, j, k + 1]);
                        elements[i, j, k].neighbours[19] = (elements[i - 1, j + 1, k + 1]);

                        elements[i, j, k].neighbours[20] = (elements[i, j + 1, k + 1]);
                        elements[i, j, k].neighbours[21] = (elements[i, j, k + 1]);
                        elements[i, j, k].neighbours[22] = (elements[i + 1, j + 1, k + 1]);

                        elements[i, j, k].neighbours[23] = (elements[i + 1, j, k + 1]);
                        elements[i, j, k].neighbours[24] = (elements[i + 1, j - 1, k + 1]);
                        elements[i, j, k].neighbours[25] = (elements[i, j - 1, k + 1]);
                    }
                }
            });
        }
    }
}
