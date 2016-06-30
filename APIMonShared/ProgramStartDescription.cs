using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace APIMonShared
{

    /// <summary>
    /// This class incapsulates information needed for the experiment launcher to locate and launch a program.
    /// It also provides some additional data needed for the experiments.
    /// </summary>
    [Serializable]
    public class ProgramStartDescription
    {

        /// <summary>
        /// Sequence number generator
        /// </summary>
        private static int id_sequence = 0;

        /// <summary>
        /// Every call returns the next sequence number
        /// </summary>
        /// <returns></returns>
        private static int getNextId() {
            return id_sequence++;
        }


        private int _id;

        /// <summary>
        /// Unique sequnce number of this ProgramStartDescription
        /// </summary>
        public int id {
            get { return _id; }
            //set { _id = value; }
        }

        /// <summary>
        /// Path to the program image directory
        /// </summary>
        private string _image_dir=string.Empty;
        public string image_dir
        {
            get { return _image_dir; }
            set { _image_dir = Path.GetFullPath(value); }
        }

        /// <summary>
        /// Filename of the program image
        /// </summary>
        private string _image_filename = string.Empty;
        public string image_filename
        {
            get { return _image_filename; }
            set {
				if (File.Exists(Path.Combine(image_dir, value))) {
					_image_filename = value;
				} else {
					throw new FileNotFoundException("File not found: "+Path.Combine(image_dir, value)+" Did you forget to set directory first?");
				}
            }
        }

        /// <summary>
        /// Full path to the program image
        /// </summary>
        public string image_path{
            get { return Path.Combine(_image_dir, _image_filename); }
        }

        /// <summary>
        /// Command line to provide to the program
        /// </summary>
        public string command_line = string.Empty;

        /// <summary>
        /// Maximum running time for the program in seconds. Default is 10
        /// </summary>
        public int max_running_time=10;
        public ProgramStartDescription(){
            _id = getNextId();
        }

		/// <summary>
		/// Copying constructor. It copies  ID as well.
		/// </summary>
		/// <param name="prev"></param>
        public ProgramStartDescription(ProgramStartDescription prev){
            _image_dir = prev._image_dir;
            _image_filename = prev._image_filename;
            command_line = prev.command_line;
			max_running_time = prev.max_running_time;
            _id = prev._id;
        }

        /// <summary>
        /// Returns list of executables in the folder.
        /// Example: to include all executables in the folder use descr_list += descr_list.last().getExecutables();
        /// </summary>
        /// <returns></returns>
		public string[] getExecutables() {
			return findExecutables(_image_dir);
		}

        /// <summary>
        /// Returns list of executables in the folder .and all it's subfolders
        /// Example: to include all executables in the folder use descr_list += descr_list.last().getExecutables();
        /// </summary>
        /// <returns></returns>
        public string[] getExecutablesRecursive(){
            return findExecutablesRecursive(_image_dir);
        }

		/// <summary>
		/// Returns list of executables (exe files) in the directory specified
		/// </summary>
		/// <param name="path">directory to look fro executables</param>
		/// <returns></returns>
        public static string[] findExecutables(string path){
            return Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Returns list of executables (exe files) in the directory specified and all subdirectories
        /// </summary>
        /// <param name="path">directory to look for executables</param>
        /// <returns></returns>
        public static string[] findExecutablesRecursive(string path)
        {            
            string[] r=Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories);
            Array.Sort(r);
            return r;
        }


		/// <summary>
		/// Creates new program description with different command line parameters. The rest is kept the same
		/// </summary>
		/// <param name="command_line">newe command line parameters for the program</param>
		/// <returns></returns>
        public ProgramStartDescription useDifferentCommandLine(string command_line){
            ProgramStartDescription result = new ProgramStartDescription(this);
            _id = getNextId();
            result.command_line = command_line;
            return result;
        }


		/// <summary>
		/// Creates new program description with different executable. The rest is kept the same
		/// </summary>
		/// <param name="image_file_name">image file name</param>
		/// <returns></returns>
		public ProgramStartDescription useDifferentImageFile(string image_file_name) {
			ProgramStartDescription result = new ProgramStartDescription(this);
            _id = getNextId();
			result.image_filename = image_file_name;
			return result;
		}

		/// <summary>
		/// This operation creates new program description on the basis of provided one with changed command line parameters
		/// </summary>
		/// <param name="a">program start desription to inherit from</param>
		/// <param name="new_params">new command line paramters for the program</param>
		/// <returns></returns>
        public static ProgramStartList operator +(ProgramStartDescription a, string new_params){
           ProgramStartList result = new ProgramStartList();
            result.Add (a);
            result.Add(a.useDifferentCommandLine(new_params));
            return result;
        }

		public override string ToString() {
			string result = string.Empty;
			result += image_path + command_line + ";max_running_time;" + max_running_time;
			return result;
		}
    }
}
