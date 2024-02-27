// Program to select and process Daily Statements Accounts (ECD in spanish)
//  Bravos tecnichal test for web developers applicants
//  Follow the instructions from README file

// Handlers are going to be used to process the xml files
using ECD_Handler.Handler;

namespace ECD_Handler
{
    class Program
    {
        // to print several strings at once, with a defined separator and ending char/string
        static void print_msgs( string[]? txt_msgs = null, string sep = "\n", string end = "\n")
        {
            if( txt_msgs is not null )
            {
                foreach (string txt in txt_msgs.Take(txt_msgs.Length - 1))
                {
                    Console.Write("{0}{1}", txt, sep);
                }

                Console.Write("{0}{1}", txt_msgs[txt_msgs.Length - 1], end);
            }
        }
        
         // Return ECD data retrieve from files
        static List<ECD_File> process_files_dir( string[] files ) // Currently not returning a value
        {
            List<ECD_File> ecd_registry = new List<ECD_File> ();

            string[] file_msg_aux= { "" 
                    , new String('-', 30)
            };

            foreach(string file in files)
            {
                file_msg_aux[0] = file.Split('\\')[file.Split('\\').Length - 1];

                // only displays the filename
                print_msgs(txt_msgs: file_msg_aux);
                
                // 2. Processing: either by adding local functions or by developing the Handler class
                ecd_registry.Add(new ECD_File(file));
            }
            return ecd_registry;
            
        }

        static void Main(string[] args)
        {
            // 1. Target Dir where statements can be found which return an array of the names of the files

            // Default Value for Working Directory: %user%/Documents/ecd/xml_files
            ConfigurationFile config = new ConfigurationFile();

            // Use JSON to configuration
            //string extension = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\config.json"));
            
            // Use TXT to configuration
            //string extension = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\config.txt"));
            
            // Use config to configuration
            //string extension = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\App.config"));

            //ConfigurationFile config = new ConfigurationFile(extension);

            string[] files = config.FilesToProcess();

            string[] welcome_msg = {"# Programa ECD Handler"
                                ,"# The files are going to be selected from " + config.WorkingDirectory
                                ,"Files Readed"};

            print_msgs(txt_msgs: welcome_msg, sep: "\n\n");

            // 3. Results returning??
            List<ECD_File> registries = process_files_dir(files);

            // 4. Print the total of each invoice for each ECD provided
            registries.ForEach((registry) =>
            {
                Console.WriteLine("Estado cuenta {0} :\n", registry.estadocuenta_id);
                foreach(KeyValuePair<string,double> kvp in registry.suma_conceptos)
                {
                    Console.WriteLine("Factura ID {0}:", kvp.Key);

                    registry.conceptos[kvp.Key].ForEach((concepto_id) =>
                        Console.Write("Concepto ID: {0}\n", concepto_id)
                    );

                    Console.Write("\nSuma Total Conceptos: {0}\n\n", kvp.Value);
                }

                Console.WriteLine();
            });
        }
    }

}

