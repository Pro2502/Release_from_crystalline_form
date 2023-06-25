using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Release_from_crystalline_form
{
    class CellularAutomata
    {
        public int CurrentGeneration { get; private set; }
        public string path = @"D:\Дипломная работа\Для диплома\Расчет высвобождения АФИ из кристаллической формы\Release_from_crystalline_form.txt";
        public Cells[,] Field;
        private readonly int _rows = 97;
        private readonly int _cols = 97;
        int N = 65;//это для лоратадина, 58 - для кетопрофена
        private Random _random = new Random();
        public int X = 97 / 2;
        public int Y = 97 /2;
        public double comparison = 0;

        public Calculation[,] Field_for_calculation;

        public double quantity = 0;
        public double difference;
        public List<double> quantityCurve = new List<double>();

      
        static void StartCreate(Cells[,] Field, Calculation[,] Field_for_calculation)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    Field[x, y] = new Cells();
                    Field[x, y].concentration = 0;
                    Field_for_calculation[x, y] = new Calculation();
                    Field_for_calculation[x, y].accumulation_concentration = 0;
                }
            }
        }

        public void Initialisation()
        {
            Field = new Cells[_rows, _cols];
            Field_for_calculation = new Calculation[_rows, _cols];

            CellularAutomata.StartCreate(Field, Field_for_calculation);

            while (N!=0)
            {
                int x = _random.Next(0, Field.GetLength(0));
                int y = _random.Next(0, Field.GetLength(1));
                if (Field[x, y].concentration == 0)
                {
                    Field[x, y].concentration = 1261;
                    N--;
                }
            } 
        }
        public void Transition_Rule_dissolution(double k)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (Field[x, y].concentration >= Field[x, y].saturated_solution)
                    {
                        double dC;

                        for (int i = -1; i <= +1; i++)
                        {
                            for (int j = -1; j <= +1; j++)
                            {
                                var I = (x + i + Field.GetLength(0)) % Field.GetLength(0);
                                var J = (y + j + Field.GetLength(1)) % Field.GetLength(1);

                                var isSelfChecking = I == x && J == y;
                                if (isSelfChecking)
                                    continue;

                                if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                {
                                    if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                    {
                                        if (Field[x, y].saturated_solution > Field[x, y].concentration)
                                        {
                                            dC = -k * (Field[x, y].saturated_solution - Field[x, y].concentration);
                                        }
                                        else
                                        {
                                            dC = k;
                                        }
                                        double limitation = Field[x, y].concentration - dC;
                                        if (limitation >= 0)
                                        {
                                            Field[I, J].concentration += dC;
                                            Field[x, y].concentration -= dC;
                                            quantity += dC;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Transition_Rule_diffusion(double constD)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (Field[x, y].concentration == 0)
                        continue;

                    difference = 0;
                    double recalculation = 0;
                    if (Field[x, y].concentration < Field[x, y].saturated_solution)
                    {
                        double dC;
                        for (int i = -1; i <= +1; i++)
                        {
                            for (int j = -1; j <= +1; j++)
                            {
                                var I = (x + i + Field.GetLength(0)) % Field.GetLength(0);
                                var J = (y + j + Field.GetLength(1)) % Field.GetLength(1);

                                var isSelfChecking = I == x && J == y;
                                if (isSelfChecking)
                                    continue;

                                if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                {
                                    if (Field[I, J].concentration < 0)
                                        //continue;
                                        throw new Exception("Косяк с законом сохранения масс");
                                    if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                    {
                                        if (Field[I, J].concentration < Field[x, y].concentration)
                                        {
                                            dC = constD * (Field[x, y].concentration - Field[I, J].concentration);

                                            double limitation = Field[x, y].concentration - dC;

                                            if (limitation >= 0)
                                            {
                                                difference += dC;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Field[x, y].concentration + Field_for_calculation[x, y].accumulation_concentration - difference < 0)
                    {
                        recalculation = Field[x, y].concentration + Field_for_calculation[x, y].accumulation_concentration / difference;

                        if (Field[x, y].concentration + Field_for_calculation[x, y].accumulation_concentration < Field[x, y].saturated_solution)
                        {

                            for (int i = -1; i <= +1; i++)
                            {
                                for (int j = -1; j <= +1; j++)
                                {
                                    var I = (x + i + Field.GetLength(0)) % Field.GetLength(0);
                                    var J = (y + j + Field.GetLength(1)) % Field.GetLength(1);

                                    var isSelfChecking = I == x && J == y;
                                    if (isSelfChecking)
                                        continue;
                                    if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                    {
                                        if (Field[I, J].concentration < 0)

                                            throw new Exception("Косяк с законом сохранения масс");
                                        if (Field[I, J].concentration + Field_for_calculation[I, J].accumulation_concentration < Field[x, y].saturated_solution)
                                        {
                                            if (Field[I, J].concentration + Field_for_calculation[I, J].accumulation_concentration < Field[x, y].concentration + Field_for_calculation[x, y].accumulation_concentration)
                                            {
                                                double dC = constD * (Field[x, y].concentration - Field[I, J].concentration);
                                                double remainder = dC * recalculation;

                                                Field_for_calculation[x, y].accumulation_concentration = Field_for_calculation[x, y].accumulation_concentration - remainder;
                                                Field_for_calculation[I, J].accumulation_concentration = Field_for_calculation[I, J].accumulation_concentration + remainder;
                                                quantity += remainder;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        double dC;

                        if (Field[x, y].concentration < Field[x, y].saturated_solution)
                        {

                            for (int i = -1; i <= +1; i++)
                            {
                                for (int j = -1; j <= +1; j++)
                                {
                                    var I = (x + i + Field.GetLength(0)) % Field.GetLength(0);
                                    var J = (y + j + Field.GetLength(1)) % Field.GetLength(1);

                                    var isSelfChecking = I == x && J == y;
                                    if (isSelfChecking)
                                        continue;
                                    if ((I != (x - 1) & J == y) | (I == x & J != (y + 1)) | (I != (x + 1) & J == y) | (I == x & J != (y - 1)))
                                    {
                                        if (Field[I, J].concentration < 0)
                                            //continue;
                                            throw new Exception("Косяк с законом сохранения масс");
                                        if (Field[I, J].concentration < Field[x, y].saturated_solution)
                                        {

                                            if (Field[I, J].concentration < Field[x, y].concentration)
                                            {
                                                dC = constD * (Field[x, y].concentration - Field[I, J].concentration);

                                                double limitation = Field[x, y].concentration - dC;

                                                if (limitation >= 0)
                                                {
                                                    Field_for_calculation[x, y].accumulation_concentration = Field_for_calculation[x, y].accumulation_concentration - dC;
                                                    Field_for_calculation[I, J].accumulation_concentration = Field_for_calculation[I, J].accumulation_concentration + dC;
                                                    quantity += dC;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            CurrentGeneration++;
        }
        public void Field_output(bool no_end)
        {
            if (!no_end)
            {
                for (int x = 0; x < Field.GetLength(0); x++)
                {
                    for (int y = 0; y < Field.GetLength(1); y++)
                    {
                        if (Field[x, y].concentration >= Field[x, y].saturated_solution)
                        {
                            int MaxC = (int)(Field[x, y].saturated_solution * 0.9);
                            if (Field[x, y].concentration > MaxC)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("A" + " ");
                                //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("D" + " ");
                                //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                                Console.ResetColor();
                            }
                        }

                        else if (Field[x, y].concentration < Field[x, y].saturated_solution && Field[x, y].concentration > 0)//(int)(Field[x, y].saturated_solution * 0.1))
                        {
                            int MinC = (int)(Field[x, y].saturated_solution * 0.4);
                            if (Field[x, y].concentration < MinC)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("S" + " ");
                                //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("L" + " ");
                                //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("W" + " ");
                            //Console.Write(Math.Round(Field[x, y].concentration, 0) + " ");
                            Console.ResetColor();
                        }
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n");
            //Console.ReadKey();
            Console.WriteLine(quantity);
        }
        public void Transformation()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {

                    Field[i, j].concentration = Field[i, j].concentration + Field_for_calculation[i, j].accumulation_concentration;
                    Field_for_calculation[i, j].accumulation_concentration = 0;
                    if (Field[i, j].concentration < 0)
                    {
                        Field[i, j].concentration = 0;
                    }
                }
            }

        }
        public void Iteration_Count(ref bool no_end)
        {
            bool condition = true;
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                   
                        double approximate_concentration = Math.Round(Field[0, 0].concentration, 3);
                        if (approximate_concentration == Math.Round(Field[x, y].concentration, 3))
                        {
                            condition = false;
                        }
                    
                }
            }
            if (condition)
            {
                Console.WriteLine("The total time for the dissolution of Arogel:" + CurrentGeneration);
                no_end = false;
            }

        }
        public void WriteAutomataToTxt()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                
                foreach (var i in quantityCurve)
                {
                    sw.WriteLine(String.Format("{0}", i));
                }


            }
        }
        
    }
}
