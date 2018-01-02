using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
