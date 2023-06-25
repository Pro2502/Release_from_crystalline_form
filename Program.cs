using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Release_from_crystalline_form
{
    class Program
    {
        const double Dt = 0.1;//для лоратадина , 0,0014 -  для кетопрофена
        const double k = 0.4;
        static void Main(string[] args)
        {
            Console.ReadLine();

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);


            CellularAutomata cellularAutomata = new CellularAutomata();
            cellularAutomata.Initialisation();
            //cellularAutomata.Field_output();

            bool no_end = true;
            bool can_update = true;
            while (no_end)
            {
                Console.Title = cellularAutomata.CurrentGeneration.ToString();

                while (can_update)
                {
                    //Console.WriteLine("Press ENTER to go through the next iteration");
                    //string str = Console.ReadLine();
                    //if (str == "")
                    //{
                    if (no_end)
                    {
                        cellularAutomata.Transition_Rule_dissolution(k);
                        //Console.WriteLine("After dissolution");
                        //cellularAutomata.Field_output();
                        cellularAutomata.Transition_Rule_diffusion(Dt);
                        cellularAutomata.Transformation();
                        //Console.WriteLine("After diffusion");

                        cellularAutomata.quantityCurve.Add((double)cellularAutomata.quantity);
                        Console.WriteLine((double)cellularAutomata.quantity);
                        cellularAutomata.Iteration_Count(ref no_end);
                        //cellularAutomata.Field_output(no_end);

                        //cellularAutomata.ReadAutomataTxt();
                        
                    }
                    else
                    {
                        cellularAutomata.Field_output(no_end);
                        cellularAutomata.WriteAutomataToTxt();
                        break;
                    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("!!!You have exited the dissolution visualization program!!!");
                    //    can_update = false;
                    //}
                }
            }
        }
    }
}