using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeMotion
{
    class VMBallClock:INotifyPropertyChanged
    {
        private Queue<MDataBalls> clock;
        private List<MDataBalls> minutes, fiveMinutes, hours;
        private List<MDataBalls> tempList;
        private int nbrClocks;
        private int[] clocksFileContent;
        private string fileLocation;
        private const string FOLDER_NAME = "datafile";
        private const string INPUT_FILE_NAME = "input.txt";     //source clock file
        private const string OUTPUT_FILE_NAME = "output.txt";   //Time of motions file
        private const string OUTPUT_TEXT = "balls cycle after";
        private const string OUTPUT_DAYS = "days";
        private const string HEADER_TEXT = "Outputs: Number of balls, Time of motion";       



        #region OutputData: Write data (nbrBalls treated, Time/ duration of motion in day)
        public void OutputData(bool createFile = false)
        {
            string fullName;
            StreamWriter strWrite;

            fileLocation = Path.Combine(System.Environment.CurrentDirectory, FOLDER_NAME); //System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //fileLocation = Path.Combine(location);
            //Directory.CreateDirectory(fileLocation);
            fullName = Path.Combine(fileLocation, OUTPUT_FILE_NAME);

            //Create file only if the file doesn't exist or the first time this function is called
            if (!File.Exists(fullName) || createFile)  //new FileInfo(fullName).Length == 0
            {               
                File.WriteAllText(fullName, string.Concat(HEADER_TEXT, Environment.NewLine, "==========================================", Environment.NewLine));                        //create or clear the file if it already exists
            }

            //Write text in file
            strWrite = File.AppendText(fullName);
            strWrite.WriteLine(Environment.NewLine + "nbrBalls " + OUTPUT_TEXT + "Time" + OUTPUT_DAYS); //nbrBalls, Time

            strWrite.Close();
        }
        #endregion

        #region InputData: Read file and convert data to int
        public void InputData()
        {
            const int FILE_NBR = 0;
            string[] clocksFile, tempFileContent;

            fileLocation = Path.Combine(System.Environment.CurrentDirectory, FOLDER_NAME);
            clocksFile = Directory.GetFiles(fileLocation, INPUT_FILE_NAME, SearchOption.TopDirectoryOnly);
            tempFileContent = File.ReadAllLines(clocksFile[FILE_NBR]);

            //Convert number of balls read from string to integer. nbrClock: number max of clocks read
            nbrClocks = 0;
            foreach (string nbrBalls in tempFileContent)
            {
                clocksFileContent[nbrClocks++] = int.Parse(nbrBalls);
            }
        }
        #endregion

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
