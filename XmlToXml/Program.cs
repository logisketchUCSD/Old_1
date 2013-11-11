/**
 * File: Program.cs
 *
 * Authors: Aaron Wolin, Devin Smith, Jason Fennell, and Max Pflueger.
 * Harvey Mudd College, Claremont, CA 91711.
 * Sketchers 2006.
 * 
 * Use at your own risk.  This code is not maintained and not guaranteed to work.
 * We take no responsibility for any harm this code may cause.
 */

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Collections;
using Microsoft.Ink;
using ConverterXML;

namespace XmlToXml
{
	/// <summary>
	/// Convert Microsoft Journal files into an XML standard.
	/// </summary>
	class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ArrayList argArray = new ArrayList(args);			
			int numArgs = argArray.Count;

			string[] files;
 
			if (numArgs == 0)
			{
				Console.WriteLine("*****************************************************************");
				Console.WriteLine("*** XmlToXml.exe");
				Console.WriteLine("*** by Aaron Wolin, Devin Smith, Jason Fennell, and Max Pflueger.");
				Console.WriteLine("*** Harvey Mudd College, Claremont, CA 91711.");
				Console.WriteLine("*** Sketchers 2006.");
				Console.WriteLine("***");
				Console.WriteLine("*** Usage: XmlToXml.exe (-c | -d directory | -r) (-f)");
				Console.WriteLine("*** Usage: XmlToXml.exe input1.jnt [input2.jnt ...]");
				Console.WriteLine("***");
				Console.WriteLine("*** -c: convert all files in current directory");
				Console.WriteLine("*** -d directory: convert all files in the specified directory");
				Console.WriteLine("*** -r: recursively convert files from the current directory");
			
				return;
			}
			else if(argArray.Contains("-c")) //Convert everything in this directory
			{
				files = Directory.GetFiles(Directory.GetCurrentDirectory());
			}
			else if(argArray.Contains("-d")) //Convert everything in specified directory
			{
				if(argArray.IndexOf("-d") + 1  >= argArray.Count)	//Are we in range?
				{
					Console.Error.WriteLine("No directory specified.");
					return;
				}
				else if(!Directory.Exists((string)argArray[argArray.IndexOf("-d") + 1])) //Does dir exist?
				{
					Console.Error.WriteLine("Directory doesn't exist.");
					return;
				}
				else
					files = Directory.GetFiles((string)argArray[argArray.IndexOf("-d") + 1]);
			}
			else if(argArray.Contains("-r")) //Recursive from current dir
			{
				//Get recursive files
				ArrayList rFiles = new ArrayList();
				DirSearch(Directory.GetCurrentDirectory(), ref rFiles);
			
				//Get current dir files
				string [] currDir = Directory.GetFiles(Directory.GetCurrentDirectory());

				files = new string[rFiles.Count + currDir.Length];

				//populate both recursive and current into files
				int current;
				for(current = 0; current < currDir.Length; ++current)
					files[current] = currDir[current];

				foreach(string s in rFiles)
				{
					files[current++] = s;
				}
			}
			else //Convert only the specified files
			{
				files = args;
			}

			string subDir = "convertedXml";
			Directory.CreateDirectory(subDir);
						
			ConverterXML.ReadXML read;
			ConverterXML.MakeXML make;
			int numPages;
			int i;
			string output;

			foreach (string input in files)
			{
				//The Microsoft Journal file must end with .jnt or .jtp
				if ( !input.ToLower().EndsWith(".xml") )
				{
					//Console.Error.WriteLine("Unknown extension in " + input + ", must be .jnt or .jtp");
					continue;
				}				
				
				try
				{
					Console.Write("Trying " + input + " ");
				
					read = new ReadXML(input);
					make = new MakeXML(read.Sketch);
				
					output = subDir + "\\" + Path.GetFileNameWithoutExtension(input) + ".xml";
					
					make.WriteXML(output);

					Console.WriteLine();
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.InnerException);
					Console.WriteLine(e.StackTrace);
					Console.ReadLine();
					continue;
				}				
			}
		}


		/// <summary>
		/// Perform a recursive directory search. http://support.microsoft.com/default.aspx?scid=kb;en-us;303974
		/// </summary>
		/// <param name="sDir">Directory to search recursively</param>
		/// <param name="rFiles">Array to add the files to</param>
		static void DirSearch(string sDir, ref ArrayList rFiles) 
		{
			try	
			{
				foreach (string d in Directory.GetDirectories(sDir)) 
				{
					foreach (string f in Directory.GetFiles(d, "*.*")) 
					{
						rFiles.Add(f);
					}
					DirSearch(d, ref rFiles);
				}
			}
			catch (System.Exception excpt) 
			{
				Console.WriteLine(excpt.Message);
			}
		}
	}
}
