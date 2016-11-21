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

namespace _8_bit_Checksum_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string addrStart = txtAddrStart.Text;
            string addrEnd = txtAddrEnd.Text;

            if (!isHex(addrStart))
            {
                MessageBox.Show("Starting address is invalid. Enter correct starting address in hexadecimal");
                return;
            }
            if (!isHex(addrEnd))
            {
                MessageBox.Show("Ending address is invalid. Enter correct ending address in hexadecimal");
                return;
            }

            string[] data = txtData.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int lineNumber = 0;

            foreach (string line in data)
            {
                int size = 0;
                foreach(string bytes in line.SplitInParts(2))
                {
                    if (!isHex(bytes))
                    {
                        MessageBox.Show("Data is invalid. Enter correct data in hexadecimal");
                        return;
                    }
                    size++;
                }
                if (size != 16)
                {
                    MessageBox.Show("Data size mismatch. Make sure each data line is 16 bytes in size.");
                    return;
                }
                lineNumber++;
            }

            int start = Int32.Parse(addrStart, System.Globalization.NumberStyles.HexNumber);
            int end = Int32.Parse(addrEnd, System.Globalization.NumberStyles.HexNumber);
            
            if(!((end - start)/16+1 == lineNumber))
            {
                MessageBox.Show("Address length and data length does not match!");
                return;
            }

            //calculate checksum and print
            List<string> output = new List<string>();

            foreach(string line in data)
            {
                string tempdata = "10" + start.ToString("X4") + "00" + line;
                byte[] hexdata = StringToByteArray(tempdata);
                byte total = (byte)(~(int)ComputeAdditionChecksum(hexdata) + 1);
                string checksum = total.ToString("X2");
                output.Add(":" + tempdata + checksum);
                start += 16;
            }

            txtResult.Text = "";

            foreach (string result in output)
            {
                txtResult.Text += (result + Environment.NewLine);
            }
        }

        private bool isHex(string input)
        {
            try
            {
                Int32.Parse(input, System.Globalization.NumberStyles.HexNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte ComputeAdditionChecksum(byte[] data)
        {
            long longSum = data.Sum(x => (long)x);
            return unchecked((byte)longSum);
        }
    }

    static class StringExtensions
    {

        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
}
