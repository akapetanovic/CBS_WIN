﻿using System;
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

            string File_Name = @"C:\CBS\test.log";
            MyStreamReader = System.IO.File.OpenText(File_Name);

            // Pass in stream reader and initialise new
            // EFD message. 
            EFD_Msg EDF_MESSAGE = new EFD_Msg(MyStreamReader);
        }
    }
}