using System;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Update;
using VDS.RDF.Query;
using VDS.RDF.Writing;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SPARQLRunner
{
    class Program
    {
        static void Main(string[] args)
        {

            string trigFile = "store.trig";
            string queryFile = "ex.rq";
            string query = File.ReadAllText(queryFile);
            Console.WriteLine(trigFile);
            Console.WriteLine(queryFile);
            TripleStore store = new TripleStore();
            TriGParser trigparser = new TriGParser();
            trigparser.Load(store, trigFile);
            
            /*store.ExecuteUpdate(query);
            TriGWriter writer = new TriGWriter();
            writer.Save(store, trigFile);
            return;*/

            Object results = store.ExecuteQuery(query);
            SparqlResultSet rset = (SparqlResultSet)results;
            /*using StreamWriter sw = new StreamWriter("result1.txt", false);
            foreach (SparqlResult sparqlResult in rset)
            {
                string str = $"<{GetString(sparqlResult, "o")}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://dbpedia.org/ontology/Place> .";
                Console.WriteLine(str);
                sw.WriteLine(str);
            }*/
            Dictionary<string, int> maxWidth = new Dictionary<string, int>();
            foreach (SparqlResult sparqlResult in rset)
            {
                foreach(string var in sparqlResult.Variables)
                {
                    if (!maxWidth.ContainsKey(var))
                    {
                        maxWidth.Add(var, var.Length);
                    }
                    maxWidth[var] = maxWidth[var] < sparqlResult[var].ToString().Length ? sparqlResult[var].ToString().Length : maxWidth[var];
                }
            }
            int total = maxWidth.Values.Sum() + maxWidth.Count + 1;
            using StreamWriter sw = new StreamWriter("result.txt", false);
            string str = String.Concat(Enumerable.Repeat('-', total));
            Console.WriteLine(str);
            sw.WriteLine(str);
            str = '|' + String.Join('|', maxWidth.Keys.Select(k => k.PadRight(maxWidth[k]))) + '|';
            Console.WriteLine(str);
            sw.WriteLine(str);
            str = String.Concat(Enumerable.Repeat('=', total));
            Console.WriteLine(str);
            sw.WriteLine(str);
            foreach (SparqlResult sparqlResult in rset)
            {
               str = '|' + String.Join('|', maxWidth.Keys.Select(k => GetString(sparqlResult, k).PadRight(maxWidth[k]))) + '|';
               Console.WriteLine(str);
               sw.WriteLine(str);
            }
            str = String.Concat(Enumerable.Repeat('-', total));
            Console.WriteLine(str);
            sw.WriteLine(str);
        }

        private static string GetString(SparqlResult result, string variable)
        {
            return result.Variables.Contains(variable) ? result[variable].ToString() : String.Empty;
        }

    }
}
