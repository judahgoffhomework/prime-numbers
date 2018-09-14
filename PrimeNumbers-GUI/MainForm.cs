using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimeNumbers_GUI
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private int start;
        private int end;
        private bool isPaused = false;
        private bool completion = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            // Find all prime numbers starting between the first and last numbers
            try
            {
                start = Convert.ToInt32(startNumTextBox.Text);
                end = Convert.ToInt32(endNumTextBox.Text);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Number!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            numbersTextBox.Clear();

            StartFindingNumbers();

            UseWaitCursor = true;

            cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                FindPrimeNumbers();
            }, cancellationTokenSource.Token);


            // Let the user know we did something even if no prime nums were found
            if (numbersTextBox.TextLength == 0)
            {
                numbersTextBox.Text = "None.";
            }

            UseWaitCursor = false;

            if (completion)
            {
                ResetForm();
            }
        }

        private void ResetForm()
        {
            // Reset the form
            startNumTextBox.Enabled = true;
            endNumTextBox.Enabled = true;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Visible = false;
            cancelButton.Enabled = false;
            pauseButton.Enabled = false;
            startButton.Enabled = true;
            completion = false;
            isPaused = false;
        }

        private void StartFindingNumbers()
        {
            // Prevent user from messing with certain controls while job is running
            progressBar1.Minimum = start;
            progressBar1.Maximum = end;
            progressBar1.Visible = true;
            cancelButton.Enabled = true;
            pauseButton.Enabled = true;
            startNumTextBox.Enabled = false;
            endNumTextBox.Enabled = false;
        }

        private void FindPrimeNumbers()
        {
            // See which numbers are factors and append them to the numbers text box
            for (int i = start; i <= end; i++)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    break;
                }

                if (IsPrime(i))
                {
                    try
                    {
                        Invoke((Action)delegate ()
                        {
                            AddNumberToTextBox(i);
                        });
                    }
                    catch (ObjectDisposedException) { }
                }

                if (isPaused)
                {
                    start = i;
                    break;
                }

                if (i + 1 > end)
                {
                    completion = true;
                    try
                    {
                        Invoke((Action)delegate ()
                       {
                           ResetForm();
                       });
                    }
                    catch (ObjectDisposedException) { }
                }
            }
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

        private async void pauseButton_Click(object sender, EventArgs e)
        {
            // Pause or resume the current job
            if (isPaused)
            {
                isPaused = false;
                await Task.Run(() =>
                {
                    FindPrimeNumbers();

                }, cancellationTokenSource.Token);
            }
            else
            {
                isPaused = true;
                pauseButton.Text = "Resume";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Cancel the job
            cancellationTokenSource.Cancel();
            ResetForm();
        }
    }
}
