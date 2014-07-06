using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Server.Tests
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
            IO.IO.Initialize(Application.StartupPath + "\\");
            WonderMails.Config.Initialize();
        }

        private void button1_Click(object sender, EventArgs e) {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.IndentChars = "  ";
            settings.Indent = true;
            settings.NewLineChars = Environment.NewLine;

            using (XmlWriter writer = XmlWriter.Create("completed.xml", settings)) {
                writer.WriteStartDocument();
                writer.WriteStartElement("Experience");

                for (int i = 0; i < 100; i++) {
                    writer.WriteStartElement("Exp");
                    writer.WriteAttributeString("level", (i + 1).ToString());
                    writer.WriteString(((i + 1) * 1500).ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            IO.XmlEditor editor = new IO.XmlEditor(IO.Paths.StartFolder + "test.xml", "Data");
            editor.SaveSetting("1", "XEQqDMLIx1gr506LTPHdNdUc2Y74Ck/F1HpdK7si9fziKYSysixrC5//CvyajTjvQe+u9sqKqOmgjQ6TTeVu5A==");
            editor.SaveSetting("2", "nCEOVxi6tgnWOO41rdKXf1HLat4E56ZaDFpzkKRH/8PbZx3bIcVqdzMeNx6jtjBBVpGuR1h+ooo/WRvqX58lrw==");
            editor.SaveSetting("3", "04527PYMR7tfc7RnrF6/av2DqUSBt9JG\"rfMJyRBey92iLAtpLQRHa4bP6v1OM3fP1VUuygByj0ah33BRwddmBQ==");
            editor.Save();

            watch.Stop();
            Console.WriteLine("XmlDocument: " + watch.Elapsed.ToString());

            watch.Reset();

            watch.Start();

            //XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.IndentChars = "  ";
            settings.Indent = true;
            settings.NewLineChars = Environment.NewLine;
            using (XmlWriter writer = XmlWriter.Create("completed.xml", settings)) {
                writer.WriteStartDocument();
                writer.WriteStartElement("CompletedList");

                writer.WriteStartElement("mail");
                writer.WriteElementString("code", "XEQqDMLIx1gr506LTPHdNdUc2Y74Ck/F1HpdK7si9fziKYSysixrC5//CvyajTjvQe+u9sqKqOmgjQ6TTeVu5A==");
                writer.WriteEndElement();

                writer.WriteStartElement("mail");
                writer.WriteElementString("code", "nCEOVxi6tgnWOO41rdKXf1HLat4E56ZaDFpzkKRH/8PbZx3bIcVqdzMeNx6jtjBBVpGuR1h+ooo/WRvqX58lrw==");
                writer.WriteEndElement();

                writer.WriteStartElement("mail");
                writer.WriteElementString("code", "04527PYMR7tfc7RnrF6/av2DqUSBt9JG\"rfMJyRBey92iLAtpLQRHa4bP6v1OM3fP1VUuygByj0ah33BRwddmBQ==");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            watch.Stop();
            Console.WriteLine("XmlWriter: " + watch.Elapsed.ToString());
        }

        private void button2_Click(object sender, EventArgs e) {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            IO.XmlEditor editor = new IO.XmlEditor(IO.Paths.StartFolder + "test.xml", "Data");
            editor.TryGetSetting("1");
            editor.TryGetSetting("2");
            editor.TryGetSetting("3");
            watch.Stop();
            Console.WriteLine("XmlDocument: " + watch.Elapsed.ToString());

            watch.Reset();

            watch.Start();
            using (XmlReader reader = XmlReader.Create("completed.xml")) {
                while (reader.Read()) {
                    // Only detect start elements.
                    if (reader.IsStartElement()) {
                        // Get element name and switch on it.
                        switch (reader.Name) {
                            case "mail":
                                // Detect this article element.
                                //Console.WriteLine("Start <article> element.");
                                // Search for the attribute name on this current node.
                                //string attribute = reader["name"];
                                //if (attribute != null) {
                                //    Console.WriteLine("  Has attribute name: " + attribute);
                                //}
                                // Next read will contain text.
                                if (reader.Read()) {
                                    // We found some sub-elements
                                    //if (reader.Read()) {
                                    reader.ReadElementString("code");
                                    //}
                                    //MessageBox.Show(reader.Value.Trim());
                                }
                                break;
                        }
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("XmlReader: " + watch.Elapsed.ToString());

        }
    }
}
