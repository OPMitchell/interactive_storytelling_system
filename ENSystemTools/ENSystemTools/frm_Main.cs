using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ENSystemTools
{
    public partial class frm_Main : Form
    {
        DataSet dataSet;
        DataTable a;
        DataTable b;
        DataTable c;
        public frm_Main()
        {
            InitializeComponent();

            //Create xml reader
            XmlReader xmlFile = XmlReader.Create("Pikachu.xml", new XmlReaderSettings());
            dataSet = new DataSet();
            //Read xml to dataset
            dataSet.ReadXml(xmlFile);
            //Pass empdetails table to datagridview datasource
            a = dataSet.Tables[0];
            b = dataSet.Tables[1];
            c = dataSet.Tables[2];

            dataGridView1.DataSource = a;
            dataGridView2.DataSource = b;
            dataGridView3.DataSource = c;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string emotionName;
            if (e.RowIndex != -1)
            {
                emotionName = (string)dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value;
                for (int i = 0; i < dataGridView3.Rows.Count-1; i++)
                {
                    if (dataGridView3.Rows[i].Cells[0].Value.ToString() == emotionName)
                    {
                        dataGridView3.Rows[i].Selected = true;
                        dataGridView3.Rows[i].Visible = true;
                    }
                    else
                    {
                        CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView3.DataSource];
                        currencyManager1.SuspendBinding();
                        dataGridView3.Rows[i].Visible = false;
                        dataGridView3.Rows[i].Selected = false;
                        currencyManager1.ResumeBinding();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataSet.WriteXml("test.xml");
        }
    }
}
