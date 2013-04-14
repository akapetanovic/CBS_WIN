using System;
using System.IO;

namespace CBS
{
	public static class EFD_File_Handler
	{
		public static void Handle_New_File (string Path)
		{
			//System.IO.File.Delete(Path);
			//System.IO.File.Move();

			StreamReader MyStreamReader;
			// Parse the file and extract all data needed by
			// CBS
			MyStreamReader = System.IO.File.OpenText (Path);

			// Pass in stream reader and initialise new
			// EFD message. 
			EFD_Msg EDF_MESSAGE = new EFD_Msg(MyStreamReader);

			// Generate output
			Generate_Output.Generate(EDF_MESSAGE);

			// Once done with the file, 
			// lets delete it as we do not
			// needed it any more
			System.IO.File.Delete(Path);
		}
	}
}

