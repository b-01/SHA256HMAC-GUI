using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace SHA256HMAC_GUI
{
    public partial class MainWindow : Form
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "%userprofile%";
            ofd.Title = "Select File";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.RestoreDirectory = true;
            ofd.ReadOnlyChecked = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxSelectFile.Text = ofd.FileName;
            }
        }

        private void buttonCreateHash_Click(object sender, EventArgs e)
        {
            if (textBoxSelectFile.Text == String.Empty)
            {
                textBoxHash.Text = "";
                MessageBox.Show("Please select a file!", "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxPassword.Text == String.Empty)
            {
                textBoxHash.Text = "";
                MessageBox.Show("Please enter a password!", "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var hash_value = SignFile(textBoxPassword.Text, textBoxSelectFile.Text);
                textBoxHash.Text = hash_value;
                MessageBox.Show("Hash created!", "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("A technical error occurred while creating the Hash!\n\n" + ex.ToString(), "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void buttonVerfiyHash_Click(object sender, EventArgs e)
        {
            if (textBoxSelectFile.Text == String.Empty)
            {
                MessageBox.Show("Please select a file!", "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxPassword.Text == String.Empty)
            {
                MessageBox.Show("Please enter a password!", "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxHash.Text == String.Empty)
            {
                MessageBox.Show("Please enter a hash value!", "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (VerifyFile(textBoxPassword.Text, textBoxSelectFile.Text, textBoxHash.Text))
                {
                    MessageBox.Show("Hash is correct!", "Verify Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("HASH IS NOT CORRECT!", "Verify Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A technical error occurred while verifying the Hash!\n\n" +ex.ToString(), "Hashing Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        // Computes a keyed hash for a source file and returns it as a Hex String
        private string SignFile(String key, String sourceFilePath)
        {
            if (key == String.Empty)
            {
                return String.Empty;
            }

            ASCIIEncoding encoder = new ASCIIEncoding();
            Byte[] keyBytes = encoder.GetBytes(key);

            // Initialize the keyed hash object.
            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                using (FileStream inStream = new FileStream(sourceFilePath, FileMode.Open))
                {
                    // Compute the hash of the input file.
                    byte[] hashValue = hmac.ComputeHash(inStream);
                    // Write Hash Value to HEX
                    return ToHexString(hashValue);
                }
            }
        }

        // Compares the hash with a given hash
        private bool VerifyFile(String key, String sourceFilePath, String comparableHashValue)
        {
            String new_hash = SignFile(key, sourceFilePath);
            if (new_hash == String.Empty || new_hash != comparableHashValue)
            {
                return false;
            }
            return true;
        }
    }
}
