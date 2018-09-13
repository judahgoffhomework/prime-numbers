using System;
using System.Windows.Forms;

namespace PrimeNumbers_GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void startButton_Click(object sender, EventArgs e)
        {
            // Find all prime numbers starting between the first and last numbers
            int firstNum = Convert.ToInt32(startNumTextBox.Text);
            int lastNum = Convert.ToInt32(endNumTextBox.Text);

            numbersTextBox.Clear();

            // Prevent user from messing with certain controls while job is running
            progressBar1.Minimum = firstNum;
            progressBar1.Maximum = lastNum;
            progressBar1.Visible = true;
            cancelButton.Enabled = true;
            pauseButton.Enabled = true;            
            startNumTextBox.Enabled = false;
            endNumTextBox.Enabled = false;

            UseWaitCursor = true;

            // See which numbers are factors and append them to the numbers text box
            for (int i = firstNum; i <= lastNum; i++)
            {
                if (IsPrime(i))
                {
                    AddNumberToTextBox(i);
                }
            }

            // Let the user know we did something even if no prime nums were found
            if (numbersTextBox.TextLength == 0)
            {
                numbersTextBox.Text = "None.";
            }

            UseWaitCursor = false;

            // Reset the form
            startNumTextBox.Enabled = true;
            endNumTextBox.Enabled = true;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Visible = false;
            cancelButton.Enabled = false;
            pauseButton.Enabled = false;
        }

        private bool IsPrime(int num)
        {
            if (num < 2)
                return false;

            // Look for a number that evenly divides the num
            for (int i = 2; i <= num / 2; i++)
                if (num % i == 0)
                    return false;

            // No divisors means the number is prime
            return true;
        }

        private void AddNumberToTextBox(int num)
        {
            numbersTextBox.AppendText(num + "\n");
            progressBar1.Value = num;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            // Pause or resume the current job 
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Cancel the work done in the for loop
        }
    }
}
