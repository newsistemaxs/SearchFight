using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SearchFight
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Vars
            GlobalVars globalVars = new GlobalVars();
            int num_args = args.Length;
            string word = string.Empty;
            string res_html = string.Empty;
            string tmp_message = string.Empty;
            string message = string.Empty;
            string winner_message = string.Empty;
            string winner = string.Empty;
            int result = 0;
            int prev_result = 0;
            int tw = 0;

            int[] arr_tw = new int[(num_args * globalVars.arr_se.Length)]; // for total winner
            int[] arr_tmptw = new int[(num_args * globalVars.arr_se.Length)]; // for total winner
            string[] arr_twe = new string[(num_args * globalVars.arr_se.Length)]; // for total winner

            //Get the params (words) to do the "for"
            if (num_args == 0)
            {
                Console.WriteLine("Please, type some words to start the search");

                return;
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Results are listed here:");
            Console.WriteLine("-------------------------------------------------");

            // Get words
            for (int x = 0; x < num_args; x++)
            //for (int x = 0; x < 2; x++)
            {
                word = args[x];

                // Get html from search engines
                int i = 1;

                foreach (string e in globalVars.arr_se)
                {
                    res_html = GetHtml(e, word);

                    // Get Results
                    tmp_message = GetResults(i, res_html);
                    message = ((i == 1 ? word + " -> " + tmp_message : message + " | " + tmp_message));

                    // Get numbers for comparison
                    result = Convert.ToInt32(tmp_message.Substring(tmp_message.IndexOf(":") + 2));

                    // Save result
                    arr_tw[tw] = result;
                    arr_tmptw[tw] = result;
                    arr_twe[tw] = globalVars.arr_namese[(i - 1)];

                    if (i == 2)
                    {
                        Console.WriteLine(message);

                        // Comparing results
                        if (prev_result > result)
                        {
                            winner_message += word + " -> " + globalVars.arr_namese[(i - 2)] + "\n";
                        }
                        else
                        {
                            winner_message += word + " -> " + globalVars.arr_namese[(i - 1)] + "\n";
                        }
                    }

                    prev_result = result;
                    
                    i += 1;
                    tw += 1;
                }
            }

            // Print winners
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Winners are:");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine(winner_message.Substring(0, winner_message.Length - 1));
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");

            // Get winner
            winner = GetWinner(arr_tw, arr_tmptw, arr_twe);
            
            Console.WriteLine("The total winner is: {0}", winner);
        }

        private static string GetWinner(int[] _arr_tw, int[] _arr_tmptw, string[] _arr_twe)
        {
            int winner = 0;

            // Order elements
            Array.Sort(_arr_tw);
            Array.Reverse(_arr_tw);

            // Get winner
            winner = _arr_tw[0];

            // Get winner Id
            int index = Array.IndexOf(_arr_tmptw, winner);

            return _arr_twe[index];
        }

        public static string GetHtml(string _searchengine, string _word)
        {
            //Console.WriteLine("{0}", _searchengine);
            //Console.WriteLine("{0}", _word);

            string url = _searchengine + _word;

            // Url Request
            WebRequest request = WebRequest.Create(url);

            // Get Response
            WebResponse response = request.GetResponse();

            // Open Stream recieved
            StreamReader reader = new StreamReader(response.GetResponseStream());

            // Read content
            string res = reader.ReadToEnd();

            return res;

            // Cerrar los streams abiertos.
            reader.Close();
            response.Close();
        }

        public static string GetResults(int pos_se, string _html)
        {
            //Vars
            GlobalVars globalVars = new GlobalVars();
            string str_find = string.Empty;
            string str_message = string.Empty;
            int index = 0;

            // Find results in html
            switch (pos_se)
            {
                case 1: // for Yahoo
                    str_find = "Siguiente</a><span>";

                    break;
                case 2: // for Bing
                    str_find = "class=\"sb_count\">";

                    break;
                //default:
                //    Console.WriteLine("Default case");
                //    break;
            }

            // Find Results in string
            index = _html.IndexOf(str_find) + str_find.Length;
            str_find = _html.Substring(index);

            // Extract result
            index = _html.IndexOf(" ") + 1;
            str_find = str_find.Substring(0, index).Replace(",", "").Replace(".", "");

            // Message for console
            str_message = globalVars.arr_namese[(pos_se - 1)] + ": " + str_find;

            return str_message;
        }
    }
}
