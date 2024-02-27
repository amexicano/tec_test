using System.Reflection.Metadata;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace ECD_Handler.Handler
{
    class ECD_File
    {
        // O. class to handle xml statements
        private XDocument xml_doc;

        public string estadocuenta_id { get; private set; }

        public Dictionary<string, double> suma_conceptos { get; }
        public Dictionary<string, List<string>> conceptos { get; }

        public ECD_File(string file)
        {
            xml_doc = XDocument.Load(file);
            suma_conceptos = new Dictionary<string, double>();
            conceptos = new Dictionary<string, List<string>>();
            GetDetallesFactura();
        }

        // Get facturas in Liquidacion by num_liq (Default 0)
        public void GetDetallesFactura(int num_liq = 0)
        {
            this.estadocuenta_id = xml_doc.Root.Attribute("FUECD").Value;

            foreach (XElement liquidacion in xml_doc.Descendants("liquidacion"))
            {
                // Check if num_liq is the number in the arg
                XAttribute liq = liquidacion.Attribute("num_liq");
                if (liq is not null && Int32.Parse(liq.Value) == num_liq)
                { 
                    
                    Double suma;
                    List<string> conceptos_factura;
                    // Get the info for each invoice
                    foreach (XElement factura in liquidacion.Element("facturas").Descendants("factura"))
                    {
                        //Init values of suma (0) and conceptos_factura(empty)
                        suma = 0f;
                        conceptos_factura = new List<string>();
                        foreach (XElement concepto in factura.Descendants("concepto"))
                        {
                            conceptos_factura.Add(concepto.Attribute("ful").Value);
                            suma += Double.Parse(concepto.Element("monto_total").Value);
                        }
                        conceptos.Add(factura.Attribute("fuf").Value, conceptos_factura);
                        suma_conceptos.Add(factura.Attribute("fuf").Value, suma);
                    }

                }
            }

        }
    }

    class ConfigurationFile
    {
        public  readonly string XML_EXTENSION = "*.xml";
        
        public string WorkingDirectory { get; }

        //Default configuration
        public ConfigurationFile() {
            WorkingDirectory = Path.Join(get_home_dir(), "Documents", "ecd", "xml_files"); // * 1. Target dir where statements can be found
        }

        public ConfigurationFile(string filename)
        {
            // Check if exists the file
            if( filename is not null && File.Exists(filename)){

                string extension = Path.GetExtension(filename);

                // Check valid file extension (config, json, txt) otherwise throw a FileLoadException
                if (extension == ".json")
                {
                    Config source;
                    using (StreamReader r = new StreamReader(filename))
                    {
                        string json = r.ReadToEnd();
                        source = JsonSerializer.Deserialize<Config>(json);
                        WorkingDirectory = source.pathname;
                    }

                    return;
                }

                if (extension == ".txt")
                {
                    string[] lines = File.ReadAllLines(filename);

                    foreach (string line in lines)
                    {
                        if (line.Split('=')[0].Equals("pathname"))
                        {
                            WorkingDirectory = line.Split('=')[1];
                            return;
                        }
                    }

                    throw new FileLoadException("No hay suficientes parametros");
                }

                if (extension == ".config")
                {
                    XmlDocument xml_doc = new XmlDocument();
                    xml_doc.Load(filename);
                    WorkingDirectory = xml_doc.DocumentElement.SelectSingleNode("pathfile").InnerText;
                    return;
                }

                WorkingDirectory = "";
                throw new FileLoadException("Archivo no válido");
            }
            else
            {
                WorkingDirectory = "";
                throw new FileNotFoundException();
            }
        }

        // to get current user dir (home dir)
        private string get_home_dir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        // return a list of filenames of xml to process 
        public string[] FilesToProcess()
        { 
            return Directory.GetFiles(WorkingDirectory, XML_EXTENSION ); // * Always xml
        }
    }

    class Config
    {
        
        public string pathname { get; set; }

        public Config()
        {
            pathname = "";
        }

        public Config(string pathname)
        {
            this.pathname = pathname;
        }
    }
}