using System;
using System.IO;
using System.Threading;

namespace CBS
{
	public static class EFD_File_Handler
	{
        private static StreamReader MyStreamReader;

        public static void Handle_New_File (string Path)
		{

            while (true)
            {
                try
                {
                    using (MyStreamReader  = System.IO.File.OpenText(Path))
                    {
                        if (MyStreamReader != null)
                        {
                            //// Pass in stream reader and initialise new
                            //// EFD message. 
                            EFD_Msg EDF_MESSAGE = new EFD_Msg(MyStreamReader);

                            MyStreamReader.Close();
                            MyStreamReader.Dispose();

                            //// Generate output
                            Generate_Output.Generate(EDF_MESSAGE);

                            //// Once done with the file, 
                            //// lets delete it as we do not
                            //// needed it any more
                            try
                            {
                                System.IO.File.Delete(Path);
                            }
                            catch (Exception e)
                            {

                            }

                            break;
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                   
                }
                Thread.Sleep(500);
            }
            
		}
	}
}

