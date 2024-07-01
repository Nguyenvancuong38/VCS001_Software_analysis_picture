using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Software_analysis_picture
{
    public partial class Form1 : Form
    {
        string valueSelectCD = @"CD C:\VCS001\POT_image_test";
        string nameStationSelected = "T2.3";

        public Form1()
        {
            InitializeComponent();
            LoadDataFromFile();
            LoadComboBoxOptions();
        }

        private void LoadDataFromFile()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameStationSelected}.txt");
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found!");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);

            dataGridView1.Rows.Clear();

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3)
                {
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], "RUN");
                }
            }

            DataGridViewButtonColumn runButtonColumn = new DataGridViewButtonColumn();
            runButtonColumn.Name = "Action";
            runButtonColumn.Text = "RUN";
            runButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(runButtonColumn);

           dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Action"].Index && e.RowIndex >= 0)
            {
                string executablePath = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                //MessageBox.Show("value RUN: " +  executablePath + " - CD: " + valueSelectCD);
                RunCommandInCmd(valueSelectCD, executablePath);
            }
        }

        private void LoadComboBoxOptions()
        {
            string comboBoxFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.txt");
            if (!File.Exists(comboBoxFilePath))
            {
                MessageBox.Show("Options file not found!");
                return;
            }

            string[] lines = File.ReadAllLines(comboBoxFilePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3)
                {
                    comboBox1.Items.Add(new ComboBoxItem(parts[1], parts[2]));
                }
            }

            comboBox1.DisplayMember = "Label";
            comboBox1.ValueMember = "Value";

            // Chọn mục đầu tiên trong ComboBox nếu có dữ liệu
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = comboBox1.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                valueSelectCD = selectedItem.Value;
                nameStationSelected = selectedItem.Label;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameStationSelected}.txt");
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found!");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);

            dataGridView1.Rows.Clear();

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3)
                {
                    dataGridView1.Rows.Add(parts[0], parts[1], parts[2], "RUN");
                }
            }
        }

        private void RunCommandInCmd(string CD, string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = $"cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;//hidden
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.StandardInput.WriteLine(@"C:");
            p.StandardInput.Flush();
            p.StandardInput.WriteLine($"{CD}");
            p.StandardInput.Flush();
            p.StandardOutput.DiscardBufferedData();
            p.StandardInput.WriteLine($"{command}");
            p.StandardInput.Flush();
            p.StandardInput.Close();

            // Đọc và hiển thị kết quả
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();

            p.WaitForExit();

            int index = output.IndexOf(command);
            if (index != -1)
            {
                string result = output.Substring(index);
                richTextBox_result_output.Text = "RUN ==> " + result;
            }
            else
            {
                MessageBox.Show("Command not found in the result string.");
            }


            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show("Error ==> ", error);
            }
        }
    }

    public class ComboBoxItem
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public ComboBoxItem(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
