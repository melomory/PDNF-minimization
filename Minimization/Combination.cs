using System.Collections.Generic;
using System.Text;

namespace Minimization
{
    /// <summary>
    /// Get combinations
    /// </summary>
    class Combination
    {
        /// <summary>
        /// Generate combinations
        /// </summary>
        /// <param name="alphabet">given alphabet</param>
        /// <param name="n">combination length</param>
        /// <returns>list of combinations</returns>
        public static List<string> GenerateCombinations(string alphabet, int n)
        {
            if (n == 0)
            {
                return new List<string>() { "" };
            }

            List<string> l = new List<string>();

            for (int i = 0; i < alphabet.Length; ++i)
            {
                char ch = alphabet[i];
                string reml = alphabet.Substring(i + 1);

                List<string> coms = GenerateCombinations(reml, n - 1);

                StringBuilder sb = new StringBuilder();
                foreach (var item in coms)
                {
                    sb.Append(ch);
                    sb.Append(item);
                    l.Add(sb.ToString());
                    sb.Clear();
                }
            }
            return l;
        }
        /// <summary>
        /// Get all minimized function forms
        /// </summary>
        /// <param name="equ_system"> equation system of coefficients</param>
        /// <returns>list of all minimized function forms</returns>
        public static List<HashSet<string>> GetMiniminizedFunctions(List<List<string>> equ_system)
        {
            List<List<string>> minimized_functions = new List<List<string>>();
            List<string> temp = new List<string>();
            List<HashSet<string>> hs_list = new List<HashSet<string>>();
            List<HashSet<string>> final_hs_list = new List<HashSet<string>>();

            for (int i = 0; i < equ_system.Count; ++i)
            {
                string tmp = equ_system[i][0];
                foreach (string item in equ_system[i])
                {
                    if (tmp.Length < item.Length)
                    {
                        break;
                    }
                    temp.Add(item);
                }

                if (hs_list.Count == 0)
                {
                    foreach (var item in temp)
                    {
                        minimized_functions.Add(new List<string>() { item });
                        hs_list.Add(new HashSet<string>() { item });
                    }
                }
                else
                {
                    foreach (var item in hs_list)
                    {
                        foreach (var k in temp)
                        {
                            HashSet<string> tm = new HashSet<string>(item);
                            if (!item.Contains(k))
                            {
                                tm.Add(k);
                                final_hs_list.Add(tm);
                            } else
                            {
                                final_hs_list.Add(tm);
                            }
                        }
                    }
                    if (final_hs_list.Count != 0)
                    {
                        hs_list = new List<HashSet<string>>(final_hs_list);
                        final_hs_list.Clear();
                    }    
                }
                temp.Clear();
            }

            // getting minimum number of occurrence
            int min = int.MaxValue;
            foreach (var item in hs_list)
            {
                if (min > item.Count)
                {
                    min = item.Count;
                }
            }

            // removing equal forms and forming a final list of minimized forms
            foreach (var item in hs_list)
            {
                if (min == item.Count)
                {
                    if (final_hs_list.Count > 0)
                    {
                        foreach (var i in final_hs_list)
                        {
                            if (!i.SetEquals(item))
                            {
                                final_hs_list.Add(item);
                                break;
                            }
                        }
                    } else
                    {
                        final_hs_list.Add(item);
                    }
                }
            }
            return new List<HashSet<string>>(final_hs_list);
        }
    }
}
