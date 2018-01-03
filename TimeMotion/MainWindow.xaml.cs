/* Team one: Time and Motion
Edward Barber - 7925969
Abhishek Sharma - 7719818
Mireille Tabod Epse Nubaga - 6542864
Joseph Kasumba - 8147696
Jeewan Kalia - 8032997
Shane Frost - 5600861 
*/

using System;
using System.Windows;

namespace TimeMotion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VMBallClock vmClock;

        public MainWindow()
        {
            InitializeComponent();
            vmClock = new VMBallClock();
            DataContext = vmClock;
        }

        private void BtnEvaluate_Click(object sender, RoutedEventArgs e)
        {
            bool newFile = true;

            try
            {
                vmClock.InputData();

                //A zero signifies the end of input
                while (vmClock.GetClockBalls())
                {
                    //calculate time
                    vmClock.EvaluateTime();

                    //Output
                    if (newFile)
                    {
                        vmClock.OutputData(newFile);
                        newFile = false;
                    }
                    else
                        vmClock.OutputData();
                }

                MessageBox.Show("End of process", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex.HResult.ToString() + ": " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
