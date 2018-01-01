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
        private List<int> clocksFileContent;
        private int nbrClocks, nbrBalls;
        private string fileLocation;
        private const string FOLDER_NAME = "datafile";
        private const string INPUT_FILE_NAME = "input.txt";     //source clock file
        private const string OUTPUT_FILE_NAME = "output.txt";   //Time of motions file
        private const string OUTPUT_TEXT = "balls cycle after";
        private const string OUTPUT_DAYS = "days";
        private const string HEADER_TEXT = "Outputs: Number of balls, Time of motion";
        private const int ZERO = 0;
        private const int ONE = 0;
        private const int MAX_SIZE_MIN = 4;
        private const int MAX_SIZE_FIVE_MIN = 11;
        private const int MAX_SIZE_HOURS = 11;
        private const bool FIX = true;

        #region Constructor
        public VMBallClock()
        {
            clock = new Queue<MDataBalls>();
            minutes = new List<MDataBalls>();
            fiveMinutes = new List<MDataBalls>();
            tempList = new List<MDataBalls>();
            hours = new List<MDataBalls>
            {
                //The fixed ball: represents 1:00 when minute = 0 and fiveminute = 0
                new MDataBalls(ONE, FIX)
            };
            clocksFileContent = new List<int>();
        }
        #endregion

        #region ID for each list
        private enum ListID
        { 
            HOUR = 1,
            FIVE_MINUTE,
            MINUTE
        }
        #endregion

        #region Time and Motion calcultation
        public void EvaluateTime()
        {
            while (minutes.Count <= MAX_SIZE_MIN)
            {
                if (minutes.Count == MAX_SIZE_MIN)
                    minutes.Reverse();

                minutes.Add(clock.Dequeue());

                if (minutes.Count > MAX_SIZE_MIN)
                    ReleaseBalls((int)ListID.MINUTE);
            }
        }
        #endregion

        public void ReleaseBalls(int listID)
        {
            //MDataBalls tempBall;
            switch (listID)
            {
                case (int)ListID.MINUTE:
                    fiveMinutes.Add(minutes.Last());
                    minutes.RemoveAt(minutes.Count() - ONE);
                    foreach (MDataBalls tempBall in minutes) //for(int i = 0; i < minutes.Count(); i++)
                        clock.Enqueue(tempBall);
                    break;
                case (int)ListID.FIVE_MINUTE:
                    hours.Add(fiveMinutes.Last());
                    fiveMinutes.RemoveAt(fiveMinutes.Count() - ONE);
                    foreach (MDataBalls tempBall in fiveMinutes) //for(int i = 0; i < fiveMinutes.Count(); i++)
                        clock.Enqueue(tempBall);
                    break;
                case (int)ListID.HOUR:
                    hours.Add(fiveMinutes.First());
                    fiveMinutes.RemoveAt(fiveMinutes.Count() - ONE);
                    foreach (MDataBalls tempBall in fiveMinutes) //for(int i = 0; i < fiveMinutes.Count(); i++)
                        clock.Enqueue(tempBall);
                    break;
            }
        }
        
        #region GetClock: get a clock from input file
        public bool GetClock()
        {
            bool newInput;

            nbrClocks = clocksFileContent.Count;
            newInput = true;

            //Execute only if clocksFileContent has some data
            if (nbrClocks > ZERO)
            {
                nbrBalls = clocksFileContent.First();
                clocksFileContent.RemoveAt(ZERO);

                //A zero signifies the end of input
                if (nbrBalls == ZERO)
                    newInput = false;

                //Create balls for the clock
                clock.Clear();
                for (int i = 0; i < nbrBalls; i++)
                    clock.Enqueue(new MDataBalls(i));
            }

            return newInput;
        }
        #endregion

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
            strWrite.WriteLine(Environment.NewLine + nbrBalls + OUTPUT_TEXT + "Time" + OUTPUT_DAYS); //nbrBalls, Time

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
                clocksFileContent.Add(int.Parse(nbrBalls));
                //clocksFileContent = tempFileContent.Select(data => data).Cast<int>().ToList();
            }
            //nbrClocks = clocksFileContent.Count();
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
