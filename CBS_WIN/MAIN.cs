using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CBS;

namespace CBS_WIN
{
    public partial class MAIN : Form
    {
        public MAIN()
        {
            InitializeComponent();
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            StreamReader MyStreamReader;

            string File_Name = CBS_Main.Get_Source_Dir() + "test.log";
            MyStreamReader = System.IO.File.OpenText(File_Name);

            // Pass in stream reader and initialise new
            // EFD message. 
            EFD_Msg EDF_MESSAGE = new EFD_Msg(MyStreamReader);

            // Generate output
            Generate_Output.Generate(EDF_MESSAGE);
        }

        private void MAIN_Load(object sender, EventArgs e)
        {
            ////////////////////////////////////////////////////////////////////////////
            // These calls are to be executed in the following order and
            // are not to be changed
            CBS_Main.Initialize();
            this.textBoxSourceDirectory.Text = CBS_Main.Get_Source_Dir();
            this.textBoxDestinationDirectory.Text = CBS_Main.Get_Destination_Dir();
            ///////////////////////////////////////////////////////////////////////////

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CBS_Main.SetSourceAndDestinationPaths(this.textBoxSourceDirectory.Text, this.textBoxDestinationDirectory.Text);
            CBS_Main.SaveSettings();
            CBS_Main.Restart_Watcher();
        }
    }
}
